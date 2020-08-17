using System;
using System.Collections.Generic;
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
    }

    [Serializable]
    public struct GroupOfOutfitsContainer
    {
        public J2I7IGroupOfOutfits value;
    }
}
