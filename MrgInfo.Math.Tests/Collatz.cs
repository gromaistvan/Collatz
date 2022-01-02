using System.Numerics;
using MrgInfo.Math.Collatz;
using Xunit;

namespace MrgInfo.Math.Tests
{
    public class Collatz
    {
        [Fact]
        public void SimpleSequence()
        {
            var sequence = new CollatzSequence();
            SequenceRecord record = sequence.Get(BigInteger.One);
            Assert.NotNull(record);
            Assert.Equal(BigInteger.One, record.Max);
            Assert.Equal(0, record.Steps);
        }

        [Theory]
        [InlineData(2, 1)]
        [InlineData(4, 2)]
        [InlineData(8, 3)]
        [InlineData(16, 4)]
        public void TrivialSequence(int number, int steps)
        {
            var sequence = new CollatzSequence();
            SequenceRecord record = sequence.Get(number);
            Assert.NotNull(record);
            Assert.Equal(number, record.Max);
            Assert.Equal(steps, record.Steps);
        }
    }
}
