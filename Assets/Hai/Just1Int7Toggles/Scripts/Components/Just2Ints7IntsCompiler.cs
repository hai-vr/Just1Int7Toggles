using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Hai.Just1Int7Toggles.Scripts.Components
{
    public class Just2Ints7IntsCompiler : MonoBehaviour
    {
        public RuntimeAnimatorController animatorController;
        public AnimationClip customEmptyClip;

        public VRCAvatarDescriptor avatar;

        public List<GroupOfOutfitsContainer> groupOfOutfits;

        public int CountBitOccupationOf(OutfitLayer wantedLayer)
        {
            return groupOfOutfits
                .Where(container => container.layer == wantedLayer && container.value != null && container.value.BitCount() != null)
                .Select(container => (int) container.value.BitCount())
                .Sum();
        }
    }

    [Serializable]
    public struct GroupOfOutfitsContainer
    {
        public J2I7IGroupOfOutfits value;
        public int anchorValue;
        public bool anchorLocked;
        public OutfitLayer layer;
    }
}
