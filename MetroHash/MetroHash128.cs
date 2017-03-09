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
            var result = new byte[16];
            var end = offset + count;
            ulong[] state = Unsafe.As<byte[], ulong[]>(ref result); // this is safe as both are blittable
            ref var firstState = ref state[0];
            ref var secondState = ref state[1];
            ulong thirdState = 0;
            ulong fourthState = 0;
            firstState = (seed - K0) * K3;
            secondState = (seed + K1) * K2;
            if (count >= 32)
            {
                thirdState = (seed + K0) * K2;
                fourthState = (seed - K1) * K3;
            }
            while (offset <= end - 32)
            {
                firstState += ToUlong(input, offset) * K0;
                offset += 8;
                firstState = RotateRight(firstState, 29) + thirdState;
                secondState += ToUlong(input, offset) * K1;
                offset += 8;
                secondState = RotateRight(secondState, 29) + fourthState;
                thirdState += ToUlong(input, offset) * K2;
                offset += 8;
                thirdState = RotateRight(thirdState, 29) + firstState;
                fourthState += ToUlong(input, offset) * K3;
                offset += 8;
                fourthState = RotateRight(fourthState, 29) + secondState;
            }
            thirdState ^= RotateRight((firstState + fourthState) * K0 + secondState, 21) * K1;
            fourthState ^= RotateRight((secondState + thirdState) * K1 + firstState, 21) * K0;
            firstState ^= RotateRight((firstState + thirdState) * K0 + fourthState, 21) * K1;
            secondState ^= RotateRight((secondState + fourthState) * K1 + thirdState, 21) * K0;

            if (end - offset >= 16)
            {
                firstState += ToUlong(input, offset) * K2;
                offset += 8;
                firstState = RotateRight(firstState, 33) * K3;
                secondState += ToUlong(input, offset) * K2;
                offset += 8;
                secondState = RotateRight(secondState, 33) * K3;
                firstState ^= RotateRight(firstState * K2 + secondState, 45) * K1;
                secondState ^= RotateRight(secondState * K3 + firstState, 45) * K0;
            }

            if (end - offset >= 8)
            {
                firstState += ToUlong(input, offset) * K2;
                offset += 8;
                firstState = RotateRight(firstState, 33) * K3;
                firstState ^= RotateRight(firstState * K2 + secondState, 27) * K1;
            }

            if (end - offset >= 4)
            {
                secondState += ToUint(input, offset) * K2;
                offset += 4;
                secondState = RotateRight(secondState, 33) * K3;
                secondState ^= RotateRight(secondState * K3 + firstState, 46) * K0;
            }

            if (end - offset >= 2)
            {
                firstState += ToUshort(input, offset) * K2;
                offset += 2;
                firstState = RotateRight(firstState, 33) * K3;
                firstState ^= RotateRight(firstState * K2 + secondState, 22) * K1;
            }

            if (end - offset >= 1)
            {
                secondState += ToByte(input, offset) * K2;
                secondState = RotateRight(secondState, 33) * K3;
                secondState ^= RotateRight(secondState * K3 + firstState, 58) * K0;
            }

            firstState += RotateRight(firstState * K0 + secondState, 13);
            secondState += RotateRight(secondState * K1 + firstState, 37);
            firstState += RotateRight(firstState * K2 + secondState, 13);
            secondState += RotateRight(secondState * K3 + firstState, 37);

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
            return Unsafe.As<byte, ushort>(ref data[start]);
        }

        /// <summary>
        ///     BitConverter methods are several times slower
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ToUint(byte[] data, int start)
        {
            return Unsafe.As<byte, uint>(ref data[start]);
        }

        /// <summary>
        ///     BitConverter methods are several times slower
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ToUlong(byte[] data, int start)
        {
            return Unsafe.As<byte, ulong>(ref data[start]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong RotateRight(ulong x, int r)
        {
            return (x >> r) | (x << (64 - r));
        }
    }
}