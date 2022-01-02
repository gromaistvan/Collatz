using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;

namespace MrgInfo.Math.Collatz
{
    /// <summary>
    ///     Collatz sequence information to a given number.
    /// </summary>
    /// <inheritdoc />
    public sealed class SequenceRecord: IEnumerable<BigInteger>
    {
        /// <summary>
        ///     A given integer.
        /// </summary>
        public BigInteger Number { get; }

        /// <summary>
        ///     Sequence steps from current (<see cref="Number"/>) to 1.
        /// </summary>
        public BigInteger Steps { get; internal set; }

        /// <summary>
        ///     Maximum integer from current (<see cref="Number"/>) to 1.
        /// </summary>
        public BigInteger Max { get; internal set; }

        /// <summary>
        ///     Next integer in Collatz sequence.
        /// </summary>
        public SequenceRecord Next { get; internal set; }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="number">
        ///     A given number.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Negative integer for <paramref name="number"/>.
        /// </exception>
        internal SequenceRecord(BigInteger number)
        {
            if (number.Sign <= 0) throw new ArgumentOutOfRangeException(nameof(number), $"{nameof(number)} <= 0");
            Max = Number = number;
        }

        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        IEnumerator<BigInteger> IEnumerable<BigInteger>.GetEnumerator()
        {
            return Iterator();

            IEnumerator<BigInteger> Iterator()
            {
                for (SequenceRecord cursor = this; cursor != null; cursor = cursor.Next)
                {
                    yield return cursor.Number;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<BigInteger>)this).GetEnumerator();

        /// <inheritdoc />
        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append(Steps);
            result.Append(':');
            for (SequenceRecord cursor = this, previous = null; cursor != null; previous = cursor, cursor = cursor.Next)
            {
                if (cursor.Number > previous?.Number) result.Append('/');
                if (cursor.Number < previous?.Number) result.Append('\\');
                if (cursor.Number == Max)
                {
                    result.Append('(');
                    result.Append(cursor.Number);
                    result.Append(')');
                }
                else
                {
                    result.Append(cursor.Number);
                }
            }
            return result.ToString();
        }
    }
}
