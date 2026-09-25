using System;
using UnityEditor.Animations;
using UnityEngine;

namespace TsiYuki.Core.Editor
{
    /// <summary>
    /// The pieces every generated FX controller is made of. Everything is
    /// created in memory and handed to <c>persist</c> as it is made, so the
    /// caller decides where it is saved (an NDMF build's asset container, a
    /// test's nothing at all).
    ///
    /// States are write-defaults off; a layer built from them has to animate
    /// every property it touches in every state.
    /// </summary>
    public static class AnimatorGraph
    {
        /// <summary>A full-weight layer with an empty state machine of the same name.</summary>
        public static AnimatorStateMachine AddLayer(AnimatorController controller, string name,
                                                    Action<UnityEngine.Object> persist)
        {
            var machine = new AnimatorStateMachine { name = name, hideFlags = HideFlags.HideInHierarchy };
            persist(machine);
            controller.AddLayer(new AnimatorControllerLayer
            {
                name = name,
                defaultWeight = 1,
                stateMachine = machine,
            });
            return machine;
        }

        /// <summary>
        /// A write-defaults-off state. A name already taken in the machine gets
        /// a suffix, as it would in the Animator window. The state is built here
        /// rather than through <c>machine.AddState(name, position)</c>, which
        /// also adds it to the machine's asset file and registers an undo step
        /// on its own; saving is <paramref name="persist"/>'s job.
        /// </summary>
        public static AnimatorState AddState(AnimatorStateMachine machine, string name, Motion motion,
                                             Vector3 position, Action<UnityEngine.Object> persist)
        {
            var state = new AnimatorState
            {
                name = machine.MakeUniqueStateName(name),
                motion = motion,
                writeDefaultValues = false,
                hideFlags = HideFlags.HideInHierarchy,
            };
            persist(state);
            machine.AddState(state, position);
            return state;
        }

        /// <summary>
        /// Enters <paramref name="state"/> from anywhere, at once, whenever the
        /// int <paramref name="parameter"/> equals <paramref name="value"/>.
        /// </summary>
        public static AnimatorStateTransition AddAnyStateTransition(AnimatorStateMachine machine, AnimatorState state,
                                                                    string parameter, int value,
                                                                    Action<UnityEngine.Object> persist)
        {
            var transition = machine.AddAnyStateTransition(state);
            transition.canTransitionToSelf = false;
            transition.hasExitTime = false;
            transition.duration = 0;
            transition.AddCondition(AnimatorConditionMode.Equals, value, parameter);
            persist(transition);
            return transition;
        }
    }
}
