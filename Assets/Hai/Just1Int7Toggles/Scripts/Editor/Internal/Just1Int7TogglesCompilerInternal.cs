using System;
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
            CreateOrReplaceController();
            CreateOrReplaceTransmitter(BitLayer.A);
            if (_alsoGenerateLayerB)
            {
                CreateOrReplaceTransmitter(BitLayer.B);
            }
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

        private void CreateOrReplaceController()
        {
            var machinist = _animatorGenerator.CreateOrRemakeLayerAtSameIndex("Hai_J1I7T_Controller", 0f)
                .WithEntryPosition(1, 1);
            
            var init = machinist.NewState("Init", 0, 0);
            var ignore = machinist.NewState("Ignore", 0, -5);
            var waiting = machinist.NewState("Waiting", 0, 1);
            var signal = machinist.NewState("Signal", 1, 3);
            var dirtyCheckForLayerA = machinist.NewState("DirtyCheckLayerA", 0, 9)
                .Drives(DirtyCheckParameterist, 1);

            init.TransitionsTo(ignore).Whenever(ItIsRemote());
            init.TransitionsTo(waiting).Whenever(ItIsLocal());
            waiting.TransitionsTo(signal).When(MainParameterist).IsLesserThan(LayerASignalThreshold);
            signal.TransitionsTo(dirtyCheckForLayerA).When(MainParameterist).IsEqualTo(0);
            dirtyCheckForLayerA.TransitionsTo(waiting).When(DirtyCheckParameterist).IsEqualTo(0);

            CreateInverters(BitLayer.A, machinist, dirtyCheckForLayerA, signal);
            
            if (_alsoGenerateLayerB) {
                var dirtyCheckForLayerB = machinist.NewState("DirtyCheckLayerB", 4, 9)
                    .Drives(DirtyCheckOfBParameterist, 1)
                    .Drives(DirtyCheckParameterist, 1); // need to dirty check the main animator in order to refresh the menu value

                
                dirtyCheckForLayerB.TransitionsTo(waiting)
                    .When(DirtyCheckOfBParameterist).IsEqualTo(0)
                    .And(DirtyCheckParameterist).IsEqualTo(0); // need to dirty check the main animator in order to refresh the menu value
                
                CreateInverters(BitLayer.B, machinist, dirtyCheckForLayerB, signal);
            }
        }

        private static void CreateInverters(BitLayer layer, Machinist machinist, Statist dirtyCheck, Statist signal)
        {
            var exponentLimit = layer == BitLayer.A ? ExponentCountForLayerA : 8;
            var firstItemNumber = layer == BitLayer.A ? 1 : ((int)layer * 8);
            for (var exponent = 0; exponent < exponentLimit; exponent++)
            {
                var itemNumber = firstItemNumber + exponent;

                var enable = machinist.NewState("Enable item #" + itemNumber, 1 + itemNumber, 1)
                    .Drives(BitAsInt(layer, exponent), 1)
                    .AutomaticallyMovesTo(dirtyCheck);
                var disable = machinist.NewState("Disable item #" + itemNumber, 1 + itemNumber, 5)
                    .Drives(BitAsInt(layer, exponent), 0)
                    .AutomaticallyMovesTo(dirtyCheck);

                signal.TransitionsTo(enable)
                    .When(MainParameterist).IsEqualTo(itemNumber)
                    .And(BitAsInt(layer, exponent)).IsEqualTo(0);
                signal.TransitionsTo(disable)
                    .When(MainParameterist).IsEqualTo(itemNumber)
                    .And(BitAsInt(layer, exponent)).IsEqualTo(1);
            }
        }

        private void CreateOrReplaceTransmitter(BitLayer layer)
        {
            var machinist = _animatorGenerator.CreateOrRemakeLayerAtSameIndex("Hai_J1I7T_Transmit" + ToName(layer), 0f)
                .WithEntryPosition(0, 1)
                .WithExitPosition(0, -3);

            var init = machinist.NewState("Init", 0, 0);

            GenerateBranchAt(layer, machinist, layer == BitLayer.A ? 6 : 7, 0, 0, layer == BitLayer.A ? LayerASignalThreshold : 0, init, 0);
        }

        private static void GenerateBranchAt(BitLayer layer, Machinist machinist, int exponent, float x, float y, int currentNumber,
            Statist previousState, int depth)
        {
            var weight = (int)Math.Pow(2, exponent);
            var isDepthEven = depth % 2 == 0;
            var horizontal = isDepthEven ? Math.Max(exponent, 1.25f) + (depth < 2 ? 2 : 0) : 0;
            var vertical = !isDepthEven ? Math.Max(exponent, 1.25f) + (depth < 2 ? 2 : 0) : 0;
            var currentThreshold = currentNumber + weight;
            
            var low = machinist.NewState(exponent + "_LT" + currentThreshold, x - horizontal, y - vertical);
            var high = machinist.NewState(exponent + "_GE" + currentThreshold, x + horizontal, y + vertical);

            previousState.TransitionsTo(low).Whenever(ItIsLocal()).And(BitAsInt(layer, exponent)).IsEqualTo(0);
            previousState.TransitionsTo(low).Whenever(ItIsRemote()).And(layer == BitLayer.A ? MainParameterist : SecondaryOfBParameterist).IsLesserThan(currentThreshold);
            
            previousState.TransitionsTo(high).Whenever(ItIsLocal()).And(BitAsInt(layer, exponent)).IsNotEqualTo(0);
            previousState.TransitionsTo(high).Whenever(ItIsRemote()).And(layer == BitLayer.A ? MainParameterist : SecondaryOfBParameterist).IsGreaterThan(currentThreshold - 1);

            if (exponent > 0)
            {
                GenerateBranchAt(layer, machinist, exponent - 1, x - horizontal, y - vertical, currentNumber, low, depth + 1);
                GenerateBranchAt(layer, machinist, exponent - 1, x + horizontal, y + vertical, currentThreshold, high, depth + 1);
            }
            else
            {
                GenerateLeafBehaviors(layer, low, currentNumber);
                GenerateLeafBehaviors(layer, high, currentThreshold);
                
                if (layer == BitLayer.A) {
                    GenerateMainLeafExitTransitions(low, currentNumber);
                    GenerateMainLeafExitTransitions(high, currentThreshold);
                }
                else
                {
                    GenerateLeafExitTransitions(low, currentNumber);
                    GenerateLeafExitTransitions(high, currentThreshold);
                }
            }
        }

        private static void GenerateMainLeafExitTransitions(Statist leaf, int encodedNumber)
        {
            leaf.Exits().Whenever(ItIsLocal()).And(DirtyCheckParameterist).IsNotEqualTo(0);
            leaf.Exits().Whenever(ItIsRemote())
                .And(MainParameterist).IsNotEqualTo(encodedNumber)
                .And(MainParameterist).IsGreaterThan(LayerASignalThreshold - 1); // Ignore local "impulse" syncs
        }

        private static void GenerateLeafExitTransitions(Statist leaf, int encodedNumber)
        {
            leaf.Exits().Whenever(ItIsLocal()).And(DirtyCheckOfBParameterist).IsNotEqualTo(0);
            leaf.Exits().Whenever(ItIsRemote())
                .And(SecondaryOfBParameterist).IsNotEqualTo(encodedNumber);
        }

        private static void GenerateLeafBehaviors(BitLayer layer, Statist leaf, int encodedNumber)
        {
            var count = layer == BitLayer.A ? ExponentCountForLayerA : 8;
            for (var exponent = 0; exponent < count; exponent++)
            {
                var bitValue = (encodedNumber >> exponent) & 1;
                leaf.Drives(BitAsInt(layer, exponent), bitValue);
                leaf.Drives(BitAsFloat(layer, exponent), bitValue);
            }
            
            leaf.Drives(layer == BitLayer.A ? MainParameterist : SecondaryOfBParameterist, encodedNumber);
            leaf.Drives(layer == BitLayer.A ? DirtyCheckParameterist : DirtyCheckOfBParameterist, 0);
        }
    }
}