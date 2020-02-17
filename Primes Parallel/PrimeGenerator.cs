using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primes_Parallel
{
    public class PrimeGenerator
    {
        private readonly PrimeSieve primeSieve;
        private ulong count = 0;

        public PrimeGenerator(PrimeSieve primeSieve)
        {
            this.primeSieve = primeSieve;
        }
    
        public void Sieve(ref HugeArray<ulong> sieve, uint sieve_size)
        {
            count = 0;

            while(segmentHigh < primeSieve.getStop())
                sieveSegment();
        }

        public ulong getSqrtStop()
        {
            return (ulong)Math.Ceiling(Math.Sqrt(primeSieve.getStop()));
        }

    }
}
