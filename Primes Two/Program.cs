using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primes_Two
{
    class Program
    {
        //10m in 1s at 1mb
        static void Main(string[] args)
        {
            Stopwatch s = new Stopwatch();
            Console.WriteLine("Insert your upperbound");
            string ubs = Console.ReadLine();
            s.Start();

            try
            {
                UInt128 ub = (UInt128)Convert.ToDecimal(ubs);
                UInt128 pc = ub;
                pc--; //2 is the first prime, we start at 3

                Parallel.ForEach(BetterEnumerable.SteppedRange(3, ((UInt128)Math.Ceiling((decimal)ub / 1000000) * 1000000), 1000000), si => {
                    for (UInt128 i = si; (i < (si + 1000000)) && (i < ub); i += 2)
                    {
                        if((i % 5 != 0) || (i == 5))
                        {
                            UInt128 max = (UInt128)Math.Ceiling(Sqrt((decimal)i));
                            for(UInt128 j = 3; j <= max; j += 2)
                            {
                                if(i % j == 0)
                                {
                                    pc--;
                                    break;
                                }
                            }
                        }
                    }
                });

                Console.WriteLine("Primes: " + pc);
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            GC.Collect();

            s.Stop();
            Console.WriteLine("Duration: " + (s.ElapsedMilliseconds / 1000.0) + "s");
            string str = Console.ReadLine();
            Console.WriteLine(str);
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
        public static IEnumerable<UInt128> SteppedRange(UInt128 fromInclusive, UInt128 toExclusive, UInt128 step)
        {
            for (var i = fromInclusive; i < toExclusive; i += step)
            {
                yield return i;
            }
        }
    }
}
