using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Hai.Just1Int7Toggles.Scripts.Components
{
    public class Just1Int7TogglesCompiler : MonoBehaviour
    {
        public RuntimeAnimatorController animatorController;
        public AnimationClip customEmptyClip;
        
        public VRCAvatarDescriptor avatar;

        public int internalVersion;
        
        public TogglableContainer[] togglables;
        public bool[] enableds;

        public GameObject item1;
        public bool enabled1;
        
        public GameObject item2;
        public bool enabled2;
        
        public GameObject item3;
        public bool enabled3;
        
        public GameObject item4;
        public bool enabled4;
        
        public GameObject item5;
        public bool enabled5;
        
        public GameObject item6;
        public bool enabled6;
        
        public GameObject item7;
        public bool enabled7;

        public bool alsoGenerateLayerB;
        
        public GameObject item8;
        public bool enabled8;
        
        public GameObject item9;
        public bool enabled9;
        
        public GameObject item10;
        public bool enabled10;
        
        public GameObject item11;
        public bool enabled11;
        
        public GameObject item12;
        public bool enabled12;
        
        public GameObject item13;
        public bool enabled13;
        
        public GameObject item14;
        public bool enabled14;
        
        public GameObject item15;
        public bool enabled15;

        public void Migrate()
        {
            if (internalVersion != 0)
            {
                return;
            }
            
            togglables = Enumerable.Repeat(0, 15).Select(i => new TogglableContainer { values = new List<TogglableItem>() }).ToArray();
            enableds = Enumerable.Repeat(0, 15).Select(i => false).ToArray();
                
            var prevItems = new[]
            {
                item1, item2, item3, item4, item5, item6,
                item7, item8, item9, item10, item11,
                item12, item13, item14, item15
            };
            for (var index = 0; index < prevItems.Length; index++)
            {
                var item = prevItems[index];
                if (item != null)
                {
                    togglables[index].values.Add(new TogglableItem
                        {item = item, initialState = J1I7TToggleableInitialState.Normal});
                }
            }
                
            var prevEnableds = new[]
            {
                enabled1, enabled2, enabled3, enabled4, enabled5, enabled6,
                enabled7, enabled8, enabled9, enabled10, enabled11,
                enabled12, enabled13, enabled14, enabled15
            };
            for (var index = 0; index < prevEnableds.Length; index++)
            {
                var item = prevEnableds[index];
                enableds[index] = item;
            }

            internalVersion = 1;
        }
    }
    
    [System.Serializable]
    public struct TogglableContainer
    {
        public List<TogglableItem> values;
    }
    
    [System.Serializable]
    public struct TogglableItem
    {
        public GameObject item;
        public J1I7TToggleableInitialState initialState;
    }

    [System.Serializable]
    public enum J1I7TToggleableInitialState
    {
        Normal, Inverse
    }
}
