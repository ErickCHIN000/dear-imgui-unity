using System;
using System.Collections.Generic;

namespace ImGuiNET.BepInEx
{
    public sealed class IntPtrEqualityComparer : IEqualityComparer<IntPtr>
    {
        public static readonly IntPtrEqualityComparer Instance = new IntPtrEqualityComparer();

        public bool Equals(IntPtr x, IntPtr y) => x == y;
        public int GetHashCode(IntPtr obj) => obj.GetHashCode();
    }
}