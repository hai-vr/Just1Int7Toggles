﻿using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Hai.Just1Int7Toggles.Scripts.Editor.Internal.Reused
{
    internal class AnimatorGenerator
    {
        private readonly AnimatorController _animatorController;
        private readonly StatefulEmptyClipProvider _emptyClipProvider;

        internal AnimatorGenerator(AnimatorController animatorController, StatefulEmptyClipProvider emptyClipProvider)
        {
            _animatorController = animatorController;
            _emptyClipProvider = emptyClipProvider;
        }

        internal void CreateParamsAsNeeded(params Parameterist[] parameterists)
        {
            foreach (var parameterist in parameterists)
            {
                switch (parameterist)
                {
                    case IntParameterist p:
                        CreateParamIfNotExists(parameterist.Name, AnimatorControllerParameterType.Int);
                        break;
                    case FloatParameterist p:
                        CreateParamIfNotExists(parameterist.Name, AnimatorControllerParameterType.Float);
                        break;
                    case BoolParameterist p:
                        CreateParamIfNotExists(parameterist.Name, AnimatorControllerParameterType.Bool);
                        break;
                }
            }
        }

        private void CreateParamIfNotExists(string paramName, AnimatorControllerParameterType type)
        {
            if (_animatorController.parameters.FirstOrDefault(param => param.name == paramName) == null)
            {
                _animatorController.AddParameter(paramName, type);
            }
        }

        internal Machinist CreateOrRemakeLayerAtSameIndex(string layerName, float weightWhenCreating)
        {
            var originalIndexToPreserveOrdering = _animatorController.layers.ToList().FindIndex(layer1 => layer1.name == layerName);
            if (originalIndexToPreserveOrdering != -1)
            {
                _animatorController.RemoveLayer(originalIndexToPreserveOrdering);
            }
            
            AddLayerWithWeight(layerName, weightWhenCreating);
            if (originalIndexToPreserveOrdering != -1)
            {
                var items = _animatorController.layers.ToList();
                var last = items[items.Count - 1];
                items.RemoveAt(items.Count - 1);
                items.Insert(originalIndexToPreserveOrdering, last);
                _animatorController.layers = items.ToArray();
            }

            var layer = TryGetLayer(layerName);
            var machinist = new Machinist(layer.stateMachine, _emptyClipProvider.Get());
            return machinist
                .WithAnyStatePosition(0, 7)
                .WithEntryPosition(0, -1)
                .WithExitPosition(7, -1);
        }

        private AnimatorControllerLayer TryGetLayer(string layerName)
        {
            return _animatorController.layers.FirstOrDefault(it => it.name == layerName);
        }

        private void AddLayerWithWeight(string layerName, float weightWhenCreating)
        {
            // This function is a replication of AnimatorController::AddLayer(string) behavior, in order to change the weight.
            // For some reason I cannot find how to change the layer weight after it has been created.
        
            var newLayer = new AnimatorControllerLayer();
            newLayer.name = _animatorController.MakeUniqueLayerName(layerName);
            newLayer.stateMachine = new AnimatorStateMachine();
            newLayer.stateMachine.name = newLayer.name;
            newLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
            newLayer.defaultWeight = weightWhenCreating;
            if (AssetDatabase.GetAssetPath(_animatorController) != "")
            {
                AssetDatabase.AddObjectToAsset(newLayer.stateMachine,
                    AssetDatabase.GetAssetPath(_animatorController));
            }

            _animatorController.AddLayer(newLayer);
        }

        internal static Vector3 GridPosition(float x, float y)
        {
            return new Vector3(x * 200 , y * 70, 0);
        }
    }
}