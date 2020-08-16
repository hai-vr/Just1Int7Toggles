using Hai.Just1Int7Toggles.Scripts.Components;
using UnityEngine;

namespace Hai.Just1Int7Toggles.Scripts.Editor.Internal
{
    public readonly struct TogglesManifest
    {
        private readonly TogglableContainer[] _togglables;
        private readonly bool[] _enableds;

        public TogglesManifest(GameObject avatar, TogglableContainer[] togglables, bool[] enableds)
        {
            _togglables = togglables;
            _enableds = enableds;
            Avatar = avatar;
        }

        public ToggleEntry GetEntry(int itemNumber)
        {
            var index = itemNumber - 1;
            return new ToggleEntry(_togglables[index].values.ToArray(), _enableds[index]);
        }

        public GameObject Avatar { get; }
    }
    
    public readonly struct ToggleEntry
    {
        public ToggleEntry(TogglableItem[] items, bool enabled)
        {
            Items = items;
            Enabled = enabled;
        }

        public TogglableItem[] Items { get; }
        public bool Enabled { get; }
    }
}