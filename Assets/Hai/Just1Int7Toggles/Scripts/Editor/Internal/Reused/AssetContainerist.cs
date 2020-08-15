using System;
using UnityEditor;
using UnityEditor.Animations;

namespace Hai.Just1Int7Toggles.Scripts.Editor.Internal.Reused
{
    internal class AssetContainerist
    {
        private readonly AnimatorController _assetContainer;
        private string _datetimeForAssetPack;

        public AssetContainerist(AnimatorController assetContainer)
        {
            _assetContainer = assetContainer;
            _datetimeForAssetPack = DateTime.Now.ToString("yyyy'-'MM'-'dd'_'HHmmss");
        }

        public AssetContainerist GenerateAssetFileIn(string pathWithNoLeadingNorTrailingSlash, string prefix, string suffix)
        {
            string separator = "";
            if (pathWithNoLeadingNorTrailingSlash.Length != 0)
            {
                separator = "/";
            }
            AssetDatabase.CreateAsset(_assetContainer, "Assets/" + pathWithNoLeadingNorTrailingSlash + separator + prefix + _datetimeForAssetPack + suffix + ".asset");

            return this;
        }

        public void Include(Assetist assetist)
        {
            AssetDatabase.AddObjectToAsset(assetist.AsAsset(), _assetContainer);
        }
    }
}