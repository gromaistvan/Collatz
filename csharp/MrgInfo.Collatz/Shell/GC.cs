using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace MrgInfo.Math.Collatz.Shell
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    static class GC
    {
        [NotNull]
        static readonly string[] _units = { "B ", "kB", "MB", "GB", "TB", "PB" };

        public static string GetTotalMemory()
        {
            float memory = System.GC.GetTotalMemory(false);
            foreach (string unit in _units)
            {
                if (memory < 1024f) return $"{memory:0.00}{unit}";
                memory /= 1024f;
            }
            return $"{memory:0.00}{_units.LastOrDefault()}";
        }
    }
}
