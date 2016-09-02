using System.Text;
using Xunit;

namespace MetroHash.Tests
{
    public class Tests
    {
        private const string TestString = "012345678901234567890123456789012345678901234567890123456789012";

        [Fact]
        public void Seed0()
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
        public void Seed1()
        {
            var input = Encoding.ASCII.GetBytes(TestString);
            var hash = MetroHash128.Hash(1, input, 0, input.Length);
            var comparison = new byte[]
            {
                0x45, 0xA3, 0xCD, 0xB8, 0x38, 0x19, 0x9D, 0x7F,
                0xBD, 0xD6, 0x8D, 0x86, 0x7A, 0x14, 0xEC, 0xEF
            };
            Assert.Equal(comparison, hash);
        }
    }
}