using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MetroHash
{
    /// <summary>
    ///     Metro Hash 128
    /// </summary>
    public sealed class MetroHash128
    {
        private const ulong K0 = 0xC83A91E1;
        private const ulong K1 = 0x8648DBDB;
        private const ulong K2 = 0x7BDEC03B;
        private const ulong K3 = 0x2F5870A5;
        private readonly byte[] _buffer;
        private readonly ulong[] _firstTwoStates;

        private readonly byte[] _result;
        private int _bytes;
        private ulong _fourthState;
        private ulong _thirdState;

        /// <summary>
        ///     Constructor for incremental version, call Update and FinalizeHash for full Hash
        /// </summary>
        /// <param name="seed">Seed</param>
        public MetroHash128(ulong seed)
        {
            _buffer = new byte[32];
            _result = new byte[16];
            _firstTwoStates = Unsafe.As<byte[], ulong[]>(ref _result);
            _thirdState = 0;
            _fourthState = 0;
            ref var firstState = ref _firstTwoStates[0];
            ref var secondState = ref _firstTwoStates[1];
            firstState = (seed - K0) * K3;
            secondState = (seed + K1) * K2;
            _thirdState = (seed + K0) * K2;
            _fourthState = (seed - K1) * K3;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ValidateInput(byte[] input, int offset, int count)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if ((uint) offset > (uint) input.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if ((uint) count > (uint) (input.Length - offset))
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
        }

        /// <summary>
        ///     Add data to hash
        /// </summary>
        /// <param name="input">data</param>
        /// <param name="offset">offset</param>
        /// <param name="count">count</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(byte[] input, int offset, int count)
        {
            ValidateInput(input, offset, count);
            ref var firstState = ref _firstTwoStates[0];
            ref var secondState = ref _firstTwoStates[1];
            var end = offset + count;
            var bMod = _bytes & 31;
            if (bMod != 0)
            {
                var fill = 32 - bMod;
                if (fill > count)
                {
                    fill = count;
                }

                Buffer.BlockCopy(input, offset, _buffer, bMod, fill);
                offset += fill;
                _bytes += fill;

                if ((_bytes & 31) != 0)
                {
                    return;
                }

                var tempOffset = 0;
                BulkLoop(ref firstState, ref secondState, ref _thirdState, ref _fourthState, ref _buffer[0],
                    ref tempOffset,
                    32);
            }

            _bytes += end - offset;
            BulkLoop(ref firstState, ref secondState, ref _thirdState, ref _fourthState, ref input[0], ref offset, end);

            if (offset < end)
            {
                Buffer.BlockCopy(input, offset, _buffer, 0, end - offset);
            }
        }

        /// <summary>
        ///     Add data to hash
        /// </summary>
        /// <param name="input">data</param>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(ReadOnlySpan<byte> input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            ref var firstState = ref _firstTwoStates[0];
            ref var secondState = ref _firstTwoStates[1];
            var count = input.Length;
            var offset = 0;
            var bMod = _bytes & 31;
            if (bMod != 0)
            {
                var fill = 32 - bMod;
                if (fill > count)
                {
                    fill = count;
                }

                input.Slice(0, fill).CopyTo(_buffer.AsSpan().Slice(bMod));
                _bytes += fill;
                offset += fill;

                if ((_bytes & 31) != 0)
                {
                    return;
                }

                var tempOffset = 0;
                BulkLoop(ref firstState, ref secondState, ref _thirdState, ref _fourthState, ref _buffer[0],
                    ref tempOffset, 32);
            }

            _bytes += count - offset;
            ref var start = ref MemoryMarshal.GetReference(input);
            BulkLoop(ref firstState, ref secondState, ref _thirdState, ref _fourthState, ref start, ref offset,
                input.Length);
            if (offset < count)
            {
                input.Slice(offset).CopyTo(_buffer.AsSpan());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void BulkLoop(ref ulong firstState, ref ulong secondState, ref ulong thirdState,
            ref ulong fourthState, ref byte b, ref int offset, int count)
        {
            // Create a local copy so that it remains in the CPU register.
            int localOffset = offset; // workaround for dotnet/runtime#39349

            while (localOffset <= count - 32)
            {
                firstState += Cast<ulong>(ref b, localOffset) * K0;
                localOffset += 8;
                firstState = RotateRight(firstState, 29) + thirdState;
                secondState += Cast<ulong>(ref b, localOffset) * K1;
                localOffset += 8;
                secondState = RotateRight(secondState, 29) + fourthState;
                thirdState += Cast<ulong>(ref b, localOffset) * K2;
                localOffset += 8;
                thirdState = RotateRight(thirdState, 29) + firstState;
                fourthState += Cast<ulong>(ref b, localOffset) * K3;
                localOffset += 8;
                fourthState = RotateRight(fourthState, 29) + secondState;
            }

            // Return the final result of the local register.
            offset = localOffset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FinalizeBulkLoop(ref ulong firstState, ref ulong secondState, ref ulong thirdState,
            ref ulong fourthState)
        {
            thirdState ^= RotateRight((firstState + fourthState) * K0 + secondState, 21) * K1;
            fourthState ^= RotateRight((secondState + thirdState) * K1 + firstState, 21) * K0;
            firstState ^= RotateRight((firstState + thirdState) * K0 + fourthState, 21) * K1;
            secondState ^= RotateRight((secondState + fourthState) * K1 + thirdState, 21) * K0;
        }

        private static void FinalizeHash(ref ulong firstState, ref ulong secondState, ref byte b, ref int offset,
            int count)
        {
            var end = offset + (count & 31);

            if (end - offset >= 16)
            {
                firstState += Cast<ulong>(ref b, offset) * K2;
                offset += 8;
                firstState = RotateRight(firstState, 33) * K3;
                secondState += Cast<ulong>(ref b, offset) * K2;
                offset += 8;
                secondState = RotateRight(secondState, 33) * K3;
                firstState ^= RotateRight(firstState * K2 + secondState, 45) * K1;
                secondState ^= RotateRight(secondState * K3 + firstState, 45) * K0;
            }

            if (end - offset >= 8)
            {
                firstState += Cast<ulong>(ref b, offset) * K2;
                offset += 8;
                firstState = RotateRight(firstState, 33) * K3;
                firstState ^= RotateRight(firstState * K2 + secondState, 27) * K1;
            }

            if (end - offset >= 4)
            {
                secondState += Cast<uint>(ref b, offset) * K2;
                offset += 4;
                secondState = RotateRight(secondState, 33) * K3;
                secondState ^= RotateRight(secondState * K3 + firstState, 46) * K0;
            }

            if (end - offset >= 2)
            {
                firstState += Cast<ushort>(ref b, offset) * K2;
                offset += 2;
                firstState = RotateRight(firstState, 33) * K3;
                firstState ^= RotateRight(firstState * K2 + secondState, 22) * K1;
            }

            if (end - offset >= 1)
            {
                secondState += Unsafe.Add(ref b, offset) * K2;
                secondState = RotateRight(secondState, 33) * K3;
                secondState ^= RotateRight(secondState * K3 + firstState, 58) * K0;
            }

            firstState += RotateRight(firstState * K0 + secondState, 13);
            secondState += RotateRight(secondState * K1 + firstState, 37);
            firstState += RotateRight(firstState * K2 + secondState, 13);
            secondState += RotateRight(secondState * K3 + firstState, 37);
        }

        [MethodImpl((MethodImplOptions.AggressiveInlining))]
        private static T Cast<T>(ref byte b, int offset)
        {
            return Unsafe.As<byte, T>(ref Unsafe.Add(ref b, offset));
        }

        /// <summary>
        ///     Finalizes the hash and returns the hash
        /// </summary>
        /// <returns>Hash</returns>
        public byte[] FinalizeHash()
        {
            var offset = 0;
            if (_bytes >= 32)
            {
                FinalizeBulkLoop(ref _firstTwoStates[0], ref _firstTwoStates[1], ref _thirdState, ref _fourthState);
            }

            FinalizeHash(ref _firstTwoStates[0], ref _firstTwoStates[1], ref _buffer[0],
                ref offset, _bytes);
            _bytes = 0;
            return _result;
        }

        /// <summary>
        ///     Finalizes the hash and returns the hash
        /// </summary>
        /// <param name="output">Span to write to</param>
        public void FinalizeHash(Span<byte> output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var offset = 0;
            if (_bytes >= 32)
            {
                FinalizeBulkLoop(ref _firstTwoStates[0], ref _firstTwoStates[1], ref _thirdState, ref _fourthState);
            }

            FinalizeHash(ref _firstTwoStates[0], ref _firstTwoStates[1], ref _buffer[0],
                ref offset, _bytes);
            _bytes = 0;
            _result.CopyTo(output);
        }

        /// <summary>
        ///     MetroHash 128 hash method
        ///     Not cryptographically secure
        /// </summary>
        /// <param name="seed">Seed to initialize data</param>
        /// <param name="input">Data you want to hash</param>
        /// <param name="offset">Start of the data you want to hash</param>
        /// <param name="count">Length of the data you want to hash</param>
        /// <returns>Hash</returns>
        public static byte[] Hash(ulong seed, byte[] input, int offset, int count)
        {
            ValidateInput(input, offset, count);
            var result = new byte[16];
            var end = offset + count;
            var state = Unsafe.As<byte[], ulong[]>(ref result);
            ref var firstState = ref state[0];
            ref var secondState = ref state[1];
            firstState = (seed - K0) * K3;
            secondState = (seed + K1) * K2;
            if (count >= 32)
            {
                var thirdState = (seed + K0) * K2;
                var fourthState = (seed - K1) * K3;
                BulkLoop(ref firstState, ref secondState, ref thirdState, ref fourthState, ref input[0], ref offset,
                    end);
                FinalizeBulkLoop(ref firstState, ref secondState, ref thirdState, ref fourthState);
            }

            FinalizeHash(ref firstState, ref secondState, ref input[0], ref offset, count);
            return result;
        }

        /// <summary>
        ///     MetroHash 128 hash method
        ///     Not cryptographically secure
        /// </summary>
        /// <param name="seed">Seed to initialize data</param>
        /// <param name="input">Data you want to hash</param>
        /// <param name="output">Span to write to</param>
        public static void Hash(ulong seed, ReadOnlySpan<byte> input, Span<byte> output)
        {
            var state = MemoryMarshal.Cast<byte, ulong>(output);
            ref var firstState = ref state[0];
            ref var secondState = ref state[1];
            firstState = (seed - K0) * K3;
            secondState = (seed + K1) * K2;
            var offset = 0;
            var count = input.Length;
            ref var start = ref MemoryMarshal.GetReference(input);
            if (input.Length >= 32)
            {
                var thirdState = (seed + K0) * K2;
                var fourthState = (seed - K1) * K3;
                BulkLoop(ref firstState, ref secondState, ref thirdState, ref fourthState, ref start, ref offset,
                    input.Length);
                FinalizeBulkLoop(ref firstState, ref secondState, ref thirdState, ref fourthState);
            }

            FinalizeHash(ref firstState, ref secondState, ref start, ref offset, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong RotateRight(ulong x, int r)
        {
            return (x >> r) | (x << (64 - r));
        }
    }
}
