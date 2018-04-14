using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Loops
{
    public class ForForeachLoops
    {
        /// <summary>
        /// The For loop that's blocks would be done parallel.
        /// </summary>
        /// <param name="fromInclusive">The Start index.</param>
        /// <param name="toExclusive">The End index.</param>
        /// <param name="body">Body of the loop.</param>
        public static void ParallelFor(int fromInclusive, int toExclusive, Action<int> body)
        {
            if(fromInclusive > toExclusive || body == null)
            {
                throw new Exception("You give either wrong indzexes, or there isn't body.");
            }
            // We want to create max 10 tasks.
            int max = toExclusive - fromInclusive;
            if (max > 10)
            {
                max = 10;
            }

            var tasks = new Task[max];
            for(int i = fromInclusive; i <= toExclusive; i++)
            {
                var iondexOfArray = i;
                if (iondexOfArray < max)
                {
                    tasks[i] = new Task(() => body(iondexOfArray));
                    tasks[i].Start();
                }
                else
                {
                    var indexOfTask = Task.WaitAny(tasks);
                    tasks[indexOfTask] = new Task(() => body(iondexOfArray));
                    tasks[indexOfTask].Start();
                }
            }
            Task.WaitAll(tasks);
        }

        /// <summary>
        /// The ForEach loop that's blocks would be done parallel.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="body"></param>
        public static void ParallelForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            if (source == null || body == null)
            {
                throw new Exception("Either source is null, or body");
            }
            int countOfWorkingBlocks = 0;
            int max = source.Count();
            if (max > 10)
            {
                max = 10;
            }
            var elem = new Task[max];
            foreach (var element in source)
            {
                
                var indexOfElement = countOfWorkingBlocks;
                countOfWorkingBlocks++;
                var current = element;
                if(indexOfElement >= max)
                {
                    var indexofTask = Task.WaitAny(elem);
                    elem[indexofTask] = new Task(() => body(current));
                    elem[indexofTask].Start();
                }
                else
                {
                    elem[indexOfElement] = new Task(() => body(current));
                    elem[indexOfElement].Start();
                }

            }
            Task.WaitAll(elem);
        }

        /// <summary>
        /// Method ForEach Parallel with Options.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="parallelOptions"></param>
        /// <param name="body"></param>
        public static void ParallelForEachWithOptions<TSource>(IEnumerable<TSource> source, ParallelOptions parallelOptions, Action<TSource> body)
        {
            if(source == null)
            {
                throw new Exception("Source is null.");
            }
            if (parallelOptions == null)
            {
                throw new Exception("parallelOptions is null.");
            }
            if (body == null)
            {
                throw new Exception("Body is null");
            }

            int countOfWorkingBlocks = 0;
            int max = source.Count();
            if (max > parallelOptions.MaxDegreeOfParallelism)
            {
                max = parallelOptions.MaxDegreeOfParallelism;
            }

            TaskFactory taskFactory = new TaskFactory(parallelOptions.CancellationToken, TaskCreationOptions.None, TaskContinuationOptions.None, parallelOptions.TaskScheduler);
            var elem = new Task[max];
            foreach (var element in source)
            {

                var indexOfElement = countOfWorkingBlocks;
                countOfWorkingBlocks++;
                var current = element;
                if (indexOfElement >= max)
                {
                    var indexofTask = Task.WaitAny(elem);
                    elem[indexofTask] = taskFactory.StartNew(() => body(current), parallelOptions.CancellationToken);
                }
                else
                {
                    elem[indexOfElement] = new Task(() => body(current));
                    elem[indexOfElement].Start();
                }
                
            }
        }


















    }
}
