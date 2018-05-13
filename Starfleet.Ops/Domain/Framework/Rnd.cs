using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Starfleet.Ops.Domain.Framework
{
    public static class DiceRoller
    {
        public static int Roll()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] singleByteBuf = new byte[1];
            int max = Byte.MaxValue - Byte.MaxValue % 6;
            while (true)
            {
                rng.GetBytes(singleByteBuf);
                int b = singleByteBuf[0];
                if (b < max)
                {
                    return b % 6 + 1;
                }
            }
        }
    }
}
