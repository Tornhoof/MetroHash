using System;
using System.Text;
using Xunit;

namespace MetroHash.Tests
{
    public class Tests
    {
        private const string TestString = "012345678901234567890123456789012345678901234567890123456789012";

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(128)]
        public void IncrementalMultiUpdate(int partSize)
        {
            var input = Encoding.ASCII.GetBytes(TestString);
            var metroHash = new MetroHash128(0);
            var remaining = input.Length;
            var offset = 0;
            while (remaining > 0)
            {
                var part = new byte[partSize];
                var toCopy = Math.Min(partSize, remaining);
                Buffer.BlockCopy(input, offset, part, 0, toCopy);
                metroHash.Update(part, 0, toCopy);
                remaining -= toCopy;
                offset += toCopy;
            }
            var hash = metroHash.FinalizeHash();
            var comparison = new byte[]
            {
                0xC7, 0x7C, 0xE2, 0xBF, 0xA4, 0xED, 0x9F, 0x9B,
                0x05, 0x48, 0xB2, 0xAC, 0x50, 0x74, 0xA2, 0x97
            };
            Assert.Equal(comparison, hash);
        }


        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(128)]
        public void MultipleSizes(int size)
        {
            var prng = new Random();
            var input = new byte[size];
            prng.NextBytes(input);

            for (var i = 0; i < size; i++)
            {
                var firstHash = MetroHash128.Hash(0, input, 0, input.Length);
                var metroHash = new MetroHash128(0);
                metroHash.Update(input, 0, input.Length);
                var secondHash = metroHash.FinalizeHash();
                Assert.Equal(firstHash, secondHash);
                var thirdHash = OldMetroHash128.Hash(0, input, 0, input.Length);
                Assert.Equal(firstHash, thirdHash);
            }
        }


        [Fact]
        public void IncrementalSeed0()
        {
            var input = Encoding.ASCII.GetBytes(TestString);
            var metroHash = new MetroHash128(0);
            metroHash.Update(input, 0, input.Length);
            var hash = metroHash.FinalizeHash();
            var comparison = new byte[]
            {
                0xC7, 0x7C, 0xE2, 0xBF, 0xA4, 0xED, 0x9F, 0x9B,
                0x05, 0x48, 0xB2, 0xAC, 0x50, 0x74, 0xA2, 0x97
            };
            Assert.Equal(comparison, hash);
        }

        [Fact]
        public void IncrementalSeed1()
        {
            var input = Encoding.ASCII.GetBytes(TestString);
            var metroHash = new MetroHash128(1);
            metroHash.Update(input, 0, input.Length);
            var hash = metroHash.FinalizeHash();
            var comparison = new byte[]
            {
                0x45, 0xA3, 0xCD, 0xB8, 0x38, 0x19, 0x9D, 0x7F,
                0xBD, 0xD6, 0x8D, 0x86, 0x7A, 0x14, 0xEC, 0xEF
            };
            Assert.Equal(comparison, hash);
        }


        [Fact]
        public void IncrementalSeed1SingleByte()
        {
            var input = Encoding.ASCII.GetBytes(TestString);
            var metroHash = new MetroHash128(1);
            for (var i = 0; i < input.Length; i++)
                metroHash.Update(new byte[1] {input[i]}, 0, 1);
            var hash = metroHash.FinalizeHash();
            var comparison = new byte[]
            {
                0x45, 0xA3, 0xCD, 0xB8, 0x38, 0x19, 0x9D, 0x7F,
                0xBD, 0xD6, 0x8D, 0x86, 0x7A, 0x14, 0xEC, 0xEF
            };
            Assert.Equal(comparison, hash);
        }


        [Fact]
        public void IncrementalSeed1SingleByteOffset()
        {
            var input = Encoding.ASCII.GetBytes(TestString);
            var metroHash = new MetroHash128(1);
            for (var i = 0; i < input.Length; i++)
                metroHash.Update(input, i, 1);
            var hash = metroHash.FinalizeHash();
            var comparison = new byte[]
            {
                0x45, 0xA3, 0xCD, 0xB8, 0x38, 0x19, 0x9D, 0x7F,
                0xBD, 0xD6, 0x8D, 0x86, 0x7A, 0x14, 0xEC, 0xEF
            };
            Assert.Equal(comparison, hash);
        }

        [Fact]
        public void NonIncrementalSeed0()
        {
            var input = Encoding.ASCII.GetBytes(TestString);
            var hash = MetroHash128.Hash(0, input, 0, input.Length);
            var comparison = new byte[]
            {
                0xC7, 0x7C, 0xE2, 0xBF, 0xA4, 0xED, 0x9F, 0x9B,
                0x05, 0x48, 0xB2, 0xAC, 0x50, 0x74, 0xA2, 0x97
            };
            Assert.Equal(comparison, hash);
        }

        [Fact]
        public void NonIncrementalSeed1()
        {
            var input = Encoding.ASCII.GetBytes(TestString);
            var hash = MetroHash128.Hash(1, input, 0, input.Length);
            var secondHash = OldMetroHash128.Hash(1, input, 0, input.Length);
            var comparison = new byte[]
            {
                0x45, 0xA3, 0xCD, 0xB8, 0x38, 0x19, 0x9D, 0x7F,
                0xBD, 0xD6, 0x8D, 0x86, 0x7A, 0x14, 0xEC, 0xEF
            };
            Assert.Equal(comparison, hash);
            Assert.Equal(comparison, secondHash);
        }

        [Fact]
        public void RandomSizes()
        {
            for (var i = 0; i < 1000; i++)
            {
                var prng = new Random();
                var size = prng.Next(10000);
                var input = new byte[size];
                prng.NextBytes(input);

                var firstHash = MetroHash128.Hash(0, input, 0, input.Length);
                var metroHash = new MetroHash128(0);
                metroHash.Update(input, 0, input.Length);
                var secondHash = metroHash.FinalizeHash();
                Assert.Equal(firstHash, secondHash);
                var thirdHash = OldMetroHash128.Hash(0, input, 0, input.Length);
                Assert.Equal(firstHash, thirdHash);
            }
        }
    }
}