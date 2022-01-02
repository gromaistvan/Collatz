using System.Diagnostics;
using System.Numerics;
using JetBrains.Annotations;
using static System.Console;

namespace MrgInfo.Math.Collatz.Shell
{
    sealed class EasyConsoleWriter: IWriter
    {
        const int _batchSize = 100_000;

        [NotNull]
        readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        BigInteger _line;

        public void Print(SequenceRecord record)
        { }

        public void Print(CollatzSequence sequence)
        {
            if (++_line % _batchSize > 0) return;
            WriteLine($"Numbers: {_line,11:###,###,##0} | Time: {_stopwatch.Elapsed:hh\\:mm\\:ss} | Memory: {GC.GetTotalMemory(),9} | Max: {sequence?.Max,19:###,###,###,###,##0} | Size: {sequence?.Size,11:###,###,##0}");
            Beep();
            _stopwatch.Restart();
        }

        public bool Active =>
            IsInputRedirected
            || ! KeyAvailable;
    }
}
