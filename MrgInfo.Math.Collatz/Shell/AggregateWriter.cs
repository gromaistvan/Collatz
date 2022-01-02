using System;
using System.Linq;
using JetBrains.Annotations;

namespace MrgInfo.Math.Collatz.Shell
{
    sealed class AggregateWriter: IWriter
    {
        [NotNull]
        readonly IWriter[] _writers;

        public AggregateWriter(params IWriter[] writers) => _writers = writers ?? throw new ArgumentNullException(nameof(writers));

        public void Print(SequenceRecord record) => Array.ForEach(_writers, w => w?.Print(record));

        public void Print(CollatzSequence sequence) => Array.ForEach(_writers, w => w?.Print(sequence));

        public bool Active => _writers.All(w => w?.Active ?? true);
    }
}
