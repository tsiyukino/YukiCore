using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor.Animations;
using UnityEngine;

namespace TsiYuki.Core.Editor.Tests
{
    // Everything is built in memory; "persisting" only records what was handed over, and when.
    public class AnimatorGraphTests
    {
        AnimatorController _controller;
        List<Object> _persisted;

        [SetUp]
        public void SetUp()
        {
            _controller = new AnimatorController { name = "AnimatorGraphTests" };
            _persisted = new List<Object>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var asset in _persisted)
                if (asset != null) Object.DestroyImmediate(asset);
            Object.DestroyImmediate(_controller);
        }

        void Persist(Object asset) => _persisted.Add(asset);

        [Test]
        public void LayerIsFullWeightAndItsMachineIsPersisted()
        {
            var machine = AnimatorGraph.AddLayer(_controller, "Outfit", Persist);

            Assert.That(_controller.layers.Length, Is.EqualTo(1));
            Assert.That(_controller.layers[0].name, Is.EqualTo("Outfit"));
            Assert.That(_controller.layers[0].defaultWeight, Is.EqualTo(1));
            Assert.That(_controller.layers[0].stateMachine, Is.SameAs(machine));
            Assert.That(machine.hideFlags, Is.EqualTo(HideFlags.HideInHierarchy));
            CollectionAssert.Contains(_persisted, machine);
        }

        [Test]
        public void StateIsWriteDefaultsOffAndPersistedBeforeItIsAttached()
        {
            var machine = AnimatorGraph.AddLayer(_controller, "Outfit", Persist);
            var clip = new AnimationClip { name = "Clip" };
            bool attachedWhenPersisted = true;

            var state = AnimatorGraph.AddState(machine, "Worn", clip, new Vector3(400, 60), asset =>
            {
                if (asset is AnimatorState) attachedWhenPersisted = machine.states.Any(s => s.state == asset);
                Persist(asset);
            });

            Assert.IsFalse(attachedWhenPersisted);
            Assert.IsFalse(state.writeDefaultValues);
            Assert.That(state.motion, Is.SameAs(clip));
            Assert.That(state.name, Is.EqualTo("Worn"));
            Assert.That(machine.states.Single().position, Is.EqualTo(new Vector3(400, 60)));
            Object.DestroyImmediate(clip);
        }

        [Test]
        public void StateMayHaveNoMotion()
        {
            var machine = AnimatorGraph.AddLayer(_controller, "Looks", Persist);
            var state = AnimatorGraph.AddState(machine, "Idle", null, Vector3.zero, Persist);
            Assert.IsNull(state.motion);
        }

        [Test]
        public void DuplicateStateNameIsMadeUnique()
        {
            var machine = AnimatorGraph.AddLayer(_controller, "Colors", Persist);
            var first = AnimatorGraph.AddState(machine, "Red", null, Vector3.zero, Persist);
            var second = AnimatorGraph.AddState(machine, "Red", null, Vector3.zero, Persist);

            Assert.That(first.name, Is.EqualTo("Red"));
            Assert.That(second.name, Is.Not.EqualTo("Red"));
            StringAssert.StartsWith("Red", second.name);
        }

        [TestCase(3)]
        [TestCase(0)]
        public void AnyStateTransitionEntersAtOnceOnEquals(int value)
        {
            var machine = AnimatorGraph.AddLayer(_controller, "Outfit", Persist);
            var state = AnimatorGraph.AddState(machine, "Worn", null, Vector3.zero, Persist);

            var transition = AnimatorGraph.AddAnyStateTransition(machine, state, "Outfit", value, Persist);

            Assert.That(machine.anyStateTransitions.Single(), Is.SameAs(transition));
            Assert.That(transition.destinationState, Is.SameAs(state));
            Assert.IsFalse(transition.canTransitionToSelf);
            Assert.IsFalse(transition.hasExitTime);
            Assert.That(transition.duration, Is.EqualTo(0));
            var condition = transition.conditions.Single();
            Assert.That(condition.mode, Is.EqualTo(AnimatorConditionMode.Equals));
            Assert.That(condition.threshold, Is.EqualTo(value));
            Assert.That(condition.parameter, Is.EqualTo("Outfit"));
            CollectionAssert.Contains(_persisted, transition);
        }
    }
}
