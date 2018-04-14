using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Loops;

namespace ParallelLoops
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            List<int> list = new List<int>();
            int[] numbers = new int[20];

            for (int i = 0; i < 20; i++)
            {
                list.Add(rnd.Next(1, 200));
                numbers[i] = i;
                Console.WriteLine(list.Last());
            }

            ForForeachLoops.ParallelFor(0, 19, num => Console.WriteLine(numbers[num]));
            Console.WriteLine();
            ForForeachLoops.ParallelForEach<int>(list, elem => Console.WriteLine(list.IndexOf(elem) + " - " + elem));

            ParallelOptions parallelOptions = new ParallelOptions();
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            parallelOptions.MaxDegreeOfParallelism = 20;
            parallelOptions.CancellationToken = token;

            ForForeachLoops.ParallelForEachWithOptions<int>(list, parallelOptions, (elem) =>Function(list,elem,token));

            Console.WriteLine("Enter 'c' if you want cancel a program.");
            var el=Console.ReadLine();
            if(el=="c" || el == "C")
            {
                tokenSource.Cancel();
            }
            Console.ReadLine();
            
        }

        public static void Function(List<int> list,int elem,CancellationToken token)
        {
            if (token.IsCancellationRequested == true)
            {
                Console.WriteLine("Task was cancelled before it got started.");
                token.ThrowIfCancellationRequested();
            }
            else
            {
                Console.WriteLine(list.IndexOf(elem) + " - " + elem);
                Thread.Sleep(1000);
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Task cancelled");
                    token.ThrowIfCancellationRequested();
                }

            }
        }
    }
}
