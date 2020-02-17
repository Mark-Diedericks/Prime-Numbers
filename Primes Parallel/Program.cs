using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primes_Parallel
{
    class Program
    {
        public static void Main(string[] args)
        {
            Stopwatch s = new Stopwatch();
            Console.WriteLine("Insert your upperbound");
            string ubs = Console.ReadLine();
            s.Start();

            try
            {
                const uint sieve_size = 2048; //max 2048
                const uint thread_count = 4;

                ulong ub = (ulong)Convert.ToDecimal(ubs);

                ParallelPrimeSieve pps = new ParallelPrimeSieve(sieve_size, thread_count);
                ulong pc = pps.CountPrimes(ub);

                Console.WriteLine("Primes: " + pc);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            GC.Collect();

            s.Stop();
            Console.WriteLine("Duration: " + (s.ElapsedMilliseconds / 1000.0) + "s");
            string str = Console.ReadLine();
            Console.WriteLine(str);
        }
}
