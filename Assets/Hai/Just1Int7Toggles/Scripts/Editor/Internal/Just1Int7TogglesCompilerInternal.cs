using Hai.Just1Int7Toggles.Scripts.Editor.Internal.Reused;
using UnityEditor.Animations;
using UnityEngine;
using static Hai.Just1Int7Toggles.Scripts.Editor.Internal.J1I7TParameters;
using static Hai.Just1Int7Toggles.Scripts.Editor.Internal.Reused.AV3Parameterists;

namespace Hai.Just1Int7Toggles.Scripts.Editor.Internal
{
    public class Just1Int7TogglesCompilerInternal
    {
        private readonly bool _alsoGenerateLayerB;
        private static readonly string EmptyClipPath = "Assets/Hai/Just1Int7Toggles/Hai_Just1Int7Toggles_EmptyClip.anim";
        private static readonly string PluginFolderName = "Just1Int7Toggles";
        internal static readonly int ExponentCountForLayerA = 7;
        internal static readonly int LayerASignalThreshold = 128;

        private readonly AnimatorGenerator _animatorGenerator;
        private readonly StatefulEmptyClipProvider _emptyClipProvider;
        private readonly ViewCreator _viewCreator;

        public Just1Int7TogglesCompilerInternal(RuntimeAnimatorController animatorController,
            AnimationClip customEmptyClip, TogglesManifest manifest, bool alsoGenerateLayerB)
        {
            _alsoGenerateLayerB = alsoGenerateLayerB;
            _emptyClipProvider = new StatefulEmptyClipProvider(new ClipGenerator(customEmptyClip, EmptyClipPath, PluginFolderName));
            _animatorGenerator = new AnimatorGenerator((AnimatorController) animatorController, _emptyClipProvider);
            _viewCreator = new ViewCreator(_animatorGenerator, manifest, alsoGenerateLayerB);
        }

        public void DoOverwriteCommonLogic()
        {
            _emptyClipProvider.Get();

            CreateParameters();
            _animatorGenerator.RemoveLayerIfExists("Hai_J1I7T_Controller");
            _animatorGenerator.RemoveLayerIfExists("Hai_J1I7T_TransmitA");
            _animatorGenerator.RemoveLayerIfExists("Hai_J1I7T_TransmitB");
            CreateOrReplaceView();
        }

        private void CreateOrReplaceView()
        {
            _viewCreator.CreateOrReplaceView();
        }

        private void CreateParameters()
        {
            _animatorGenerator.CreateParamsAsNeeded(IsLocal, MainParameterist, DirtyCheckParameterist, AlwaysOneParameterist);
            for (var exponent = 0; exponent < ExponentCountForLayerA; exponent++)
            {
                _animatorGenerator.CreateParamsAsNeeded(BitAsInt(BitLayer.A, exponent), BitAsFloat(BitLayer.A, exponent));
            }

            if (_alsoGenerateLayerB) {
                _animatorGenerator.CreateParamsAsNeeded(SecondaryOfBParameterist, DirtyCheckOfBParameterist);
                for (var exponent = 0; exponent < 8; exponent++)
                {
                    _animatorGenerator.CreateParamsAsNeeded(BitAsInt(BitLayer.B, exponent), BitAsFloat(BitLayer.B, exponent));
                }
            }
        }
    }
}
