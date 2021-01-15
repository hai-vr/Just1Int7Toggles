using Hai.Just1Int7Toggles.Scripts.Editor.Internal.Reused;
using UnityEditor.Animations;
using UnityEngine;
using static Hai.Just1Int7Toggles.Scripts.Editor.Internal.J1I7TParameters;

namespace Hai.Just1Int7Toggles.Scripts.Editor.Internal
{
    public class Just1Int7TogglesCompilerInternal
    {
        private static readonly string EmptyClipPath = "Assets/Hai/Just1Int7Toggles/Hai_Just1Int7Toggles_EmptyClip.anim";
        private static readonly string PluginFolderName = "Just1Int7Toggles";

        private readonly AnimatorGenerator _animatorGenerator;
        private readonly StatefulEmptyClipProvider _emptyClipProvider;
        private readonly ViewCreator _viewCreator;
        private readonly CasterCreator _casterCreator;

        public Just1Int7TogglesCompilerInternal(RuntimeAnimatorController animatorController,
            AnimationClip customEmptyClip, TogglesManifest manifest)
        {
            _emptyClipProvider = new StatefulEmptyClipProvider(new ClipGenerator(customEmptyClip, EmptyClipPath, PluginFolderName));
            _animatorGenerator = new AnimatorGenerator((AnimatorController) animatorController, _emptyClipProvider);
            _viewCreator = new ViewCreator(_animatorGenerator, manifest);
            _casterCreator = new CasterCreator(_animatorGenerator, manifest);
        }

        public void DoOverwriteCommonLogic()
        {
            _emptyClipProvider.Get();

            RemoveLegacyParameters();
            CreateParameters();
            _animatorGenerator.RemoveLayerIfExists("Hai_J1I7T_Controller");
            _animatorGenerator.RemoveLayerIfExists("Hai_J1I7T_TransmitA");
            _animatorGenerator.RemoveLayerIfExists("Hai_J1I7T_TransmitB");
            CreateOrReplaceCaster();
            CreateOrReplaceView();
        }

        private void RemoveLegacyParameters()
        {
            _animatorGenerator.RemoveParamsIndiscriminatelyAsNeeded(
                "J1I7T_A_Sync", "J1I7T_B_Sync",
                "J1I7T_A_0", "J1I7T_A_1", "J1I7T_A_2", "J1I7T_A_3", "J1I7T_A_4", "J1I7T_A_5", "J1I7T_A_6",
                "J1I7T_B_0", "J1I7T_B_1", "J1I7T_B_2", "J1I7T_B_3", "J1I7T_B_4", "J1I7T_B_5", "J1I7T_B_6", "J1I7T_B_7",
                "J1I7T_A_0F", "J1I7T_A_1F", "J1I7T_A_2F", "J1I7T_A_3F", "J1I7T_A_4F", "J1I7T_A_5F", "J1I7T_A_6F",
                "J1I7T_B_0F", "J1I7T_B_1F", "J1I7T_B_2F", "J1I7T_B_3F", "J1I7T_B_4F", "J1I7T_B_5F", "J1I7T_B_6F", "J1I7T_B_7F",
                "J1I7T_A_DirtyCheck", "J1I7T_B_DirtyCheck",
                "J1I7T_Internal_One"
            );
        }

        private void CreateOrReplaceCaster()
        {
            _casterCreator.CreateOrReplaceCaster();
        }

        private void CreateOrReplaceView()
        {
            _viewCreator.CreateOrReplaceView();
        }

        private void CreateParameters()
        {
            _animatorGenerator.CreateParamsAsNeeded(AlwaysOneParameterist);
        }
    }
}
