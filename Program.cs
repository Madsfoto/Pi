using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Globalization;


namespace Pi
{
    class Program
    {
        static void ShowUpdate()
        {
            Interlocked.Increment(ref number);
            if (number % 100000 == 0)
            {

                Console.WriteLine(number.ToString("N1", CultureInfo.InvariantCulture));
                
            }

        }
        static ulong number = 0;

        static void Main(string[] args)
        {
            
            ConcurrentBag<string> cb = new();
           
            
            void doCalc(decimal start_nominator, decimal accuracy)
            {
                //Console.WriteLine(d1);
                ulong denomMin = (ulong)Math.Floor(start_nominator / (decimal)Math.PI); 
                //Console.WriteLine(denomMin);
                ulong denomMax = (ulong)Math.Ceiling(start_nominator / (decimal)Math.PI);
                //Console.WriteLine(denomMax);
                
                
                for (decimal denominator = denomMin; denominator < denomMax; denominator++)
                {

                    decimal fraction = decimal.Divide(start_nominator, denominator);

                    //Console.WriteLine(fraction);

                    decimal absDiff = Math.Abs(fraction - (decimal)Math.PI);
                    if (absDiff < accuracy)
                    {
                        //Console.WriteLine(fraction);
                        cb.Add(start_nominator+";"+denominator+";"+fraction+";"+absDiff);
                        
                        //Console.WriteLine("Nominator = " + d1 + ". Denominator = " + denominator);

                    }

                }
            }

            // the idea:
            /* Pi is 3.1415
             * which means that 22/7 is close. 
             * I want the next numbers that are close
             * so I want the nominator and denominator to increase, 
             * with the nominator starting at 3.1*denominator (rounded down) and ending at 3.2*denominator (rounded up)
             * 
             * Step 1: For()
             * Step 2: Parallel.for()
             * 
             */




            #region singleThread
            // nominator
            //for (ulong nominator = 350; nominator < 123456360; nominator++)
            //{

            //    ulong denomMin = (ulong)Math.Floor(nominator / 4M);
            //    //Console.WriteLine(denomMin);
            //    ulong denomMax = (ulong)Math.Ceiling(nominator / 3M);
            //    //Console.WriteLine(denomMax);

            //    long distance = Math.Abs((long)denomMin - (long)denomMax);
            //    //Console.WriteLine(distance);

            //    // denominator
            //    for (ulong denominator = denomMin; denominator < denomMax; denominator++)
            //    {

            //            decimal fraction = (decimal)nominator / denominator;
            //            //Console.WriteLine(fraction);

            //            if (Math.Abs((fraction - pi)) < 0.0000001M)
            //            {
            //                Console.WriteLine(fraction);
            //                //addNumbersToList(nominator, denominator);
            //                Console.WriteLine("Nominator = " + nominator + ". Denominator = " + denominator);

            //            }





            //    }


            //}
            //Console.ReadKey(); 
            #endregion
            // constants: 
            long start = 1;
            long end = 10;

            if (args.Length==0)
            {
                start = 5000000000;
                end = 7500000000;


              
            }
            
            else if(args.Length==2)
            {
                start = Int64.Parse(args[0]);
                end = Int64.Parse(args[1]);
            }
            

            
            decimal precision = 0.000000000000000001M;
            

            Stopwatch sw = new();
            sw.Start();

            
            Parallel.For(start, end, nominator =>
            {
                doCalc(nominator, precision);
                ShowUpdate();
                
            });

            sw.Stop();
            
            // show items in cb
            Console.WriteLine("Items in cb: "+cb.Count);
            
            Console.WriteLine("Time elapsed: {0}", sw.Elapsed);
            long ms = sw.ElapsedMilliseconds;
            long mysPerNom = (ms*1000) / (end - start);
            Console.WriteLine("Micro seconds elapsed per nominator: {0}", mysPerNom);

            using StreamWriter file = new("Num_Denom.txt", true);
            if(File.Exists("Num_Denom.txt"))
            {

            }
            else
            {
                file.WriteLine("Starting at " + start + " and ending at " + end + " with precision of " + precision);
                file.WriteLine("Nominator;denominator;fractionResult;absDiff");

            }
            while (!cb.IsEmpty)
            {

                bool success = cb.TryTake(out string bagElement);
                if (success)
                {
                    file.WriteLine(bagElement);
                }

            }



            file.WriteLine();



        }
    }
}
