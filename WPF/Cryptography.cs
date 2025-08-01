﻿using CRINGEGRAM.Models;
using System;
using System.Text;
using System.Numerics;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace CRINGEGRAM
{
    public interface Cryptography
    {
        void       KeySetup(BigInteger ZOV);
        BigInteger Encrypt(BigInteger ZOV);
        BigInteger Decrypt(BigInteger ZOV);
    }

    public class MARS : Cryptography
    {
        private uint A, B, C, D;
        private uint[] S = {
            0x09D0C479, 0x28C8FFE0, 0x84AA6C39, 0x9DAD7287, 0x7DFF9BE3, 0xD4268361, 0xC96DA1D4, 0x7974CC93, 0x85D0582E, 0x2A4B5705,
            0x1CA16A62, 0xC3BD279D, 0x0F1F25E5, 0x5160372F, 0xC695C1FB, 0x4D7FF1E4, 0xAE5F6BF4, 0x0D72EE46, 0xFF23DE8A, 0xB1CF8E83,
            0xF14902E2, 0x3E981E42, 0x8BF53EB6, 0x7F4BF8AC, 0x83631F83, 0x25970205, 0x76AFE784, 0x3A7931D4, 0x4F846450, 0x5C64C3F6,
            0x210A5F18, 0xC6986A26, 0x28F4E826, 0x3A60A81C, 0xD340A664, 0x7EA820C4, 0x526687C5, 0x7EDDD12B, 0x32A11D1D, 0x9C9EF086,
            0x80F6E831, 0xAB6F04AD, 0x56FB9B53, 0x8B2E095C, 0xB68556AE, 0xD2250B0D, 0x294A7721, 0xE21FB253, 0xAE136749, 0xE82AAE86,
            0x93365104, 0x99404A66, 0x78A784DC, 0xB69BA84B, 0x04046793, 0x23DB5C1E, 0x46CAE1D6, 0x2FE28134, 0x5A223942, 0x1863CD5B,
            0xC190C6E3, 0x07DFB846, 0x6EB88816, 0x2D0DCC4A, 0xA4CCAE59, 0x3798670D, 0xCBFA9493, 0x4F481D45, 0xEAFC8CA8, 0xDB1129D6,
            0xB0449E20, 0x0F5407FB, 0x6167D9A8, 0xD1F45763, 0x4DAA96C3, 0x3BEC5958, 0xABABA014, 0xB6CCD201, 0x38D6279F, 0x02682215,
            0x8F376CD5, 0x092C237E, 0xBFC56593, 0x32889D2C, 0x854B3E95, 0x05BB9B43, 0x7DCD5DCD, 0xA02E926C, 0xFAE527E5, 0x36A1C330,
            0x3412E1AE, 0xF257F462, 0x3C4F1D71, 0x30A2E809, 0x68E5F551, 0x9C61BA44, 0x5DED0AB8, 0x75CE09C8, 0x9654F93E, 0x698C0CCA,
            0x243CB3E4, 0x2B062B97, 0x0F3B8D9E, 0x00E050DF, 0xFC5D6166, 0xE35F9288, 0xC079550D, 0x0591AEE8, 0x8E531E74, 0x75FE3578,
            0x2F6D829A, 0xF60B21AE, 0x95E8EB8D, 0x6699486B, 0x901D7D9B, 0xFD6D6E31, 0x1090ACEF, 0xE0670DD8, 0xDAB2E692, 0xCD6D4365,
            0xE5393514, 0x3AF345F0, 0x6241FC4D, 0x460DA3A3, 0x7BCF3729, 0x8BF1D1E0, 0x14AAC070, 0x1587ED55, 0x3AFD7D3E, 0xD2F29E01,
            0x29A9D1F6, 0xEFB10C53, 0xCF3B870F, 0xB414935C, 0x664465ED, 0x024ACAC7, 0x59A744C1, 0x1D2936A7, 0xDC580AA6, 0xCF574CA8,
            0x040A7A10, 0x6CD81807, 0x8A98BE4C, 0xACCEA063, 0xC33E92B5, 0xD1E0E03D, 0xB322517E, 0x2092BD13, 0x386B2C4A, 0x52E8DD58,
            0x58656DFB, 0x50820371, 0x41811896, 0xE337EF7E, 0xD39FB119, 0xC97F0DF6, 0x68FEA01B, 0xA150A6E5, 0x55258962, 0xEB6FF41B,
            0xD7C9CD7A, 0xA619CD9E, 0xBCF09576, 0x2672C073, 0xF003FB3C, 0x4AB7A50B, 0x1484126A, 0x487BA9B1, 0xA64FC9C6, 0xF6957D49,
            0x38B06A75, 0xDD805FCD, 0x63D094CF, 0xF51C999E, 0x1AA4D343, 0xB8495294, 0xCE9F8E99, 0xBFFCD770, 0xC7C275CC, 0x378453A7,
            0x7B21BE33, 0x397F41BD, 0x4E94D131, 0x92CC1F98, 0x5915EA51, 0x99F861B7, 0xC9980A88, 0x1D74FD5F, 0xB0A495F8, 0x614DEED0,
            0xB5778EEA, 0x5941792D, 0xFA90C1F8, 0x33F824B4, 0xC4965372, 0x3FF6D550, 0x4CA5FEC0, 0x8630E964, 0x5B3FBBD6, 0x7DA26A48,
            0xB203231A, 0x04297514, 0x2D639306, 0x2EB13149, 0x16A45272, 0x532459A0, 0x8E5F4872, 0xF966C7D9, 0x07128DC0, 0x0D44DB62,
            0xAFC8D52D, 0x06316131, 0xD838E7CE, 0x1BC41D00, 0x3A2E8C0F, 0xEA83837E, 0xB984737D, 0x13BA4891, 0xC4F8B949, 0xA6D6ACB3,
            0xA215CDCE, 0x8359838B, 0x6BD1AA31, 0xF579DD52, 0x21B93F93, 0xF5176781, 0x187DFDDE, 0xE94AEB76, 0x2B38FD54, 0x431DE1DA,
            0xAB394825, 0x9AD3048F, 0xDFEA32AA, 0x659473E3, 0x623F7863, 0xF3346C59, 0xAB3AB685, 0x3346A90B, 0x6B56443E, 0xC6DE01F8,
            0x8D421FC0, 0x9B0ED10C, 0x88F1A1E9, 0x54C1F029, 0x7DEAD57B, 0x8D7BA426, 0x4CF5178A, 0x551A7CCA, 0x1A9A5F08, 0xFCD651B9,
            0x25605182, 0xE11FC6C3, 0xB6FD9676, 0x337B3027, 0xB7C8EB14, 0x9E5FD030, 0x6B57E354, 0xAD913CF7, 0x7E16688D, 0x58872A69,
            0x2C2FC7DF, 0xE389CCC6, 0x30738DF1, 0x0824A734, 0xE1797A8B, 0xA4A8D57B, 0x5B5D193B, 0xC8A8309B, 0x73F9A978, 0x73398D32,
            0x0F59573E, 0xE9DF2B03, 0xE8A5B6C8, 0x848D0704, 0x98DF93C2, 0x720A1DC3, 0x684F259A, 0x943BA848, 0xA6370152, 0x863B5EA3,
            0xD17B978B, 0x6D9B58EF, 0x0A700DD4, 0xA73D36BF, 0x8E6A0829, 0x8695BC14, 0xE35B3447, 0x933AC568, 0x8894B022, 0x2F511C27,
            0xDDFBCC3C, 0x006662B6, 0x117C83FE, 0x4E12B414, 0xC2BCA766, 0x3A2FEC10, 0xF4562420, 0x55792E2A, 0x46F5D857, 0xCEDA25CE,
            0xC3601D3B, 0x6C00AB46, 0xEFAC9C28, 0xB3C35047, 0x611DFEE3, 0x257C3207, 0xFDD58482, 0x3B14D84F, 0x23BECB64, 0xA075F3A3,
            0x088F8EAD, 0x07ADF158, 0x7796943C, 0xFACABF3D, 0xC09730CD, 0xF7679969, 0xDA44E9ED, 0x2C854C12, 0x35935FA3, 0x2F057D9F,
            0x690624F8, 0x1CB0BAFD, 0x7B0DBDC6, 0x810F23BB, 0xFA929A1A, 0x6D969A17, 0x6742979B, 0x74AC7D05, 0x010E65C4, 0x86A3D963,
            0xF907B5A0, 0xD0042BD3, 0x158D7D03, 0x287A8255, 0xBBA8366F, 0x096EDC33, 0x21916A7B, 0x77B56B86, 0x951622F9, 0xA6C5E650,
            0x8CEA17D1, 0xCD8C62BC, 0xA3D63433, 0x358A68FD, 0x0F9B9D3C, 0xD6AA295B, 0xFE33384A, 0xC000738E, 0xCD67EB2F, 0xE2EB6DC2,
            0x97338B02, 0x06C9F246, 0x419CF1AD, 0x2B83C045, 0x3723F18A, 0xCB5B3089, 0x160BEAD7, 0x5D494656, 0x35F8A74B, 0x1E4E6C9E,
            0x000399BD, 0x67466880, 0xB4174831, 0xACF423B2, 0xCA815AB3, 0x5A6395E7, 0x302A67C5, 0x8BDB446B, 0x108F8FA4, 0x10223EDA,
            0x92B8B48B, 0x7F38D0EE, 0xAB2701D4, 0x0262D415, 0xAF224A30, 0xB3D88ABA, 0xF8B2C3AF, 0xDAF7EF70, 0xCC97D3B7, 0xE9614B6C,
            0x2BAEBFF4, 0x70F687CF, 0x386C9156, 0xCE092EE5, 0x01E87DA6, 0x6CE91E6A, 0xBB7BCC84, 0xC7922C20, 0x9D3B71FD, 0x060E41C6,
            0xD7590F15, 0x4E03BB47, 0x183C198E, 0x63EEB240, 0x2DDBF49A, 0x6D5CBA54, 0x923750AF, 0xF9E14236, 0x7838162B, 0x59726C72,
            0x81B66760, 0xBB2926C1, 0x48A0CE0D, 0xA6C0496D, 0xAD43507B, 0x718D496A, 0x9DF057AF, 0x44B1BDE6, 0x054356DC, 0xDE7CED35,
            0xD51A138B, 0x62088CC9, 0x35830311, 0xC96EFCA2, 0x686F86EC, 0x8E77CB68, 0x63E1D6B8, 0xC80F9778, 0x79C491FD, 0x1B4C67F2,
            0x72698D7D, 0x5E368C31, 0xF7D95E2E, 0xA1D3493F, 0xDCD9433E, 0x896F1552, 0x4BC4CA7A, 0xA6D1BAF4, 0xA5A96DCC, 0x0BEF8B46,
            0xA169FDA7, 0x74DF40B7, 0x4E208804, 0x9A756607, 0x038E87C8, 0x20211E44, 0x8B7AD4BF, 0xC6403F35, 0x1848E36D, 0x80BDB038,
            0x1E62891C, 0x643D2107, 0xBF04D6F8, 0x21092C8C, 0xF644F389, 0x0778404E, 0x7B78ADB8, 0xA2C52D53, 0x42157ABE, 0xA2253E2E,
            0x7BF3F4AE, 0x80F594F9, 0x953194E7, 0x77EB92ED, 0xB3816930, 0xDA8D9336, 0xBF447469, 0xF26D9483, 0xEE6FAED5, 0x71371235,
            0xDE425F73, 0xB4E59F43, 0x7DBE2D4E, 0x2D37B185, 0x49DC9A63, 0x98C39D98, 0x1301C9A2, 0x389B1BBF, 0x0C18588D, 0xA421C1BA,
            0x7AA3865C, 0x71E08558, 0x3C5CFCAA, 0x7D239CA4, 0x0297D9DD, 0xD7DC2830, 0x4B37802B, 0x7428AB54, 0xAEEE0347, 0x4B3FBB85,
            0x692F2F08, 0x134E578E, 0x36D9E0BF, 0xAE8B5FCF, 0xEDB93ECF, 0x2B27248E, 0x170EB1EF, 0x7DC57FD6, 0x1E760F16, 0xB1136601,
            0x864E1B9B, 0xD7EA7319, 0x3AB871BD, 0xCFA4D76F, 0xE31BD782, 0x0DBEB469, 0xABB96061, 0x5370F85D, 0xFFB07E37, 0xDA30D0FB,
            0xEBC977B6, 0x0B98B40F, 0x3A4D0FE6, 0xDF4FC26B, 0x159CF22A, 0xC298D6E2, 0x2B78EF6A, 0x61A94AC0, 0xAB561187, 0x14EEA0F0,
            0xDF0D4164, 0x19AF70EE};
        private uint[] T = new uint[15];
        private uint[] K = new uint[40];

        public void KeySetup(BigInteger ZOV)
        {
            uint[] Key = MessageCrypt.NumToArray<uint>(ZOV);
            Array.Copy(Key, T, Key.Length);
            T[Key.Length] = (uint)Key.Length;

            for (byte j = 0; j < 4; j++)
            {
                for (byte i = 0; i < 15; i++)
                    T[i] = (uint)(T[i] ^ RL(T[(i + 8) % 15] ^ T[(i + 13) % 15], 3) ^ 4 * i + j);

                for (byte t = 0; t < 4; t++)
                    for (byte i = 0; i < 15; i++)
                        T[i] = T[i] + RL(S[T[(i + 14) % 15] & 0x1FF], 9);

                for (byte i = 0; i < 10; i++)
                    K[10 * j + i] = T[4 * i % 15];
            }

            for (byte i = 5; i < 36; i += 2)
            {
                uint w = K[i] | 3;
                uint p = RL(S[(K[i] & 3) + 265], (int)(K[i - 1] & 0x1F));
                K[i] = w ^ p & GenerateMask(w);
            }
        }

        public BigInteger Encrypt(BigInteger ZOV)
        {
            uint[] Data = MessageCrypt.NumToArray<uint>(ZOV);

            A = Data[0] + K[0]; C = Data[2] + K[2];
            B = Data[1] + K[1]; D = Data[3] + K[3];

            // Forward Mixing
            for (short i = 0; i < 8; i++)
            {
                B ^= S[A & 0xFF];
                B += S[(RR(A, 8) & 0xFF) + 256];
                C += S[RR(A, 16) & 0xFF];
                D ^= S[(RR(A, 24) & 0xFF) + 256];
                A = RR(A, 24);
                A += i == 1 || i == 5 ? B : 0;
                A += i == 0 || i == 4 ? D : 0;
                TransportBlocks();
            }

            // Cryptographic Core
            for (short i = 0; i < 16; i++)
            {
                uint R = RL(RL(A, 13) * K[2 * i + 5], 10);
                uint M = RL(A + K[2 * i + 4], (int)(RR(R, 5) & 0x1F));
                uint L = RL(S[M & 0x1FF] ^ RR(R, 5) ^ R, (int)(R & 0x1F));
                B += i < 8 ? L : 0;
                D ^= i < 8 ? R : 0;
                C = C + M;
                B ^= i >= 8 ? R : 0;
                D += i >= 8 ? L : 0;
                A = RL(A, 13);
                TransportBlocks();
            }

            // Backwards Mixing
            for (short i = 0; i < 8; i++)
            {
                A -= i == 2 || i == 6 ? D : 0;
                A -= i == 3 || i == 7 ? B : 0;
                B ^= S[(A & 0xFF) + 256];
                C -= S[RL(A, 8) & 0xFF];
                D -= S[(RL(A, 16) & 0xFF) + 256];
                D ^= S[RL(A, 24) & 0xFF];
                A = RL(A, 24);
                TransportBlocks();
            }

            A -= K[36]; C -= K[38];
            B -= K[37]; D -= K[39];


            return MessageCrypt.ArrayToNum(new uint[] { A, B, C, D });
        }

        public BigInteger Decrypt(BigInteger ZOV)
        {
            uint[] Data = MessageCrypt.NumToArray<uint>(ZOV);

            A = Data[0] + K[36]; C = Data[2] + K[38];
            B = Data[1] + K[37]; D = Data[3] + K[39];

            // Reverse Backwards Mixing
            for (short i = 7; i >= 0; i--)
            {
                ReverseTransportBlocks();
                A = RR(A, 24);
                D ^= S[RR(A, 8) & 0xFF];
                D += S[(RR(A, 16) & 0xFF) + 256];
                C += S[RR(A, 24) & 0xFF];
                B ^= S[(A & 0xFF) + 256];
                A += i == 3 || i == 7 ? B : 0;
                A += i == 2 || i == 6 ? D : 0;
            }

            // Reverse Cryptographic Core
            for (short i = 15; i >= 0; i--)
            {
                ReverseTransportBlocks();
                A = RR(A, 13);
                uint R = RL(RL(A, 13) * K[2 * i + 5], 10);
                uint M = RL(A + K[2 * i + 4], (int)(RR(R, 5) & 0x1F));
                uint L = RL(S[M & 0x1FF] ^ RR(R, 5) ^ R, (int)(R & 0x1F));
                D -= i >= 8 ? L : 0;
                B ^= i >= 8 ? R : 0;
                C = C - M;
                D ^= i < 8 ? R : 0;
                B -= i < 8 ? L : 0;
            }

            // Reverse Forward Mixing
            for (short i = 7; i >= 0; i--)
            {
                ReverseTransportBlocks();
                A -= i == 0 || i == 4 ? D : 0;
                A -= i == 1 || i == 5 ? B : 0;
                A = RL(A, 24);
                D ^= S[(RR(A, 24) & 0xFF) + 256];
                C -= S[RR(A, 16) & 0xFF];
                B -= S[(RR(A, 8) & 0xFF) + 256];
                B ^= S[A & 0xFF];
            }

            A -= K[0]; C -= K[2];
            B -= K[1]; D -= K[3];

            return MessageCrypt.ArrayToNum(new uint[] { A, B, C, D });
        }

        private uint GenerateMask(uint x)
        {
            uint M = 0;
            M = (~x ^ x >> 1) & 0x7FFFFFFF;

            M &= M >> 1 & M >> 2;
            M &= M >> 3 & M >> 6;

            M <<= 1;
            M |= M << 1;
            M |= M << 2;
            M |= M << 4;

            return M & 0xFFFFFFFC;
        }

        private void TransportBlocks()
        {
            uint swap = A;
            A = B;
            B = C;
            C = D;
            D = swap;
        }

        private void ReverseTransportBlocks()
        {
            uint swap = D;
            D = C;
            C = B;
            B = A;
            A = swap;
        }

        private uint RL(uint value, int shift)
        {
            return value << shift | (value >>> (32 - shift));
        }

        private uint RR(uint value, int shift)
        {
            return (value >>> shift) | value << 32 - shift;
        }
    }

    public class MacGuffin : Cryptography
    {
        private ushort[,] S =
        {
            {
            0x0002, 0x0000, 0x0000, 0x0003, 0x0003, 0x0001, 0x0001, 0x0000,
            0x0000, 0x0002, 0x0003, 0x0000, 0x0003, 0x0003, 0x0002, 0x0001, 
            0x0001, 0x0002, 0x0002, 0x0000, 0x0000, 0x0002, 0x0002, 0x0003,
            0x0001, 0x0003, 0x0003, 0x0001, 0x0000, 0x0001, 0x0001, 0x0002,
            0x0000, 0x0003, 0x0001, 0x0002, 0x0002, 0x0002, 0x0002, 0x0000,
            0x0003, 0x0000, 0x0000, 0x0003, 0x0000, 0x0001, 0x0003, 0x0001,
            0x0003, 0x0001, 0x0002, 0x0003, 0x0003, 0x0001, 0x0001, 0x0002,
            0x0001, 0x0002, 0x0002, 0x0000, 0x0001, 0x0000, 0x0000, 0x0003
            },

            {
            0x000c, 0x0004, 0x0004, 0x000c, 0x0008, 0x0000, 0x0008, 0x0004,
            0x0000, 0x000c, 0x000c, 0x0000, 0x0004, 0x0008, 0x0000, 0x0008,
            0x000c, 0x0008, 0x0004, 0x0000, 0x0000, 0x0004, 0x000c, 0x0008,
            0x0008, 0x0000, 0x0000, 0x000c, 0x0004, 0x000c, 0x0008, 0x0004,
            0x0000, 0x000c, 0x0008, 0x0008, 0x0004, 0x0008, 0x000c, 0x0004,
            0x0008, 0x0004, 0x0000, 0x000c, 0x000c, 0x0000, 0x0004, 0x0000,
            0x0004, 0x000c, 0x0008, 0x0000, 0x0008, 0x0004, 0x0000, 0x0008,
            0x000c, 0x0000, 0x0004, 0x0004, 0x0000, 0x0008, 0x000c, 0x000c
            },

            {
            0x0020, 0x0030, 0x0000, 0x0010, 0x0030, 0x0000, 0x0020, 0x0030,
            0x0000, 0x0010, 0x0010, 0x0000, 0x0030, 0x0000, 0x0010, 0x0020,
            0x0010, 0x0000, 0x0030, 0x0020, 0x0020, 0x0010, 0x0010, 0x0020,
            0x0030, 0x0020, 0x0000, 0x0030, 0x0000, 0x0030, 0x0020, 0x0010,
            0x0030, 0x0010, 0x0000, 0x0020, 0x0000, 0x0030, 0x0030, 0x0000,
            0x0020, 0x0000, 0x0030, 0x0030, 0x0010, 0x0020, 0x0000, 0x0010,
            0x0030, 0x0000, 0x0010, 0x0030, 0x0000, 0x0020, 0x0020, 0x0010,
            0x0010, 0x0030, 0x0020, 0x0010, 0x0020, 0x0000, 0x0010, 0x0020
            },

            {
            0x0040, 0x00c0, 0x00c0, 0x0080, 0x0080, 0x00c0, 0x0040, 0x0040,
            0x0000, 0x0000, 0x0000, 0x00c0, 0x00c0, 0x0000, 0x0080, 0x0040,
            0x0040, 0x0000, 0x0000, 0x0040, 0x0080, 0x0000, 0x0040, 0x0080,
            0x00c0, 0x0040, 0x0080, 0x0080, 0x0000, 0x0080, 0x00c0, 0x00c0,
            0x0080, 0x0040, 0x0000, 0x00c0, 0x00c0, 0x0000, 0x0000, 0x0000,
            0x0080, 0x0080, 0x00c0, 0x0040, 0x0040, 0x00c0, 0x00c0, 0x0080,
            0x00c0, 0x00c0, 0x0040, 0x0000, 0x0040, 0x0040, 0x0080, 0x00c0,
            0x0040, 0x0080, 0x0000, 0x0040, 0x0080, 0x0000, 0x0000, 0x0080
            },

            {
            0x0000, 0x0200, 0x0200, 0x0300, 0x0000, 0x0000, 0x0100, 0x0200,
            0x0100, 0x0000, 0x0200, 0x0100, 0x0300, 0x0300, 0x0000, 0x0100,
            0x0200, 0x0100, 0x0100, 0x0000, 0x0100, 0x0300, 0x0300, 0x0200,
            0x0300, 0x0100, 0x0000, 0x0300, 0x0200, 0x0200, 0x0300, 0x0000,
            0x0000, 0x0300, 0x0000, 0x0200, 0x0100, 0x0200, 0x0300, 0x0100,
            0x0200, 0x0100, 0x0300, 0x0200, 0x0100, 0x0000, 0x0200, 0x0300,
            0x0300, 0x0000, 0x0300, 0x0300, 0x0200, 0x0000, 0x0100, 0x0300,
            0x0000, 0x0200, 0x0100, 0x0000, 0x0000, 0x0100, 0x0200, 0x0100
            },

            {
            0x0800, 0x0800, 0x0400, 0x0c00, 0x0800, 0x0000, 0x0c00, 0x0000,
            0x0c00, 0x0400, 0x0000, 0x0800, 0x0000, 0x0c00, 0x0800, 0x0400,
            0x0000, 0x0000, 0x0c00, 0x0400, 0x0400, 0x0c00, 0x0000, 0x0800,
            0x0800, 0x0000, 0x0400, 0x0c00, 0x0400, 0x0400, 0x0c00, 0x0800,
            0x0c00, 0x0000, 0x0800, 0x0400, 0x0c00, 0x0000, 0x0400, 0x0800,
            0x0000, 0x0c00, 0x0800, 0x0400, 0x0800, 0x0c00, 0x0400, 0x0800,
            0x0400, 0x0c00, 0x0000, 0x0800, 0x0000, 0x0400, 0x0800, 0x0400,
            0x0400, 0x0000, 0x0c00, 0x0000, 0x0c00, 0x0800, 0x0000, 0x0c00
            },

            {
            0x0000, 0x3000, 0x3000, 0x0000, 0x0000, 0x3000, 0x2000, 0x1000,
            0x3000, 0x0000, 0x0000, 0x3000, 0x2000, 0x1000, 0x3000, 0x2000,
            0x1000, 0x2000, 0x2000, 0x1000, 0x3000, 0x1000, 0x1000, 0x2000,
            0x1000, 0x0000, 0x2000, 0x3000, 0x0000, 0x2000, 0x1000, 0x0000,
            0x1000, 0x0000, 0x0000, 0x3000, 0x3000, 0x3000, 0x3000, 0x2000,
            0x2000, 0x1000, 0x1000, 0x0000, 0x1000, 0x2000, 0x2000, 0x1000,
            0x2000, 0x3000, 0x3000, 0x1000, 0x0000, 0x0000, 0x2000, 0x3000,
            0x0000, 0x2000, 0x1000, 0x0000, 0x3000, 0x1000, 0x0000, 0x2000
            },

            {
            0xc000, 0x4000, 0x0000, 0xc000, 0x8000, 0xc000, 0x0000, 0x8000,
            0x0000, 0x8000, 0xc000, 0x4000, 0xc000, 0x4000, 0x4000, 0x0000,
            0x8000, 0x8000, 0xc000, 0x4000, 0x4000, 0x0000, 0x8000, 0xc000,
            0x4000, 0x0000, 0x0000, 0x8000, 0x8000, 0xc000, 0x4000, 0x0000,
            0x4000, 0x0000, 0xc000, 0x4000, 0x0000, 0x8000, 0x4000, 0x4000,
            0xc000, 0x0000, 0x8000, 0x8000, 0x8000, 0x8000, 0x0000, 0xc000,
            0x0000, 0xc000, 0x0000, 0x8000, 0x8000, 0xc000, 0xc000, 0x0000,
            0xc000, 0x4000, 0x4000, 0x4000, 0x4000, 0x0000, 0x8000, 0xc000
            }
        };
        private ushort[,] SB =
        {
            {2, 5, 6, 9, 11, 13},
            {1, 4, 7, 10, 8, 14},
            {3, 6, 8, 13, 0, 15},
            {12, 14, 1, 2, 4, 10},
            {0, 10, 3, 14, 6, 12},
            {7, 8, 12, 15, 1, 5},
            {9, 15, 5, 11, 2, 7},
            {11, 13, 0, 4, 3, 9}
        };
        private ushort[,] IN =
        {
            {0x0036, 0x06C0, 0x6900},
            {0x5048, 0x2106, 0x8411},
            {0x8601, 0x4828, 0x10C4},
            {0x2980, 0x9011, 0x022A}
        };
        private ushort[] OUT =
        {
            0x000F, 0x00F0, 0x3300, 0xCC00
        };
        private ushort[] val = new ushort[96];
        private ushort[] ST = new ushort[65536];
   
        public void KeySetup(BigInteger ZOV)
        {
            for (int i = 0; i < 65536; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    ST[i] |= S[j, ((i >>> SB[j, 0]) & 1)
                          | (((i >>> SB[j, 1]) & 1) << 1)
                          | (((i >>> SB[j, 2]) & 1) << 2)
                          | (((i >>> SB[j, 3]) & 1) << 3)
                          | (((i >>> SB[j, 4]) & 1) << 4)
                          | (((i >>> SB[j, 5]) & 1) << 5)];   
                }
            }

            for (int i = 0; i < 2; i++)
            {
                byte[] K = new byte[8];
                Array.Copy(ZOV.ToByteArray(), 8 * i, K, 0, 8);

                for (int j = 0; j < 32; j++)
                {
                    Block_Encrypt(K);

                    val[3 * j]     ^= (ushort)(K[0] | (K[1] << 8));
                    val[3 * j + 1] ^= (ushort)(K[2] | (K[3] << 8));
                    val[3 * j + 2] ^= (ushort)(K[4] | (K[5] << 8));
                }
            }
        }

        public BigInteger Encrypt(BigInteger ZOV)
        {
            byte[] Data = Block_Encrypt(ZOV.ToByteArray());
            return MessageCrypt.ArrayToNum(Data);
        }

        public BigInteger Decrypt(BigInteger ZOV)
        {
            byte[] Data = Block_Decrypt(ZOV.ToByteArray());
            return MessageCrypt.ArrayToNum(Data);
        }

        private byte[] Block_Encrypt(byte[] BLK)
        {
            ushort r_0, r_1, r_2, r_3, a, b, c;
            byte Z = 0;

            r_0 = (ushort)(BLK[0] | (BLK[1] << 8));
            r_1 = (ushort)(BLK[2] | (BLK[3] << 8));
            r_2 = (ushort)(BLK[4] | (BLK[5] << 8));
            r_3 = (ushort)(BLK[6] | (BLK[7] << 8));
 
            for (int i = 0; i < 8; i++)
            {
                a = (ushort)(r_1 ^ (val[Z++])); b = (ushort)(r_2 ^ (val[Z++])); c = (ushort)(r_3 ^ (val[Z++]));

                r_0 ^= (ushort)
                     ((OUT[0] & ST[(a & IN[0, 0]) | (b & IN[0, 1]) | (c & IN[0, 2])])
                    | (OUT[1] & ST[(a & IN[1, 0]) | (b & IN[1, 1]) | (c & IN[1, 2])])
                    | (OUT[2] & ST[(a & IN[2, 0]) | (b & IN[2, 1]) | (c & IN[2, 2])])
                    | (OUT[3] & ST[(a & IN[3, 0]) | (b & IN[3, 1]) | (c & IN[3, 2])]));

                a = (ushort)(r_2 ^ (val[Z++])); b = (ushort)(r_3 ^ (val[Z++])); c = (ushort)(r_0 ^ (val[Z++]));

                r_1 ^= (ushort)
                     ((OUT[0] & ST[(a & IN[0, 0]) | (b & IN[0, 1]) | (c & IN[0, 2])])
                    | (OUT[1] & ST[(a & IN[1, 0]) | (b & IN[1, 1]) | (c & IN[1, 2])])
                    | (OUT[2] & ST[(a & IN[2, 0]) | (b & IN[2, 1]) | (c & IN[2, 2])])
                    | (OUT[3] & ST[(a & IN[3, 0]) | (b & IN[3, 1]) | (c & IN[3, 2])]));

                a = (ushort)(r_3 ^ (val[Z++])); b = (ushort)(r_0 ^ (val[Z++])); c = (ushort)(r_1 ^ (val[Z++]));

                r_2 ^= (ushort)
                     ((OUT[0] & ST[(a & IN[0, 0]) | (b & IN[0, 1]) | (c & IN[0, 2])])
                    | (OUT[1] & ST[(a & IN[1, 0]) | (b & IN[1, 1]) | (c & IN[1, 2])])
                    | (OUT[2] & ST[(a & IN[2, 0]) | (b & IN[2, 1]) | (c & IN[2, 2])])
                    | (OUT[3] & ST[(a & IN[3, 0]) | (b & IN[3, 1]) | (c & IN[3, 2])]));

                a = (ushort)(r_0 ^ (val[Z++])); b = (ushort)(r_1 ^ (val[Z++])); c = (ushort)(r_2 ^ (val[Z++]));

                r_3 ^= (ushort)
                     ((OUT[0] & ST[(a & IN[0, 0]) | (b & IN[0, 1]) | (c & IN[0, 2])])
                    | (OUT[1] & ST[(a & IN[1, 0]) | (b & IN[1, 1]) | (c & IN[1, 2])])
                    | (OUT[2] & ST[(a & IN[2, 0]) | (b & IN[2, 1]) | (c & IN[2, 2])])
                    | (OUT[3] & ST[(a & IN[3, 0]) | (b & IN[3, 1]) | (c & IN[3, 2])]));
            }

            BLK[0] = (byte)(r_0); BLK[1] = (byte)(r_0 >> 8);
            BLK[2] = (byte)(r_1); BLK[3] = (byte)(r_1 >> 8);
            BLK[4] = (byte)(r_2); BLK[5] = (byte)(r_2 >> 8);
            BLK[6] = (byte)(r_3); BLK[7] = (byte)(r_3 >> 8);

           return BLK;
        }

        private byte[] Block_Decrypt(byte[] BLK)
        {

            ushort r_0, r_1, r_2, r_3, a, b, c;
            byte Z = 96;

            r_0 = (ushort)(BLK[0] | (BLK[1] << 8));
            r_1 = (ushort)(BLK[2] | (BLK[3] << 8));
            r_2 = (ushort)(BLK[4] | (BLK[5] << 8));
            r_3 = (ushort)(BLK[6] | (BLK[7] << 8));

            for (int i = 0; i < 8; i++)
            {
                c = (ushort)(r_2 ^ (val[--Z])); b = (ushort)(r_1 ^ (val[--Z])); a = (ushort)(r_0 ^ (val[--Z]));

                r_3 ^= (ushort)
                     ((OUT[0] & ST[(a & IN[0, 0]) | (b & IN[0, 1]) | (c & IN[0, 2])])
                    | (OUT[1] & ST[(a & IN[1, 0]) | (b & IN[1, 1]) | (c & IN[1, 2])])
                    | (OUT[2] & ST[(a & IN[2, 0]) | (b & IN[2, 1]) | (c & IN[2, 2])])
                    | (OUT[3] & ST[(a & IN[3, 0]) | (b & IN[3, 1]) | (c & IN[3, 2])]));

                c = (ushort)(r_1 ^ (val[--Z])); b = (ushort)(r_0 ^ (val[--Z])); a = (ushort)(r_3 ^ (val[--Z]));

                r_2 ^= (ushort)
                     ((OUT[0] & ST[(a & IN[0, 0]) | (b & IN[0, 1]) | (c & IN[0, 2])])
                    | (OUT[1] & ST[(a & IN[1, 0]) | (b & IN[1, 1]) | (c & IN[1, 2])])
                    | (OUT[2] & ST[(a & IN[2, 0]) | (b & IN[2, 1]) | (c & IN[2, 2])])
                    | (OUT[3] & ST[(a & IN[3, 0]) | (b & IN[3, 1]) | (c & IN[3, 2])]));

                c = (ushort)(r_0 ^ (val[--Z])); b = (ushort)(r_3 ^ (val[--Z])); a = (ushort)(r_2 ^ (val[--Z]));

                r_1 ^= (ushort)
                     ((OUT[0] & ST[(a & IN[0, 0]) | (b & IN[0, 1]) | (c & IN[0, 2])])
                    | (OUT[1] & ST[(a & IN[1, 0]) | (b & IN[1, 1]) | (c & IN[1, 2])])
                    | (OUT[2] & ST[(a & IN[2, 0]) | (b & IN[2, 1]) | (c & IN[2, 2])])
                    | (OUT[3] & ST[(a & IN[3, 0]) | (b & IN[3, 1]) | (c & IN[3, 2])]));

                c = (ushort)(r_3 ^ (val[--Z])); b = (ushort)(r_2 ^ (val[--Z])); a = (ushort)(r_1 ^ (val[--Z]));

                r_0 ^= (ushort)
                     ((OUT[0] & ST[(a & IN[0, 0]) | (b & IN[0, 1]) | (c & IN[0, 2])])
                    | (OUT[1] & ST[(a & IN[1, 0]) | (b & IN[1, 1]) | (c & IN[1, 2])])
                    | (OUT[2] & ST[(a & IN[2, 0]) | (b & IN[2, 1]) | (c & IN[2, 2])])
                    | (OUT[3] & ST[(a & IN[3, 0]) | (b & IN[3, 1]) | (c & IN[3, 2])]));
            }

            BLK[0] = (byte)(r_0); BLK[1] = (byte)(r_0 >> 8);
            BLK[2] = (byte)(r_1); BLK[3] = (byte)(r_1 >> 8);
            BLK[4] = (byte)(r_2); BLK[5] = (byte)(r_2 >> 8);
            BLK[6] = (byte)(r_3); BLK[7] = (byte)(r_3 >> 8);

            return BLK;
        }
    }

    class MessageCrypt
    {
        private byte BlockSize;
        private string AlgMode;
        private string StuffMode;
        private string EncryptMode;
        private List<BigInteger> PlainText  = new List<BigInteger>();
        private List<BigInteger> ClosedText = new List<BigInteger>();

        public MessageCrypt(CryptoModes CryptoModes, string MessageText, BigInteger GeneralKey)
        {
            SetModes(CryptoModes);
            EnStuff(MessageText);
            BasicEncrypt(GeneralKey);
        }

        public MessageCrypt(CryptoModes CryptoModes, List<byte[]> MessageText, BigInteger GeneralKey)
        {
            SetClosedText(MessageText);
            SetModes(CryptoModes);
            BasicDecrypt(GeneralKey);
            DeStuff();
        }

        private void SetModes(CryptoModes CryptoModes)
        {
            AlgMode = CryptoModes.AlgMode;
            StuffMode = CryptoModes.StuffMode;
            EncryptMode = CryptoModes.EncryptMode;
            BlockSize = (AlgMode == "MacGuffin") ? (byte)2 : (AlgMode == "MARS") ? (byte)4 : BlockSize;
        }

        private void EnStuff(string MessageText)
        {
            BigInteger QuadTextBlock = 0;
            byte[] MessageBytes = Encoding.UTF8.GetBytes(MessageText);

            for (int i = 0; i < 4 * ((MessageBytes.Length + (4 * BlockSize - 1)) / (4 * BlockSize)); i++)
            {
                byte[] TextBlock = new byte[BlockSize];

                for (int j = 0; j < BlockSize; j++)
                {
                    if (BlockSize * i + j < MessageBytes.Length)
                    {
                        TextBlock[j] = MessageBytes[BlockSize * i + j];
                    }
                    else
                    {
                        switch (StuffMode)
                        {
                            case "Zeros":

                                for (int k = j; k < BlockSize; k++)
                                {
                                    TextBlock[k] = 0;
                                }

                                break;

                            case "ANSIX.923":

                                for (int k = j; k < BlockSize; k++)
                                {
                                    if (k == BlockSize - 1)
                                    {
                                        TextBlock[k] = (byte)(BlockSize - j);
                                    }
                                    else
                                    {
                                        TextBlock[k] = 0;
                                    }
                                }

                                break;

                            case "PKCS7":

                                for (int k = j; k < BlockSize; k++)
                                {
                                    TextBlock[k] = (byte)(BlockSize - j);
                                }

                                break;

                            case "ISO10126":

                                for (int k = j; k < BlockSize; k++)
                                {
                                    if (k == BlockSize - 1)
                                    {
                                        TextBlock[k] = (byte)(BlockSize - j);
                                    }
                                    else
                                    {
                                        Random random = new Random();
                                        TextBlock[k] = (byte)random.Next(0, 255);
                                    }
                                }

                                break;
                        }

                        break;
                    }
                }

                for (int j = 0 ; j < BlockSize; j++)
                {
                    QuadTextBlock <<= 8;
                    QuadTextBlock |= TextBlock[j];
                }

                if ((i + 1) % 4 == 0)
                {
                    PlainText.Add(QuadTextBlock);
                    QuadTextBlock = 0;
                }
            }
        }

        private void DeStuff()
        {
            int count = PlainText.Count - 1;
            {
                BigInteger QuadTextBlock = PlainText[count];

                if (StuffMode == "Zeros")
                {
                    while (true)
                    {
                        if ((byte)(QuadTextBlock & 0xFF) == 0)
                        {
                            QuadTextBlock >>>= 8;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < 4; j++)
                    {
                        byte[] TextBlock = BitConverter.GetBytes((uint)(QuadTextBlock & 0xFFFFFFFF));

                        if (TextBlock[0] < 5)
                        {
                            QuadTextBlock >>>= 8 * TextBlock[0];
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                PlainText[count] = QuadTextBlock;
            }
        }

        private void BasicEncrypt(BigInteger GeneralKey)
        {
            BigInteger IV = 0;

            if (EncryptMode != "ECB")
            {
                IV = P_D_H.GenNum();

                if (AlgMode == "MacGuffin")
                {
                    IV >>>= 64;
                }

                ClosedText.Add(IV);
            }

            Type Type = Type.GetType("CRINGEGRAM." + AlgMode);
            Cryptography Cryptography = (Cryptography)Activator.CreateInstance(Type);
            Cryptography.KeySetup(GeneralKey);

            switch (EncryptMode)
            {
                case "ECB":

                    foreach (var QuadTextBlock in PlainText)
                    {
                        ClosedText.Add(Cryptography.Encrypt(QuadTextBlock));    
                    }

                    break;

                case "CBC":

                    foreach (var QuadTextBlock in PlainText)
                    {
                        IV = Cryptography.Encrypt(IV ^ QuadTextBlock);
                        ClosedText.Add(IV);
                    }

                    break;

                case "PCBC":

                    foreach (var QuadTextBlock in PlainText)
                    {
                        IV = Cryptography.Encrypt(IV ^ QuadTextBlock);
                        ClosedText.Add(IV);
                        IV ^= QuadTextBlock;
                    }

                    break;

                case "CFB":

                    foreach (var QuadTextBlock in PlainText)
                    {
                        IV = QuadTextBlock ^ Cryptography.Encrypt(IV);
                        ClosedText.Add(IV);
                    }

                    break;

                case "OFB":

                    foreach (var QuadTextBlock in PlainText)
                    {
                        IV = Cryptography.Encrypt(IV);
                        ClosedText.Add(IV ^ QuadTextBlock);
                    }

                    break;

                case "CTR":

                    foreach (var QuadTextBlock in PlainText)
                    {
                        ClosedText.Add(Cryptography.Encrypt(IV++) ^ QuadTextBlock);
                    }

                    break;

                case "RD":

                    foreach (var QuadTextBlock in PlainText)
                    {
                        ClosedText.Add(Cryptography.Encrypt(IV ^ QuadTextBlock));
                        IV = IV + ((IV << 64) >>> 64);
                    }

                    break;
            }
        }

        private void BasicDecrypt(BigInteger GeneralKey)
        {
            BigInteger IV = 0;

            if (EncryptMode != "ECB")
            {
                IV = ClosedText[0];
                ClosedText.RemoveAt(0);
            }

            Type Type = Type.GetType("CRINGEGRAM." + AlgMode);
            Cryptography Cryptography = (Cryptography)Activator.CreateInstance(Type);
            Cryptography.KeySetup(GeneralKey);

            switch (EncryptMode)
            {
                case "ECB":

                    foreach (var QuadTextBlock in ClosedText)
                    {
                        PlainText.Add(Cryptography.Decrypt(QuadTextBlock));
                    }

                    break;

                case "CBC":

                    foreach (var QuadTextBlock in ClosedText)
                    {
                        PlainText.Add(IV ^ Cryptography.Decrypt(QuadTextBlock));
                        IV = QuadTextBlock; 
                    }

                    break;

                case "PCBC":

                    foreach (var QuadTextBlock in ClosedText)
                    {
                        BigInteger P = IV ^ Cryptography.Decrypt(QuadTextBlock);
                        PlainText.Add(P);
                        IV = QuadTextBlock ^ P;
                    }

                    break;

                case "CFB":

                    foreach (var QuadTextBlock in ClosedText)
                    {
                        PlainText.Add(QuadTextBlock ^ Cryptography.Encrypt(IV));
                        IV = QuadTextBlock;
                    }

                    break;

                case "OFB":

                    foreach (var QuadTextBlock in ClosedText)
                    {
                        IV = Cryptography.Encrypt(IV);
                        PlainText.Add(IV ^ QuadTextBlock);
                    }

                    break;

                case "CTR":

                    foreach (var QuadTextBlock in ClosedText)
                    {
                        PlainText.Add(Cryptography.Encrypt(IV++) ^ QuadTextBlock);
                    }

                    break;

                case "RD":

                    foreach (var QuadTextBlock in ClosedText)
                    {
                        PlainText.Add(IV ^ Cryptography.Decrypt(QuadTextBlock));
                        IV = IV + ((IV << 64) >>> 64);
                    }

                    break;
            }
        }
        
        public static T[] NumToArray<T>(BigInteger ZOV) where T : struct
        {
            T[] result = new T[4];
            byte size = (byte)(((ZOV.GetByteCount() + 2) >>> 2) << 3);

            if (size == 32)
            {
                for (int i = 0; i < 4; i++)
                {
                    result[i] = (T)Convert.ChangeType((uint)(ZOV & (uint)(Math.Pow(2, size) - 1)), typeof(T));
                    ZOV >>= size;
                }
            }
            else if (size == 16)
            {
                for (int i = 0; i < 4; i++)
                {
                    result[i] = (T)Convert.ChangeType((ushort)(ZOV & (ushort)(Math.Pow(2, size) - 1)), typeof(T));
                    ZOV >>= size;
                }
            }

            return result;
        }

        public static BigInteger ArrayToNum(object ZOV)
        {
            BigInteger result = 0;

            if (ZOV is uint[] Z)
            {
                for (int i = 3; i >= 0; i--)
                {
                    result <<= 32;
                    result |= Z[i];
                }
            }
            else if (ZOV is byte[] V)
            {
                for (int i = 7; i >= 0; i--)
                {
                    result <<= 8;
                    result |= V[i];
                }
            }

            return result;
        }

        public List<byte[]>GetClosedText()
        {
            List<byte[]> ZOV = new List<byte[]>();

            for (int i = 0; i < ClosedText.Count; i++)
            {
                ZOV.Add(ClosedText[i].ToByteArray());
            }

            return ZOV;
        }

        public string GetPlainText()
        {
            List<byte> ZOV = new List<byte>();

            for (int i = 0; i < PlainText.Count; i++)
            {
                byte[] Z = PlainText[i].ToByteArray();

                for (int j = Z.Length - 1; j >= 0; j--)
                {
                    if (Z[j] != 0)
                    {
                        ZOV.Add(Z[j]);
                    }
                }
            }

            return Encoding.UTF8.GetString(ZOV.ToArray());
        }

        private void SetClosedText(List<byte[]> ZOV)
        {
            foreach (byte[] Z in ZOV)
            {
                ClosedText.Add(new BigInteger(Z));
            }
        }
    }
}
