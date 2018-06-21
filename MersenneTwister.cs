namespace MersenneTwister
{
    class MersenneTwister
    {
        private const uint A =           0b10011001000010001011000011011111; // 2567483615 // 0x9908B0DF
        private const uint B =           0b10011101001011000101011010000000; // 2636928640 // 0x9D2C5680
        private const uint C =           0b11101111110001100000000000000000; // 4022730752 // 0xEFC60000
        private const uint D =           0b11111111111111111111111111111111; // 4294967295 // 0xFFFFFFFF
        private const uint F =           0b01101100000001111000100101100101; // 1812433253 // 0x6C078965
        private const uint L =           0b00000000000000000000000000010010; // 0000000018 // 0x00000012
        private const uint M =           0b00000000000000000000000110001101; // 0000000397 // 0x0000018D
        private const uint N =           0b00000000000000000000001001110000; // 0000000624 // 0x00000270
        private const uint R =           0b00000000000000000000000000011111; // 0000000031 // 0x0000001F
        private const uint S =           0b00000000000000000000000000000111; // 0000000007 // 0x00000007
        private const uint T =           0b00000000000000000000000000001111; // 0000000015 // 0x0000000F
        private const uint U =           0b00000000000000000000000000001011; // 0000000011 // 0x0000000B
        private const uint W =           0b00000000000000000000000000100000; // 0000000032 // 0x00000020
        private const uint MASK_LOWER =  0b01111111111111111111111111111111; // 2147483647 // 0x7FFFFFFF
        private const uint MASK_UPPER =  0b10000000000000000000000000000000; // 2147483648 // 0x80000000

        private uint[] mt;
        private uint index;

        public MersenneTwister(uint seed)
        {
            mt = new uint[N];
            index = N;
            mt[0] = seed;

            for (uint i = 1; i < N; i++)
            {
                mt[i] = (F * (mt[i - 1] ^ (mt[i - 1] >> 30)) + i);
            }
        }

        public uint Random()
        {
            uint i = index;
            if (index >= N)
            {
                this.Twist();
                i = index;
            }
            uint y = mt[i];
            index = i + 1;

            y ^= (mt[i] >> (int)U);
            y ^= (y << (int)S) & B;
            y ^= (y << (int)T) & C;
            y ^= (y >> (int)L);

            return y;
        }

        private void Twist()
        {
            uint i, x, xA;
            for (i = 0; i < N; i++)
            {
                x = (mt[i] & MASK_UPPER) + (mt[(i + 1) % N] & MASK_LOWER);
                xA = x >> 1;
                if ((x & 0x1) == 1)
                {
                    xA ^= A;
                }
                mt[i] = mt[(i + M) % N] ^ xA;
            }
            index = 0;
        }
    }
}