using System.Runtime.CompilerServices;

namespace goldfish.Core.Data.Optimization;

    /// <summary>
    ///     Very fast PRNG that can replace Random for some instances.
    /// </summary>
    /// <remarks>
    ///     Optimized for x64, but even in AnyCPU, this is roughly 10% faster than System.Random.
    ///     Pulled from: http://xoroshiro.di.unimi.it/xoroshiro128plus.c
    /// </remarks>
    public class Xoroshiro128
    {
        /// <summary>
        /// Creates a new instance of Xoroshiro128, seeded from DateTime.Now
        /// </summary>
        public Xoroshiro128() : this((ulong) DateTime.Now.Ticks)
        {
            
        }
 
        /// <summary>
        /// Creates a new instance of Xoroshiro128 with a specified seed.
        /// </summary>
        /// <param name="seed">Value to seed this PRNG with.</param>
        public Xoroshiro128(ulong seed)
        {
            var x = seed;
            var z = x += 0x9E3779B97F4A7C15;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
            _seed1 = z ^ (z >> 31);
            z = x + 0x9E3779B97F4A7C15;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
            _seed2 = z ^ (z >> 31);
        }
 
        private ulong _seed1;
        private ulong _seed2;
        private static ulong[] JumpValue = {0xbeac0467eba5facb, 0xd86b048b86aa9922};
 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong rotl(ulong x, int k)
        {
            return (x << k) | (x >> (64 - k));
        }
 
        /// <summary>
        /// Get the next ulong for this instance.
        /// </summary>
        /// <returns>Next psuedo-random value.</returns>
        public ulong Next()
        {
 
            var s0 = _seed1;
            var s1 = _seed2;
            var result = s0 + s1;
 
            s1 ^= s0;
            _seed1 = rotl(s0, 55) ^ s1 ^ (s1 << 14); // a, b
            _seed2 = rotl(s1, 36); // c
 
            return result;
 
        }
 
        /// <summary>
        /// Jumps 2 ^ 64 values. This can be used to parallelize operations.
        /// </summary>
        public void Jump()
        {
            ulong s0 = 0;
            ulong s1 = 0;
            for (var i = 0; i < 2; i++)
                for (var b = 0; b < 64; b++)
                {
                    if ((JumpValue[i] & 1UL << b) > 0)
                    {
                        s0 ^= _seed1;
                        s1 ^= _seed2;
                    }
                    Next();
                }
 
            _seed1 = s0;
            _seed2 = s1;
        }
    }