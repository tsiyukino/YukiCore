using System;
using System.Collections.Generic;
using UnityEngine;

namespace TsiYuki.Core.Editor
{
    /// <summary>
    /// Lets one TsiYuki tool install its menu inside a menu another one
    /// generates, without either having to run first.
    ///
    /// During a build each tool registers the menu it generated and, if the user
    /// pointed it somewhere, asks for it to be moved there. Nothing is wired up
    /// at that moment, because the destination may not exist yet. A pass in this
    /// package runs once every TsiYuki tool has had its turn and settles all the
    /// requests together.
    ///
    /// Everything is keyed by instance id, captured while the components are
    /// still alive: a tool removes its own component at the end of its pass, and
    /// a destroyed component compares equal to null, so holding the references
    /// would mean losing the table before it is ever read.
    ///
    /// This package knows nothing about Modular Avatar or VRChat: the tool that
    /// made the request hands over the code that does the wiring.
    /// </summary>
    public static class YukiMenuRegistry
    {
        class Placement
        {
            public int Source;
            public int Destination;
            public string DestinationName;
            public Action<GameObject, GameObject> Wire;
            public Action<string> OnCycle;
            public Action<string> OnMissing;
        }

        static readonly Dictionary<int, GameObject> Roots = new Dictionary<int, GameObject>();
        static readonly List<Placement> Placements = new List<Placement>();

        /// <summary>Starts a build with nothing left over from the last one.</summary>
        public static void Reset()
        {
            Roots.Clear();
            Placements.Clear();
        }

        /// <summary>"This component's menu is that object."</summary>
        public static void Register(UnityEngine.Object source, GameObject menuRoot)
        {
            if (source == null || menuRoot == null) return;
            Roots[source.GetInstanceID()] = menuRoot;
        }

        public static GameObject Find(UnityEngine.Object source)
        {
            GameObject root;
            return source != null && Roots.TryGetValue(source.GetInstanceID(), out root) ? root : null;
        }

        /// <summary>
        /// "Move my menu inside the menu that component generates." The wiring
        /// runs later, at <see cref="Resolve"/>, with both roots in hand.
        /// </summary>
        public static void Request(UnityEngine.Object source, UnityEngine.Object destination,
                                   Action<GameObject, GameObject> wire,
                                   Action<string> onCycle = null,
                                   Action<string> onMissing = null)
        {
            if (source == null || destination == null || wire == null) return;
            Placements.Add(new Placement
            {
                Source = source.GetInstanceID(),
                Destination = destination.GetInstanceID(),
                DestinationName = destination.name,
                Wire = wire,
                OnCycle = onCycle,
                OnMissing = onMissing,
            });
        }

        /// <summary>
        /// Settles every request. A destination that generated no menu is
        /// reported and left where it was, as is anything that would make a menu
        /// contain itself.
        /// </summary>
        public static void Resolve()
        {
            foreach (var placement in Placements)
            {
                GameObject sourceRoot, destinationRoot;
                if (!Roots.TryGetValue(placement.Source, out sourceRoot) || sourceRoot == null) continue;

                if (!Roots.TryGetValue(placement.Destination, out destinationRoot) || destinationRoot == null)
                {
                    if (placement.OnMissing != null) placement.OnMissing(placement.DestinationName);
                    continue;
                }
                if (Cycles(placement.Source, placement.Destination))
                {
                    if (placement.OnCycle != null) placement.OnCycle(placement.DestinationName);
                    continue;
                }
                placement.Wire(sourceRoot, destinationRoot);
            }
            Placements.Clear();
        }

        /// <summary>
        /// True when following "installs into" from <paramref name="destination"/>
        /// leads back to <paramref name="source"/> — a menu that would end up
        /// inside itself.
        /// </summary>
        static bool Cycles(int source, int destination)
        {
            var seen = new HashSet<int>();
            var at = destination;
            while (seen.Add(at))
            {
                if (at == source) return true;
                bool found = false;
                foreach (var placement in Placements)
                    if (placement.Source == at) { at = placement.Destination; found = true; break; }
                if (!found) return false;
            }
            return false;
        }
    }
}
