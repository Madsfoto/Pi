using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;


namespace Pi
{
    class Program
    {
        

        static void Main(string[] args)
        {
            ConcurrentBag<string> cb = new ConcurrentBag<string>();
            ulong denomMaxTotal = 5871781006564002450;
            decimal pi = 3.14159265359M;

            void doCalc(decimal d1, decimal accuracy)
            {
                //Console.WriteLine(d1);
                ulong denomMin = (ulong)Math.Floor(d1 / 4M);
                //Console.WriteLine(denomMin);
                ulong denomMax = (ulong)Math.Ceiling(d1 / 3M);
                //Console.WriteLine(denomMax);
                long distance = Math.Abs((long)denomMin - (long)denomMax);
                //Console.WriteLine(distance);


                for (ulong denominator = denomMin; denominator < denomMax; denominator++)
                {

                    decimal fraction = (decimal)d1 / denominator;
                  //Console.WriteLine(fraction);

                    if (Math.Abs((fraction - pi)) < accuracy)
                    {
                        //Console.WriteLine(fraction);
                        cb.Add(d1+";"+denominator);
                        
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

            Parallel.For(20, 360000, nominator =>
            {
                doCalc(nominator, 0.0000001M);
                
            });

            // show items in cb
            Console.WriteLine("Items in cb: "+cb.Count);

            while (cb.Count > 0)
            {
                string bagElement;
                bool success = cb.TryTake(out bagElement);
                if (success)
                {
                    Console.WriteLine(bagElement);
                }
            }



        }
    }
}
