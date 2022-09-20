using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers
{
    public static class PrimeNumbers
    {
        public static List<int> Primes(int upTo) => Enumerable.Range(2, upTo + 1).Where(n => IsPrime(n)).ToList();

        public static bool IsPrime(int n) => !Enumerable.Range(2, n / 2).Any(f => IsFactor(f, n));

        public static bool IsFactor(int f, int ofN) => ofN % f == 0;


    }
}
