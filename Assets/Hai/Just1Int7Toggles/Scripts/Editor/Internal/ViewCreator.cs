using System;
using System.Collections.Generic;
using System.Linq;
using Hai.Just1Int7Toggles.Scripts.Components;
using Hai.Just1Int7Toggles.Scripts.Editor.Internal.Reused;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using static Hai.Just1Int7Toggles.Scripts.Editor.Internal.J1I7TParameters;
using static Hai.Just1Int7Toggles.Scripts.Editor.Internal.Reused.AV3Parameterists;

namespace Hai.Just1Int7Toggles.Scripts.Editor.Internal
{
    internal class ViewCreator
    {
        private readonly AnimatorGenerator _animatorGenerator;
        private readonly TogglesManifest _manifest;
        private readonly bool _alsoGenerateLayerB;

        internal ViewCreator(AnimatorGenerator animatorGenerator, TogglesManifest manifest, bool alsoGenerateLayerB)
        {
            _animatorGenerator = animatorGenerator;
            _manifest = manifest;
            _alsoGenerateLayerB = alsoGenerateLayerB;
        }

        internal void CreateOrReplaceView()
        {
            var machinist = _animatorGenerator.CreateOrRemakeLayerAtSameIndex("Hai_J1I7T_View", 1f)
                .WithEntryPosition(0, -3)
                .WithExitPosition(0, -5);
            var init = machinist.NewState("Init", 0, -2);
            var local = machinist.NewState("Local SetSyncValue", -1, -1);
            var remote = machinist.NewState("Remote DirtyCheck", 1, -1);
            var blend = machinist.NewState("Blend", 0, 0)
                .WithAnimation(CreateBlendTree())
                .WithWriteDefaultsSetTo(true) // FIXME: Why do I have to do this for the system to work?
                .Drives(AlwaysOneParameterist, 1f);
            
            {
                var defaultStageValue = Just1Int7TogglesCompilerInternal.LayerASignalThreshold;
                for (var exponent = 0; exponent < Just1Int7TogglesCompilerInternal.ExponentCountForLayerA; exponent++)
                {
                    var itemNumber = exponent + 1;
                    var enabled = _manifest.GetEntry(itemNumber).Enabled;
                    defaultStageValue += enabled ? (int)Math.Pow(2, exponent) : 0;

                    local.Drives(BitAsInt(BitLayer.A, exponent), enabled ? 1 : 0);
                    local.Drives(BitAsFloat(BitLayer.A, exponent), enabled ? 1f : 0f);
                }
                local.Drives(MainParameterist, defaultStageValue);
                remote.Drives(DirtyCheckParameterist, 1);
            }
            
            if (_alsoGenerateLayerB)
            {
                var defaultStageValue = 0;
                for (var exponent = 0; exponent < 8; exponent++)
                {
                    var itemNumber = exponent + 8;
                    var enabled = _manifest.GetEntry(itemNumber).Enabled;
                    defaultStageValue += enabled ? (int)Math.Pow(2, exponent) : 0;

                    local.Drives(BitAsInt(BitLayer.B, exponent), enabled ? 1 : 0);
                    local.Drives(BitAsFloat(BitLayer.B, exponent), enabled ? 1f : 0f);
                }
                local.Drives(SecondaryOfBParameterist, defaultStageValue);
                remote.Drives(DirtyCheckOfBParameterist, 1);
            }

            init.TransitionsTo(local).Whenever(ItIsLocal());
            init.TransitionsTo(remote).Whenever(ItIsRemote());
            local.AutomaticallyMovesTo(blend);
            remote.AutomaticallyMovesTo(blend);
        }
        
        private Motion CreateBlendTree()
        {
            var assetContainer_Base = new AnimatorController();
            var assetContainer = new AssetContainerist(assetContainer_Base)
                .GenerateAssetFileIn("", "GeneratedJ1I7T__", "");
            
            var childMotions = new List<ChildMotion>();
            for (var exponent = 0; exponent < Just1Int7TogglesCompilerInternal.ExponentCountForLayerA; exponent++)
            {
                GenerateItemTree(BitLayer.A, exponent, assetContainer, assetContainer_Base, childMotions);
            }

            if (_alsoGenerateLayerB)
            {
                for (var exponent = 0; exponent < 8; exponent++)
                {
                    GenerateItemTree(BitLayer.B, exponent, assetContainer, assetContainer_Base, childMotions);
                }
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

        private void GenerateItemTree(BitLayer layer, int exponent, AssetContainerist assetContainer,
            AnimatorController assetContainerBase,
            List<ChildMotion> childMotions)
        {
            var firstItemNumber = layer == BitLayer.A ? 1 : ((int)layer * 8);
            var itemNumber = exponent + firstItemNumber;
            var entry = _manifest.GetEntry(itemNumber);

            if (entry.Items.Length == 0) return;

            Dictionary<string, J1I7TToggleableInitialState> group = entry.Items
                .Where(togglable => togglable.item != null)
                .GroupBy(togglable => ResolveRelativePath(_manifest.Avatar.transform, togglable.item.transform))
                .ToDictionary(items => items.Key, items => items.First().initialState);

            if (group.Count == 0) return;
            
            var clipForOn = CreateClipToEnable(itemNumber, group);
            var clipForOff = CreateClipToDisable(itemNumber, group);

            assetContainer.Include(clipForOn);
            assetContainer.Include(clipForOff);

            var subTree = new BlendTree
            {
                name = "autoBT_Item" + itemNumber,
                blendParameter = BitAsFloat(layer, exponent).Name,
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

        private static Motionist CreateClipToDisable(int itemNumber, Dictionary<string, J1I7TToggleableInitialState> relativePaths)
        {
            var motionist = Motionist.FromScratch()
                .WithName("Disable " + itemNumber)
                .NonLooping();
            foreach (var path in relativePaths)
            {
                switch (path.Value)
                {
                    case J1I7TToggleableInitialState.Normal:
                        motionist.TogglesGameObjectOff(path.Key);
                        break;
                    case J1I7TToggleableInitialState.Inverse:
                        motionist.TogglesGameObjectOn(path.Key);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return motionist;
        }

        private static Motionist CreateClipToEnable(int itemNumber, Dictionary<string, J1I7TToggleableInitialState> relativePaths)
        {
            var motionist = Motionist.FromScratch()
                .WithName("Enable " + itemNumber)
                .NonLooping();
            
            foreach (var path in relativePaths)
            {
                switch (path.Value)
                {
                    case J1I7TToggleableInitialState.Normal:
                        motionist.TogglesGameObjectOn(path.Key);
                        break;
                    case J1I7TToggleableInitialState.Inverse:
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