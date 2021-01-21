﻿using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDKBase;

namespace Hai.Just1Int7Toggles.Scripts.Editor.Internal.Reused
{
    internal class Machinist
    {
        private readonly AnimatorStateMachine _machine;
        private readonly AnimationClip _emptyClip;

        internal Machinist(AnimatorStateMachine machine, AnimationClip emptyClip)
        {
            _machine = machine;
            _emptyClip = emptyClip;
        }

        internal Machinist WithEntryPosition(int x, int y)
        {
            _machine.entryPosition = AnimatorGenerator.GridPosition(x, y);
            return this;
        }

        internal Machinist WithExitPosition(int x, int y)
        {
            _machine.exitPosition = AnimatorGenerator.GridPosition(x, y);
            return this;
        }

        internal Machinist WithAnyStatePosition(int x, int y)
        {
            _machine.anyStatePosition = AnimatorGenerator.GridPosition(x, y);
            return this;
        }

        internal Statist NewState(string name, float x, float y)
        {
            var state = _machine.AddState(name, AnimatorGenerator.GridPosition(x, y));
            state.motion = _emptyClip;
            state.writeDefaultValues = false;

            return new Statist(state);
        }

        public Transitionist AnyTransitionsTo(Statist destination)
        {
            return new Transitionist(Statist.NewDefaultTransition(_machine.AddAnyStateTransition(destination._state)));
        }
    }

    internal class Statist
    {
        internal readonly AnimatorState _state;
        private VRCAvatarParameterDriver _driver;
        private VRCAnimatorTrackingControl _tracking;

        internal Statist(AnimatorState state)
        {
            _state = state;
        }

        internal Statist WithAnimation(Motion clip)
        {
            _state.motion = clip;
            return this;
        }

        internal Transitionist TransitionsTo(Statist destination)
        {
            return new Transitionist(NewDefaultTransition(_state.AddTransition(destination._state)));
        }

        internal Statist AutomaticallyMovesTo(Statist destination)
        {
            var transition = NewDefaultTransition(_state.AddTransition(destination._state));
            transition.hasExitTime = true;
            return this;
        }

        internal Transitionist Exits()
        {
            return new Transitionist(NewDefaultTransition(_state.AddExitTransition()));
        }

        internal static AnimatorStateTransition NewDefaultTransition(AnimatorStateTransition transition)
        {
            transition.duration = 0;
            transition.hasExitTime = false;
            transition.exitTime = 0;
            transition.hasFixedDuration = true;
            transition.offset = 0;
            transition.interruptionSource = TransitionInterruptionSource.None;
            transition.orderedInterruption = true;
            transition.canTransitionToSelf = true;
            return transition;
        }

        internal Statist Drives(IntParameterist parameterist, int value)
        {
            CreateDriverBehaviorIfNotExists();
            _driver.parameters.Add(new VRC_AvatarParameterDriver.Parameter
            {
                name = parameterist.Name, value = value
            });
            return this;
        }

        internal Statist Drives(BoolParameterist parameterist, bool value)
        {
            CreateDriverBehaviorIfNotExists();
            _driver.parameters.Add(new VRC_AvatarParameterDriver.Parameter
            {
                name = parameterist.Name, value = value ? 1 : 0
            });
            return this;
        }

        internal Statist Drives(FloatParameterist parameterist, float value)
        {
            CreateDriverBehaviorIfNotExists();
            _driver.parameters.Add(new VRC_AvatarParameterDriver.Parameter
            {
                name = parameterist.Name, value = value
            });
            return this;
        }

        private void CreateDriverBehaviorIfNotExists()
        {
            if (_driver != null) return;
            _driver = _state.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
            _driver.parameters = new List<VRC_AvatarParameterDriver.Parameter>();
        }

        public Statist WithWriteDefaultsSetTo(bool shouldWriteDefaults)
        {
            _state.writeDefaultValues = shouldWriteDefaults;
            return this;
        }

        public Statist TrackingTracks(TrackingElement element)
        {
            CreateTrackingBehaviorIfNotExists();
            SettingElementTo(element, VRC_AnimatorTrackingControl.TrackingType.Tracking);

            return this;
        }

        public Statist TrackingAnimates(TrackingElement element)
        {
            CreateTrackingBehaviorIfNotExists();
            SettingElementTo(element, VRC_AnimatorTrackingControl.TrackingType.Animation);

            return this;
        }

        private void SettingElementTo(TrackingElement element, VRC_AnimatorTrackingControl.TrackingType target)
        {
            switch (element)
            {
                case TrackingElement.Head:
                    _tracking.trackingHead = target;
                    break;
                case TrackingElement.LeftHand:
                    _tracking.trackingLeftHand = target;
                    break;
                case TrackingElement.RightHand:
                    _tracking.trackingRightHand = target;
                    break;
                case TrackingElement.Hip:
                    _tracking.trackingHip = target;
                    break;
                case TrackingElement.LeftFoot:
                    _tracking.trackingLeftFoot = target;
                    break;
                case TrackingElement.RightFoot:
                    _tracking.trackingRightFoot = target;
                    break;
                case TrackingElement.LeftFingers:
                    _tracking.trackingLeftFingers = target;
                    break;
                case TrackingElement.RightFingers:
                    _tracking.trackingRightFingers = target;
                    break;
                case TrackingElement.Eyes:
                    _tracking.trackingEyes = target;
                    break;
                case TrackingElement.Mouth:
                    _tracking.trackingMouth = target;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(element), element, null);
            }
        }

        private void CreateTrackingBehaviorIfNotExists()
        {
            if (_tracking != null) return;
            _tracking = _state.AddStateMachineBehaviour<VRCAnimatorTrackingControl>();
        }

