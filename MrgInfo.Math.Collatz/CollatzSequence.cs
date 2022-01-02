using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;
using static System.Numerics.BigInteger;

namespace MrgInfo.Math.Collatz
{
    /// <summary>
    ///     Calculate Collatz's sequence.
    /// </summary>
    public sealed class CollatzSequence
    {
        [NotNull]
        ConcurrentDictionary<BigInteger, SequenceRecord> Sequences { get; } = new ConcurrentDictionary<BigInteger, SequenceRecord>
        {
            [One] = new SequenceRecord(One)
        };

        /// <summary>
        ///     Number of reached integers.
        /// </summary>
        public BigInteger Size => Sequences.Count;

        /// <summary>
        ///     Biggest integer reached.
        /// </summary>
        public BigInteger Max { get; private set; }

        /// <summary>
        ///     Looks for collatz sequence of a given integer (no calculation).
        /// </summary>
        /// <param name="number">
        ///     Some positive integer.
        /// </param>
        /// <returns>
        ///     Collatz sequence data if achievable, otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Negative integer for <paramref name="number"/>.
        /// </exception>
        public SequenceRecord Find(in BigInteger number)
        {
            if (number.Sign <= 0) throw new ArgumentOutOfRangeException(nameof(number), $"{nameof(number)} <= 0");

            return Sequences.TryGetValue(number, out SequenceRecord result) ? result : null;
        }

        /// <summary>
        ///     Calculates collatz data for a given integer.
        /// </summary>
        /// <param name="number">
        ///     Some positive integer.
        /// </param>
        /// <returns>
        ///     Collatz sequence data.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Negative integer for <paramref name="number"/>.
        /// </exception>
        public SequenceRecord Get(BigInteger number)
        {
            if (number.Sign <= 0) throw new ArgumentOutOfRangeException(nameof(number), $"{nameof(number)} <= 0");

            var records = new Stack<SequenceRecord>();
            SequenceRecord current;
            while (! Sequences.TryGetValue(number, out current))
            {
                records.Push(new SequenceRecord(number));
                number = number.IsEven
                    ? number >> 1
                    : (number << 1) + number + 1;
            }
            while (records.Count > 0)
            {
                SequenceRecord top = records.Pop() ?? throw new InvalidOperationException(nameof(Stack.Pop));
                top.Next = current;
                top.Steps = current.Steps + 1;
                top.Max = Max(top.Number, current.Max);
                current = Sequences.GetOrAdd(top.Number, top) ?? throw new InvalidOperationException(nameof(ConcurrentDictionary<BigInteger, SequenceRecord>.GetOrAdd));
                if (current.Number <= Max) continue;
                lock (Sequences)
                {
                    if (current.Number > Max)
                    {
                        Max = current.Number;
                    }
                }
            }
            return current;
        }
    }
}
