using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hai.Just1Int7Toggles.Scripts.Components
{
    public class JustHaiToggleGroup : MonoBehaviour
    {
        public int internalVersion;

        public List<TogglableItemV2> togglables;
        public bool hintEnabled;
        public string parameterName;
    }

    [Serializable]
    public struct TogglableItemV2
    {
        public GameObject item;
        public ToggleableInitialStateV2 initialState;
    }

    [Serializable]
    public enum ToggleableInitialStateV2
    {
        Normal,
        Inverse
    }
}
