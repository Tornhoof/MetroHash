using System;
using System.Runtime.CompilerServices;

namespace MetroHash
{
    /// <summary>
    /// Metro Hash 128
    /// </summary>
    public static class MetroHash128
    {
        private const ulong K0 = 0xC83A91E1;
        private const ulong K1 = 0x8648DBDB;
        private const ulong K2 = 0x7BDEC03B;
        private const ulong K3 = 0x2F5870A5;

        /// <summary>
        /// MetroHash 128 hash method
        /// Not cryptographically secure
        /// </summary>
        /// <param name="seed">Seed to initialize data</param>
        /// <param name="input">Data you want to hash</param>
        /// <param name="offset">Start of the data you want to hash</param>
        /// <param name="count">Length of the data you want to hash</param>
        /// <returns></returns>
        public static byte[] Hash(ulong seed, byte[] input, int offset, int count)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            if (input.Length < offset + count)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            var state = new ulong[4];
            var end = offset + count;
            state[0] = (seed - K0)*K3;
            state[1] = (seed + K1)*K2;
            if (count >= 32)
            {
                state[2] = (seed + K0)*K2;
                state[3] = (seed - K1)*K3;
            }
            while (offset <= end - 32)
            {
                state[0] += ToUlong(input, offset)*K0;
                offset += 8;
                state[0] = RotateRight(state[0], 29) + state[2];
                state[1] += ToUlong(input, offset)*K1;
                offset += 8;
                state[1] = RotateRight(state[1], 29) + state[3];
                state[2] += ToUlong(input, offset)*K2;
                offset += 8;
                state[2] = RotateRight(state[2], 29) + state[0];
                state[3] += ToUlong(input, offset)*K3;
                offset += 8;
                state[3] = RotateRight(state[3], 29) + state[1];
            }
            state[2] ^= RotateRight((state[0] + state[3])*K0 + state[1], 21)*K1;
            state[3] ^= RotateRight((state[1] + state[2])*K1 + state[0], 21)*K0;
            state[0] ^= RotateRight((state[0] + state[2])*K0 + state[3], 21)*K1;
            state[1] ^= RotateRight((state[1] + state[3])*K1 + state[2], 21)*K0;

            if (end - offset >= 16)
            {
                state[0] += ToUlong(input, offset)*K2;
                offset += 8;
                state[0] = RotateRight(state[0], 33)*K3;
                state[1] += ToUlong(input, offset)*K2;
                offset += 8;
                state[1] = RotateRight(state[1], 33)*K3;
                state[0] ^= RotateRight(state[0]*K2 + state[1], 45)*K1;
                state[1] ^= RotateRight(state[1]*K3 + state[0], 45)*K0;
            }

            if (end - offset >= 8)
            {
                state[0] += ToUlong(input, offset)*K2;
                offset += 8;
                state[0] = RotateRight(state[0], 33)*K3;
                state[0] ^= RotateRight(state[0]*K2 + state[1], 27)*K1;
            }

            if (end - offset >= 4)
            {
                state[1] += ToUint(input, offset)*K2;
                offset += 4;
                state[1] = RotateRight(state[1], 33)*K3;
                state[1] ^= RotateRight(state[1]*K3 + state[0], 46)*K0;
            }

            if (end - offset >= 2)
            {
                state[0] += ToUshort(input, offset)*K2;
                offset += 2;
                state[0] = RotateRight(state[0], 33)*K3;
                state[0] ^= RotateRight(state[0]*K2 + state[1], 22)*K1;
            }

            if (end - offset >= 1)
            {
                state[1] += ToByte(input, offset)*K2;
                state[1] = RotateRight(state[1], 33)*K3;
                state[1] ^= RotateRight(state[1]*K3 + state[0], 58)*K0;
            }

            state[0] += RotateRight(state[0]*K0 + state[1], 13);
            state[1] += RotateRight(state[1]*K1 + state[0], 37);
            state[0] += RotateRight(state[0]*K2 + state[1], 13);
            state[1] += RotateRight(state[1]*K3 + state[0], 37);

            var result = new byte[16];
            Buffer.BlockCopy(state, 0, result, 0, 16);
            return result;
        }

        /// <summary>
        ///     BitConverter methods are several times slower
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte ToByte(byte[] data, int start)
        {
            return data[start];
        }

        /// <summary>
        ///     BitConverter methods are several times slower
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ushort ToUshort(byte[] data, int start)
        {
            return (ushort) (data[start] | (data[start + 1] << 8));
        }

        /// <summary>
        ///     BitConverter methods are several times slower
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ToUint(byte[] data, int start)
        {
            return (uint) (data[start] | (data[start + 1] << 8) | (data[start + 2] << 16) | (data[start + 3] << 24));
        }

        /// <summary>
        ///     BitConverter methods are several times slower
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ToUlong(byte[] data, int start)
        {
            var i1 = (uint) (data[start] | (data[start + 1] << 8) | (data[start + 2] << 16) | (data[start + 3] << 24));
            var i2 =
                (ulong) (data[start + 4] | (data[start + 5] << 8) | (data[start + 6] << 16) | (data[start + 7] << 24));
            return i1 | (i2 << 32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong RotateRight(ulong x, int r)
        {
            return (x >> r) | (x << (64 - r));
        }
    }
}