        internal enum TrackingElement
        {
            Head,
            LeftHand,
            RightHand,
            Hip,
            LeftFoot,
            RightFoot,
            LeftFingers,
            RightFingers,
            Eyes,
            Mouth
        }
    }

    internal class Transitionist
    {
        private readonly AnimatorStateTransition _transition;

        internal Transitionist(AnimatorStateTransition transition)
        {
            _transition = transition;
        }

        internal Transitionist WithExitTimeTo(float value)
        {
            _transition.hasExitTime = true;
            _transition.exitTime = value;
            return this;
        }

        internal BuildingIntTransitionist When(IntParameterist parameter)
        {
            return new BuildingIntTransitionist(new TransitionContinuationist(_transition), _transition, parameter);
        }

        internal BuildingFloatTransitionist When(FloatParameterist parameter)
        {
            return new BuildingFloatTransitionist(new TransitionContinuationist(_transition), _transition, parameter);
        }

        internal BuildingBoolTransitionist When(BoolParameterist parameter)
        {
            return new BuildingBoolTransitionist(new TransitionContinuationist(_transition), _transition, parameter);
        }

        internal TransitionContinuationist Whenever(Action<TransitionContinuationist> action)
        {
            var transitionContinuationist = new TransitionContinuationist(_transition);
            action(transitionContinuationist);
            return transitionContinuationist;
        }

        internal TransitionContinuationist Whenever()
        {
            return new TransitionContinuationist(_transition);
        }

        internal class TransitionContinuationist
        {
            private readonly AnimatorStateTransition _transition;

            internal TransitionContinuationist(AnimatorStateTransition transition)
            {
                _transition = transition;
            }

            internal BuildingIntTransitionist And(IntParameterist parameter)
            {
                return new BuildingIntTransitionist(this, _transition, parameter);
            }

            internal BuildingFloatTransitionist And(FloatParameterist parameter)
            {
                return new BuildingFloatTransitionist(this, _transition, parameter);
            }

            internal BuildingBoolTransitionist And(BoolParameterist parameter)
            {
                return new BuildingBoolTransitionist(this, _transition, parameter);
            }

            internal TransitionContinuationist AndWhenever(Action<TransitionContinuationist> action)
            {
                action(this);
                return this;
            }
        }

        internal class BuildingIntTransitionist
        {
            private readonly TransitionContinuationist _transitionist;
            private readonly AnimatorStateTransition _transition;
            private readonly IntParameterist _parameterist;

            internal BuildingIntTransitionist(TransitionContinuationist transitionist, AnimatorStateTransition transition, IntParameterist parameterist)
            {
                _transitionist = transitionist;
                _transition = transition;
                _parameterist = parameterist;
            }

            internal TransitionContinuationist IsEqualTo(int value)
            {
                _transition.AddCondition(AnimatorConditionMode.Equals, value, _parameterist.Name);
                return _transitionist;
            }

            internal TransitionContinuationist IsNotEqualTo(int value)
            {
                _transition.AddCondition(AnimatorConditionMode.NotEqual, value, _parameterist.Name);
                return _transitionist;
            }

            internal TransitionContinuationist IsLesserThan(int value)
            {
                _transition.AddCondition(AnimatorConditionMode.Less, value, _parameterist.Name);
                return _transitionist;
            }

            internal TransitionContinuationist IsGreaterThan(int value)
            {
                _transition.AddCondition(AnimatorConditionMode.Greater, value, _parameterist.Name);
                return _transitionist;
            }
        }

        internal class BuildingFloatTransitionist
        {
            private readonly TransitionContinuationist _transitionist;
            private readonly AnimatorStateTransition _transition;
            private readonly FloatParameterist _parameterist;

            internal BuildingFloatTransitionist(TransitionContinuationist transitionist, AnimatorStateTransition transition, FloatParameterist parameterist)
            {
                _transitionist = transitionist;
                _transition = transition;
                _parameterist = parameterist;
            }

            internal TransitionContinuationist IsLesserThan(float value)
            {
                _transition.AddCondition(AnimatorConditionMode.Less, value, _parameterist.Name);
                return _transitionist;
            }

            internal TransitionContinuationist IsGreaterThan(float value)
            {
                _transition.AddCondition(AnimatorConditionMode.Greater, value, _parameterist.Name);
                return _transitionist;
            }
        }

        internal class BuildingBoolTransitionist
        {
            private readonly TransitionContinuationist _transitionist;
            private readonly AnimatorStateTransition _transition;
            private readonly BoolParameterist _parameterist;

            internal BuildingBoolTransitionist(TransitionContinuationist transitionist, AnimatorStateTransition transition, BoolParameterist parameterist)
            {
                _transitionist = transitionist;
                _transition = transition;
                _parameterist = parameterist;
            }

            internal TransitionContinuationist IsTrue()
            {
                _transition.AddCondition(AnimatorConditionMode.If, 0, _parameterist.Name);
                return _transitionist;
            }

            internal TransitionContinuationist IsFalse()
            {
                _transition.AddCondition(AnimatorConditionMode.IfNot, 0, _parameterist.Name);
                return _transitionist;
            }
        }

        public Transitionist WithSourceInterruption()
        {
            _transition.interruptionSource = TransitionInterruptionSource.Source;
            return this;
        }

        public Transitionist WithTransitionDuration(float transitionDuration)
        {
            _transition.duration = transitionDuration;
            return this;
        }

        public Transitionist WithNoOrderedInterruption()
        {
            _transition.orderedInterruption = false;
            return this;
        }

        public Transitionist WithNoTransitionToSelf()
        {
            _transition.canTransitionToSelf = false;
            return this;
        }
    }
}
