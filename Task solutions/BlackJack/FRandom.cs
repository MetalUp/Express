using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foo
{
    public class FRandom
    {
        private readonly uint U;
        private readonly uint V;

        private FRandom(uint u, uint v)
        {
            U = u;
            V = v;
        }

        /// <summary>
        /// Produces pseudo-random number from two specified non-zero seed values.
        /// If either value is zero, a default will be used instead.
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static FRandom Seed(uint u = 521288629, uint v = 362436069)
        {
            return new FRandom(u, v);
        }

        /// <summary>
        /// Seed the random generator using a long e.g. a DateTime.ToFileTime()
        /// </summary>
        public static FRandom Seed(long l)
        {
            return Seed((uint)(l >> 16), (uint)(l % 4294967296));
        }

        public double InRange0to1()
        {
            return ((NewU(U) << 16) + NewV(V) + 1.0) * 2.328306435454494e-10;
        }

        public int InRange(int minValue, int maxValue)
        {
            return (int)(minValue + InRange0to1() * (maxValue - minValue));
        }

        public FRandom Next() => new FRandom(NewU(U), NewV(U));

        private static uint NewU(uint u) => 36969 * (u & 65535) + (u >> 16);

        private static uint NewV(uint v) => 18000 * (v & 65535) + (v >> 16);

        public override string ToString() => InRange0to1().ToString();

        public static List<FRandom> CreateList(int length, FRandom seed) => ExtendList(new List<FRandom>(), length, seed);
        private static List<FRandom> ExtendList(List<FRandom> list, int n, FRandom r) => n == 0 ? list : ExtendList(list.Append(r).ToList(), n - 1, r.Next());

        public FRandom Skip(int n) => n == 0 ? this : Next().Skip(n - 1);
    }
}
