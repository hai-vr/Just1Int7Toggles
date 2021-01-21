using System;
using System.Collections.Generic;
using System.Linq;
using Hai.Just1Int7Toggles.Scripts.Components;
using Hai.Just1Int7Toggles.Scripts.Editor.Internal.Reused;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static Hai.Just1Int7Toggles.Scripts.Editor.Internal.J1I7TParameters;

namespace Hai.Just1Int7Toggles.Scripts.Editor.Internal
{
    internal class ViewCreator
    {
        private readonly AnimatorGenerator _animatorGenerator;
        private readonly TogglesManifest _manifest;

        internal ViewCreator(AnimatorGenerator animatorGenerator, TogglesManifest manifest)
        {
            _animatorGenerator = animatorGenerator;
            _manifest = manifest;
        }

        internal void CreateOrReplaceView()
        {
            var machinist = _animatorGenerator.CreateOrRemakeLayerAtSameIndex("Hai_J1I7T_View", 1f)
                .WithEntryPosition(0, -3)
                .WithExitPosition(0, -5);
            var init = machinist.NewState("Init", 1, -2)
                .Drives(InitializedParameterist, false);
            var waiting = machinist.NewState("Waiting", 0, 0)
                .Drives(InitializedParameterist, true);

            var assetContainer_Base = new AnimatorController();
            var assetContainer = new AssetContainerist(assetContainer_Base)
                .GenerateAssetFileIn("", "GeneratedJ1I7T__", "");

            int height = 0;
            Statist previousOn = init;
            Statist previousOff = null;
            foreach (var group in _manifest.Groups)
            {
                var groupParam = new BoolParameterist(group.parameterName);
                var internalParam = new BoolParameterist("JHTIS_T_" + group.parameterName);
                var itemName = group.parameterName;

                _animatorGenerator.CreateParamsAsNeeded(groupParam, internalParam);

                if (group.togglables.Count != 0)
                {
                    Dictionary<string, ToggleableInitialStateV2> subgroup = group.togglables
                        .Where(togglable => togglable.item != null)
                        .GroupBy(togglable => ResolveRelativePath(_manifest.Avatar.transform, togglable.item.transform))
                        .ToDictionary(items => items.Key, items => items.First().initialState);

                    if (subgroup.Count != 0)
                    {
                        var clipForOn = CreateClipToEnable(itemName, subgroup);
                        var clipForOff = CreateClipToDisable(itemName, subgroup);

                        var off = machinist.NewState($"{itemName} OFF", 2, height)
                            .WithAnimation(clipForOff.Expose())
                            .Drives(internalParam, false);
                        var on = machinist.NewState($"{itemName} ON", 3, height)
                            .WithAnimation(clipForOn.Expose())
                            .Drives(internalParam, true);

                        waiting.TransitionsTo(off).WithExitTimeTo(1f)
                            .When(groupParam).IsFalse()
                            .And(internalParam).IsTrue();
                        waiting.TransitionsTo(on).WithExitTimeTo(1f)
                            .When(groupParam).IsTrue()
                            .And(internalParam).IsFalse();

                        off.TransitionsTo(waiting).WithExitTimeTo(1f)
                            .When(InitializedParameterist).IsTrue();
                        on.TransitionsTo(waiting).WithExitTimeTo(1f)
                            .When(InitializedParameterist).IsTrue();

                        previousOn.TransitionsTo(off).WithExitTimeTo(1f)
                            .When(InitializedParameterist).IsFalse()
                            .And(groupParam).IsFalse();
                        previousOn.TransitionsTo(on).WithExitTimeTo(1f)
                            .When(InitializedParameterist).IsFalse()
                            .And(groupParam).IsTrue();

                        previousOff?.TransitionsTo(off).WithExitTimeTo(1f)
                            .When(InitializedParameterist).IsFalse()
                            .And(groupParam).IsFalse();
                        previousOff?.TransitionsTo(on).WithExitTimeTo(1f)
                            .When(InitializedParameterist).IsFalse()
                            .And(groupParam).IsTrue();

                        previousOff = off;
                        previousOn = on;

                        assetContainer.Include(clipForOn);
                        assetContainer.Include(clipForOff);

                        height++;
                    }
                }
            }

            previousOn.AutomaticallyMovesTo(waiting);
            previousOff?.AutomaticallyMovesTo(waiting);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static Motionist CreateClipToDisable(string itemName, Dictionary<string, ToggleableInitialStateV2> relativePaths)
        {
            var motionist = Motionist.FromScratch()
                .WithName("Disable " + itemName)
                .NonLooping();
            foreach (var path in relativePaths)
            {
                switch (path.Value)
                {
                    case ToggleableInitialStateV2.Normal:
                        motionist.TogglesGameObjectOff(path.Key);
                        break;
                    case ToggleableInitialStateV2.Inverse:
                        motionist.TogglesGameObjectOn(path.Key);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return motionist;
        }

        private static Motionist CreateClipToEnable(string itemName, Dictionary<string, ToggleableInitialStateV2> relativePaths)
        {
            var motionist = Motionist.FromScratch()
                .WithName("Enable " + itemName)
                .NonLooping();

            foreach (var path in relativePaths)
            {
                switch (path.Value)
                {
                    case ToggleableInitialStateV2.Normal:
                        motionist.TogglesGameObjectOn(path.Key);
                        break;
                    case ToggleableInitialStateV2.Inverse:
                        motionist.TogglesGameObjectOff(path.Key);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return motionist;
        }

        private static string ResolveRelativePath(Transform avatar, Transform item)
        {
            if (item.parent != avatar && item.parent != null)
            {
                return ResolveRelativePath(avatar, item.parent) + "/" + item.name;
            }

            return item.name;
        }
    }
}
