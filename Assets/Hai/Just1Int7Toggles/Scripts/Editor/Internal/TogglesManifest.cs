using Hai.Just1Int7Toggles.Scripts.Components;
using UnityEngine;

namespace Hai.Just1Int7Toggles.Scripts.Editor.Internal
{
    public readonly struct TogglesManifest
    {
        public JustHaiToggleGroup[] Groups { get; }
        public GameObject Avatar { get; }

        public TogglesManifest(GameObject avatar, JustHaiToggleGroup[] groups)
        {
            Groups = groups;
            Avatar = avatar;
        }
    }
}
