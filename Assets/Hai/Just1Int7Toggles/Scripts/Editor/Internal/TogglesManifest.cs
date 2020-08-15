using System;
using UnityEngine;

namespace Hai.Just1Int7Toggles.Scripts.Editor.Internal
{
    public readonly struct TogglesManifest
    {
        public TogglesManifest(GameObject avatar,
        ToggleEntry entry1,
        ToggleEntry entry2,
        ToggleEntry entry3,
        ToggleEntry entry4,
        ToggleEntry entry5,
        ToggleEntry entry6,
        ToggleEntry entry7,
        ToggleEntry entry8,
        ToggleEntry entry9,
        ToggleEntry entry10,
        ToggleEntry entry11,
        ToggleEntry entry12,
        ToggleEntry entry13,
        ToggleEntry entry14,
        ToggleEntry entry15
        ) {
            Avatar = avatar;
            Entry1 = entry1;
            Entry2 = entry2;
            Entry3 = entry3;
            Entry4 = entry4;
            Entry5 = entry5;
            Entry6 = entry6;
            Entry7 = entry7;
            Entry8 = entry8;
            Entry9 = entry9;
            Entry10 = entry10;
            Entry11 = entry11;
            Entry12 = entry12;
            Entry13 = entry13;
            Entry14 = entry14;
            Entry15 = entry15;
        }

        public ToggleEntry GetEntry(int itemNumber)
        {
            switch (itemNumber)
            {
                case 1: return Entry1;
                case 2: return Entry2;
                case 3: return Entry3;
                case 4: return Entry4;
                case 5: return Entry5;
                case 6: return Entry6;
                case 7: return Entry7;
                case 8: return Entry8;
                case 9: return Entry9;
                case 10: return Entry10;
                case 11: return Entry11;
                case 12: return Entry12;
                case 13: return Entry13;
                case 14: return Entry14;
                case 15: return Entry15;
            }

            throw new ArgumentOutOfRangeException();
        }

        public GameObject Avatar { get; }
        private ToggleEntry Entry1 { get; }
        private ToggleEntry Entry2 { get; }
        private ToggleEntry Entry3 { get; }
        private ToggleEntry Entry4 { get; }
        private ToggleEntry Entry5 { get; }
        private ToggleEntry Entry6 { get; }
        private ToggleEntry Entry7 { get; }
        private ToggleEntry Entry8 { get; }
        private ToggleEntry Entry9 { get; }
        private ToggleEntry Entry10 { get; }
        private ToggleEntry Entry11 { get; }
        private ToggleEntry Entry12 { get; }
        private ToggleEntry Entry13 { get; }
        private ToggleEntry Entry14 { get; }
        private ToggleEntry Entry15 { get; }
    }
    
    public readonly struct ToggleEntry
    {
        public ToggleEntry(GameObject item, bool enabled)
        {
            Item = item;
            Enabled = enabled;
        }

        public GameObject Item { get; }
        public bool Enabled { get; }
    }
}