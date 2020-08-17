using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Hai.Just1Int7Toggles.Scripts.Components
{
    public class J2I7IGroupOfOutfits : MonoBehaviour
    {
        public string name;
        public Texture2D icon;

        public List<J2I7IOutfit> outfits;
        public int indexDefaultOn;

        public VRCExpressionsMenu menu0to7;
        public VRCExpressionsMenu menu7to15;

        public int? BitCount()
        {
            if (outfits.Count == 0) return 0;
            if (outfits.Count == 1) return 1;
            if (outfits.Count == 2) return 1;
            if (outfits.Count <= 4) return 2;
            if (outfits.Count <= 8) return 3;
            if (outfits.Count <= 16) return 4;
            return null;
        }
    }

    [Serializable]
    public struct J2I7IOutfit
    {
        public string name;
        public Texture2D icon;

        public List<GameObject> items;
    }

    [Serializable]
    public enum OutfitLayer
    {
        MainLayer,
        SecondaryLayerB
    }

}
