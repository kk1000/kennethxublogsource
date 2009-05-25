using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace InterlockVsMonitor
{
    class ThreadingHelper
    {
        public static long RecordElapsed(Action action)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        public static void RunWorksInParallel(ICollection<Action> works)
        {
            int workCount = works.Count;
            Barrier barrier = new Barrier(workCount);

            List<Thread> threads = new List<Thread>(workCount);

            foreach (Action work in works)
            {
                Action action = work;
                var t = new Thread(new ThreadStart(
                                       delegate
                                           {
                                               barrier.Wait();
                                               action();
                                           }));
                threads.Add(t);
                t.Name = String.Format("Thread {0} of {1}", threads.Count, workCount);
                t.Start();
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }
    }
}
