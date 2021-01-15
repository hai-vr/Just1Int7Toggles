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
            var init = machinist.NewState("Init", 0, -2);
            var blend = machinist.NewState("Blend", 0, 0)
                .WithAnimation(CreateBlendTree())
                .WithWriteDefaultsSetTo(true) // FIXME: Why do I have to do this for the system to work?
                .Drives(AlwaysOneParameterist, 1f);

            init.AutomaticallyMovesTo(blend);
        }

        private Motion CreateBlendTree()
        {
            var assetContainer_Base = new AnimatorController();
            var assetContainer = new AssetContainerist(assetContainer_Base)
                .GenerateAssetFileIn("", "GeneratedJ1I7T__", "");

            var childMotions = new List<ChildMotion>();
            foreach (var group in _manifest.Groups)
            {
                GenerateItemTree(group, assetContainer, assetContainer_Base, childMotions);
            }

            var blendTree = new BlendTree
            {
                name = "autoBT_ALL",
                blendType = BlendTreeType.Direct,
                children = childMotions.ToArray()
            };
            AssetDatabase.AddObjectToAsset(blendTree, assetContainer_Base);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return blendTree;
        }

        private void GenerateItemTree(JustHaiToggleGroup container, AssetContainerist assetContainer,
            AnimatorController assetContainerBase,
            List<ChildMotion> childMotions)
        {
            var itemName = container.parameterName;

            if (container.togglables.Count == 0) return;

            Dictionary<string, ToggleableInitialStateV2> group = container.togglables
                .Where(togglable => togglable.item != null)
                .GroupBy(togglable => ResolveRelativePath(_manifest.Avatar.transform, togglable.item.transform))
                .ToDictionary(items => items.Key, items => items.First().initialState);

            if (group.Count == 0) return;

            var clipForOn = CreateClipToEnable(itemName, group);
            var clipForOff = CreateClipToDisable(itemName, group);

            assetContainer.Include(clipForOn);
            assetContainer.Include(clipForOff);

            var subTree = new BlendTree
            {
                name = "autoBT_Item" + itemName,
                blendParameter = container.parameterName + "_F",
                minThreshold = 0,
                maxThreshold = 1,
                blendType = BlendTreeType.Simple1D,
                children = new[]
                {
                    new ChildMotion {motion = clipForOff.Expose(), threshold = 0},
                    new ChildMotion {motion = clipForOn.Expose(), threshold = 1}
                }
            };

            AssetDatabase.AddObjectToAsset(subTree, assetContainerBase);

            childMotions.Add(new ChildMotion
                {motion = subTree, timeScale = 1, directBlendParameter = AlwaysOneParameterist.Name});
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
