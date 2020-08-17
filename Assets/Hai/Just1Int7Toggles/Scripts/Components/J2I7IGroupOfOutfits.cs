using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Hai.Just1Int7Toggles.Scripts.Components
{
    public class J2I7IGroupOfOutfits : MonoBehaviour
    {
        public string name;
        public Image picture;
        
        public List<J2I7IOutfit> outfits;
        public int indexDefaultOn;

        public VRCExpressionsMenu menu0to7;
        public VRCExpressionsMenu menu7to15;
    }
    
    [Serializable]
    public struct J2I7IOutfit
    {
        public string name;
        public Image picture;
        
        public List<GameObject> items;
    }
}