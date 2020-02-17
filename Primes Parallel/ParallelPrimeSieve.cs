using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primes_Parallel
{
    public class ParallelPrimeSieve
    {
        private readonly uint sieve_size;
        private readonly uint thread_count;

        public ParallelPrimeSieve(uint sieve_size)
        {
            this.sieve_size = Math.Min(Math.Max(sieve_size, 1), 2048);
            this.thread_count = Math.Max(1, getMaxThreads());
        }

        public ParallelPrimeSieve(uint sieve_size, uint thread_count)
        {
            this.sieve_size = Math.Min(Math.Max(sieve_size, 1), 2048);
            this.thread_count = Math.Min(Math.Max(thread_count, 1), getMaxThreads());
        }

        private uint getMaxThreads()
        {
            return 0; //TODO
        }

        public ulong CountPrimes(ulong ub)
        {
            if(thread_count == 1)
            {
                return new PrimeSieve(sieve_size).CountPrimes(1, ub);
            }
            else
            {
                return 0;
            }
        }
    }
}
