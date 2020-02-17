using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primes_Parallel
{
    public class SmallPrime
    {
        public readonly ulong first;
        public readonly ulong last;
        public readonly string str;

        public SmallPrime(ulong f, ulong l, string s)
        {
            first = f;
            last = l;
            str = s;
        }
    }

    public class PrimeSieve
    {

        readonly SmallPrime[] smallPrimes = 
        {
        new SmallPrime( 2,  2, "2"),
        new SmallPrime( 3,  3, "3" ),
        new SmallPrime( 5,  5, "5" ),
        };

        private readonly uint sieve_size;
        private ulong start = 0;
        private ulong stop = 0;

        public PrimeSieve(uint sieve_size)
        {
            this.sieve_size = Math.Min(Math.Max(sieve_size, 1), 2048);
        }

        public ulong CountPrimes(ulong start, ulong stop)
        {
            ulong count = 0;
            this.start = start;
            this.stop = stop;

            foreach (SmallPrime s in smallPrimes)
                if (s.first > start && s.last < stop)
                    count++;

            if(stop >= 7)
            {
                PrimeGenerator pg = new PrimeGenerator(this);

                if(pg.getSqrtStop() > getLimit())
                {
                    SievingPrimes sp = new SievingPrimes(pg, ps);
                    sp.Generate();
                }

                count += pg.Sieve();
            }

            return count;
        }

        public ulong getLimit()
        {
            return UInt64.MaxValue;
        }

        public ulong getStart()
        {
            return start;
        }

        public ulong getStop()
        {
            return stop;
        }
    }
}
