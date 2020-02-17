using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCL;
using System.Diagnostics;

namespace Primes_GPU
{
    class Program
    {
        static void Main(string[] args)
        {
            completePrimes();
        }

        private static async void completePrimes()
        {
            EasyCL context = new EasyCL();
            context.InvokeAborted += Context_InvokeAborted;

            if (AcceleratorDevice.HasGPU) context.Accelerator = AcceleratorDevice.GPU;
            else Environment.Exit(1);

            Stopwatch s = new Stopwatch();
            Console.WriteLine("Insert your upperbound");
            string ubs = Console.ReadLine();
            s.Start();

            try
            {
                int ub = Convert.ToInt32(ubs);

                long pc = 0;
                int[] primes = Enumerable.Range(2, ub).ToArray();

                context.LoadKernel(PrimeNumbers);
                await context.InvokeAsync("GetIfPrime", primes.Length, primes);

                foreach (byte b in primes)
                    if (b != 0)
                        pc++;

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

        private static void Context_InvokeAborted(object sender, string e)
        {
            Console.WriteLine("Failed: " + e);
            string str = Console.ReadLine();
            Console.WriteLine(str);
            Environment.Exit(1);
        }

        private static void Context_ProgressChangedEvent(object sender, double e)
        {
            Console.WriteLine(e.ToString("0.00%"));
        }

        private static string PrimeNumbers
        {
            get
            {
                return @"
        kernel void GetIfPrime(global int* message)
        {
            int index = get_global_id(0);

            int upperl=(int)sqrt((float)message[index]);
            for(int i=2;i<=upperl;i++)
            {
                if(message[index]%i==0)
                {
                    //printf("" %d / %d\n"",index,i );
                    message[index]=0;
                    return;
                }
            }
            //printf("" % d"",index);
        }";
            }
        }
    }
}
