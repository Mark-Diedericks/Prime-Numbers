using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Primes_One
{
    class Program
    {
        //10m: 611ms vs 613ms
        //100m: 6.715s vs 6.795s
        static void Main(string[] args)
        {
            Stopwatch s = new Stopwatch();
            Console.WriteLine("Insert your upperbound");
            string ubs = Console.ReadLine();
            s.Start();

            try
            {
                long ub = (long)Convert.ToDecimal(ubs);
                long ubsqrt = (long)Math.Ceiling(Sqrt((decimal)ub));

                long MEM_SIZE = 2 * (1024 * 1024);
                long SEG_SIZE = (long)Math.Max(Sqrt((decimal)ub), (decimal)MEM_SIZE);

                Console.WriteLine("Upper Bound SquareRoot: " + ubsqrt + ", Upper Bound Set To: " + ub);
                
                HugeArray<byte> hs = new HugeArray<byte>((long)Math.Ceiling(((decimal)ubsqrt + 1.0m) / 8.0m));
                long pc = (long)((ub < 2) ? 0 : 1); //If it is less than two, then 0 primes

                hs[0] = Set(hs[0], 0, true);
                hs[0] = Set(hs[0], 1, true);

                //Calculate primes below upper bound sqrt
                for (long i = 2; i <= ubsqrt; i++)
                {
                    long pi = (long)Math.Floor((decimal)i / 8.0m);

                    if (Get(hs[pi], (int)(i % 8)) == false)
                    {
                        Parallel.Invoke(() => {
                            for (long j = i * i; j <= ubsqrt; j += i)
                            {
                                long pj = (long)Math.Floor((decimal)j / 8.0m);
                                if (Get(hs[pj], (int)(j % 8)) == false)
                                {
                                    hs[pj] = Set(hs[pj], (int)(j % 8), true);
                                }
                            }
                        });
                    }
                }

                HugeArray<byte> ss;
                long[] primes = new long[0];
                long[] next = new long[0]; ;
                long si = 3;
                long ni = 3;

                Parallel.ForEach(BetterEnumerable.SteppedRange(0, ub, SEG_SIZE), low => {
                    ss = new HugeArray<byte>((long)Math.Ceiling((decimal)SEG_SIZE / 8.0m));
                    long high = (long)Math.Min((decimal)low + (decimal)SEG_SIZE, (decimal)ub);

                    for(; si*si <= high; si += 2)
                    {
                        long psi = (long)Math.Floor((decimal)si / 8.0m);
                        if (Get(hs[psi], (int)(si % 8)) == false)
                        {
                            Add(ref primes, si);
                            Add(ref next, (si * si - low));
                        }
                    }

                    for(long i = 0; i < primes.Count(); i++)
                    {
                        long j = next[i];

                        for (long k = primes[i] * 2; j < SEG_SIZE; j += k)
                        {
                            long pj = (long)Math.Floor((decimal)j / 8.0m);
                            ss[pj] = Set(ss[pj], (int)(j % 8), true);
                        }

                        next[i] = j - SEG_SIZE;
                    }

                    for(; ni <= high; ni += 2)
                    {
                        long pni = (long)Math.Floor((decimal)(ni - low) / 8.0m);
                        if(Get(ss[pni], (int)((ni - low) % 8)) == false)
                        {
                            pc++;
                        }
                    }
                });

                    /*Task t = Task.Factory.StartNew(() => {
                        for (long i = 2; i < ubsqrt; i++)
                        {
                            long pi = (long)Math.Floor((decimal)i / 8.0m);

                            if (Get(hs[pi], (int)(i % 8)) == false)
                            {
                                Parallel.Invoke(() => {
                                    for (long j = i * i; j < ub; j += i)
                                    {
                                        long pj = (long)Math.Floor((decimal)j / 8.0m);
                                        if (Get(hs[pj], (int)(j % 8)) == false)
                                        {
                                            hs[pj] = Set(hs[pj], (int)(j % 8), true);
                                            pc--;
                                        }
                                    }
                                });
                            }
                        }
                    });

                    t.Wait();
                t.Dispose();*/

                hs = null;
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

        public static void Add(ref long[] arr, long val)
        {
            long[] narr = new long[arr.Length + 1];
            arr.CopyTo(narr, 1);
            narr[narr.Length - 1] = val;
            arr = narr;
        }

        public static byte Set(byte aByte, int pos, bool value)
        {
            if (value)
            {
                //left-shift 1, then bitwise OR
                aByte = (byte)(aByte | (1 << pos));
            }
            else
            {
                //left-shift 1, then take complement, then bitwise AND
                aByte = (byte)(aByte & ~(1 << pos));
            }

            return aByte;
        }

        public static bool Get(byte aByte, int pos)
        {
            //left-shift 1, then bitwise AND, then check for non-zero
            return ((aByte & (1 << pos)) != 0);
        }

        //https://stackoverflow.com/questions/4124189/performing-math-operations-on-decimal-datatype-in-c
        // x - a number, from which we need to calculate the square root
        // epsilon - an accuracy of calculation of the root from our number.
        // The result of the calculations will differ from an actual value
        // of the root on less than epslion.
        public static decimal Sqrt(decimal x, decimal epsilon = 0.0M)
        {
            if (x < 0) throw new OverflowException("Cannot calculate square root from a negative number");

            decimal current = (decimal)Math.Sqrt((double)x), previous;
            do
            {
                previous = current;
                if (previous == 0.0M) return 0;
                current = (previous + x / previous) / 2;
            }
            while (Math.Abs(previous - current) > epsilon);
            return current;
        }
    }

    //https://stackoverflow.com/questions/7142446/parallel-for-step-size/7142469
    public static class BetterEnumerable
    {
        public static IEnumerable<long> SteppedRange(long fromInclusive, long toExclusive, long step)
        {
            for (var i = fromInclusive; i < toExclusive; i += step)
            {
                yield return i;
            }
        }
    }
}
