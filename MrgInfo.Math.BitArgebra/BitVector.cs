using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using JetBrains.Annotations;
using static System.Math;
using static MrgInfo.Math.BitAlgebra.Bit;
using static MrgInfo.Math.BitAlgebra.BitVector.Initialization;

namespace MrgInfo.Math.BitAlgebra
{
    public enum Bit: byte
    {
        Zero,
        One,
    }

    [SuppressMessage("ReSharper", "RedundantCommaInInitializer")]
    public class BitVector
    {
        public enum Initialization
        {
            Zeros,
            Ones,
            Randoms
        }

        static Bit Sum(int x)
        {
            int xx = x;
            xx ^= x >> 2;
            xx ^= x >> 4;
            xx ^= x >> 6;
            xx &= 0b11;
            return xx == 1 || xx == 2 ? One : Zero;
        }

        static Bit Sum(IReadOnlyList<int> v)
        {
            if (v == null || v.Count == 0) return Zero;
            int x = v[0];
            for (var i = 1; i < v.Count; i++)
            {
                x ^= v[i];
            }
            return Sum(x);
        }

        public static Bit Multiple(BitVector left, BitVector right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            var result = new int[Min(left._items.Length, right._items.Length)];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = left._items[i] & right._items[i];
            }
            return Sum(result);
        }

        public static Bit operator *(BitVector left, BitVector right) => Multiple(left, right);

        [NotNull] static readonly IReadOnlyList<byte> _masks = new byte[]
        {
            0b00000001, 0b00000010, 0b00000100, 0b00001000,
            0b00010000, 0b00100000, 0b01000000, 0b10000000,
        };

        static void Main()
        {
            var vector1 = new BitVector(Zero, One, One, Zero, One, One);
            var vector2 = new BitVector(One, One, One, One, One, One, One, One, One, One);
            Console.WriteLine(vector1);
            Console.WriteLine(vector2);
            Console.WriteLine(Multiple(vector1, vector2));

            var vectorA = new BitVector(24, Randoms);
            var vectorB = new BitVector(24, Randoms);
            Console.WriteLine(vectorA);
            Console.WriteLine(vectorB);
            Console.WriteLine(vectorA * vectorB);

            Console.ReadLine();
        }

        public int Dimensions { get; }

        [NotNull] readonly byte[] _items;

        public BitVector(int dimension)
            : this(dimension, Zeros)
        { }

        public BitVector(int dimension, Initialization initialization)
        {
            Dimensions = dimension;
            _items = new byte[dimension / 8 + (dimension % 8 == 0 ? 0 : 1)];
            switch (initialization)
            {
                    case Zeros:
                        break;
                    case Ones:
                        Array.Fill(_items, (byte)0b11111111);
                        break;
                    case Randoms:
                        var rnd = new Random();
                        rnd.NextBytes(_items);
                        break;
                    default:
                        goto case Zeros;
            }
        }

        public BitVector([NotNull] params Bit[] bits)
        {
            if (bits == null) throw new ArgumentNullException(nameof(bits));

            Dimensions = bits.Length;
            _items = new byte[Dimensions / 8 + (Dimensions % 8 == 0 ? 0 : 1)];
            var idx = 0;
            foreach (Bit bit in bits)
            {
                if (bit == One) _items[idx / 8] |= _masks[idx % 8];
                ++idx;
            }
        }

        public override string ToString()
        {
            var first = true;
            var result = new StringBuilder();
            int dimension = Dimensions;
            result.Append('[');
            foreach (byte item in _items)
            {
                foreach (byte mask in _masks)
                {
                    if (dimension-- == 0) break;
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        result.Append(',');
                    }
                    result.Append((item & mask) > 0 ? '1' : '0');
                }
            }
            result.Append(']');
            return result.ToString();
        }
    }
}
