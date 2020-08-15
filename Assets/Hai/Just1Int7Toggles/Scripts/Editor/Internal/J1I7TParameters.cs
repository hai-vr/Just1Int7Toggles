using System;
using Hai.Just1Int7Toggles.Scripts.Editor.Internal.Reused;

namespace Hai.Just1Int7Toggles.Scripts.Editor.Internal
{
    internal static class J1I7TParameters
    {
        internal static readonly IntParameterist MainParameterist = new IntParameterist("J1I7T_A_Sync");
        internal static readonly IntParameterist SecondaryOfBParameterist = new IntParameterist("J1I7T_B_Sync");
        internal static readonly IntParameterist DirtyCheckParameterist = new IntParameterist("J1I7T_A_DirtyCheck");
        internal static readonly IntParameterist DirtyCheckOfBParameterist = new IntParameterist("J1I7T_B_DirtyCheck");
        internal static readonly FloatParameterist AlwaysOneParameterist = new FloatParameterist("J1I7T_Internal_One");

        internal static IntParameterist BitAsInt(BitLayer layer, int exponent)
        {
            return new IntParameterist($"J1I7T_{ToName(layer)}_{exponent}");
        }

        internal static FloatParameterist BitAsFloat(BitLayer layer, int exponent)
        {
            return new FloatParameterist($"J1I7T_{ToName(layer)}_{exponent}F");
        }

        internal static string ToName(BitLayer layer)
        {
            switch (layer)
            {
                case BitLayer.A:
                    return "A";
                case BitLayer.B:
                    return "B";
                default:
                    throw new ArgumentOutOfRangeException(nameof(layer), layer, null);
            }
        }

        internal enum BitLayer
        {
            A,
            B
        }
    }
}