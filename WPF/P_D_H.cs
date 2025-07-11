using CRINGEGRAM.Models;
using System;
using System.Numerics;

namespace CRINGEGRAM
{
    public class P_D_H
    {
        private byte G;
        private BigInteger P;
        private BigInteger PublicKey;
        private BigInteger SecretKey;
        private BigInteger GeneralKey;

        public P_D_H()
        {
            Random Random = new Random();
            G = (byte)Random.Next(2, 7);

            do
            {
                P = GenNum();
            }
            while (!T_M_R());

            SecretKey = GenNum();
            PublicKey = PowWithMod(G, SecretKey, P);
        }

        public P_D_H(CryptoData CryptoData)
        {
            SecretKey = GenNum();
            PublicKey = PowWithMod(CryptoData.G, SecretKey, new BigInteger(CryptoData.P));
            GeneralKey = PowWithMod(new BigInteger(CryptoData.PublicKey), SecretKey, new BigInteger(CryptoData.P));
        }

        public static BigInteger GenNum()
        {
            BigInteger Num = 1;

            for (int i = 0; i < 16; i++)
            {
                Random Random = new Random();
                byte ez = (byte)Random.Next(0, 256);

                Num = Num << 8;

                if (i == 0)
                {
                    Num = Num >>> 1;
                }

                Num |= ez;
            }

            return Num;
        }

        public static BigInteger PowWithMod(BigInteger G, BigInteger a, BigInteger P)
        {
            BigInteger Result = 1;
            BigInteger Value = G % P;

            while (a > 0)
            {
                if (a % 2 == 1)
                {
                    Result = Result * Value % P;
                }

                Value = Value * Value % P;
                a /= 2;
            }

            return Result;
        }

        private bool T_M_R()
        {
            BigInteger t = P - 1;
            int s = 0;

            while (t % 2 == 0)
            {
                t /= 2;
                s++;
            }

            Random Random = new Random();

            for (int i = 0; i < 50; i++)
            {
                BigInteger a = Random.Next(2, 0x7FFFFFFF);
                BigInteger x = PowWithMod(a, t, P);
                if (x == 1 || x == P - 1) continue;

                for (int j = 1; j < s; j++)
                {
                    x = PowWithMod(x, 2, P);
                    if (x == 1) return false;
                    if (x == P - 1) break;
                }

                if (x != P - 1) return false;
            }

            return true;
        }

        public CryptoData GetCryptoData()
        {
            var CryptoData = new CryptoData
            {
                G = G,
                P = P.ToByteArray(),
                PublicKey = PublicKey.ToByteArray()
            };

            return CryptoData;
        }

        public BigInteger GetSecretKey()
        {
            return SecretKey;
        }

        public byte[] GetPublicKey()
        {
            return PublicKey.ToByteArray();
        }

        public BigInteger GetGeneralKey()
        {
            return GeneralKey;
        }
    }
}
