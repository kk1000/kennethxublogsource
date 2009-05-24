using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace InterlockVsMonitor
{
    class Program
    {
        private static int loop = 1000000;
        private IAtomic _atomic;
        private static Accumulator accumulator;
        
        static void Main(string[] args)
        {
            int threadCount = 1;
            if (args.Length > 0)
            {
                try
                {
                    threadCount = int.Parse(args[0]);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            }
            Console.WriteLine("Using {0} threads:", threadCount);

            var a = new Program { _atomic = new InterlockAtomic() };
            var b = new Program { _atomic = new MonitorAtomic() };
            var c = new Program { _atomic = new MonitorVolatileAtomic() };
            RunAll(a.RunCompareExchange, threadCount);
            RunAll(b.RunCompareExchange, threadCount);
            RunAll(c.RunCompareExchange, threadCount);
            RunAll(a.RunExchange, threadCount);
            RunAll(b.RunExchange, threadCount);
            RunAll(c.RunExchange, threadCount);
        }


        private static void RecordElapsed(Action action, bool writeDetail)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            accumulator.Accumulate(sw.ElapsedMilliseconds);
            if (writeDetail)
            {
                Console.WriteLine("{0}: {1}.{2} : {3}ns",
                    Thread.CurrentThread.Name,
                    action.Target,
                    action.Method.Name,
                    sw.ElapsedMilliseconds * (1000000.0 / loop));
            }
        }


        private void RunCompareExchange()
        {
            int result1, result2, result3;
            for (int i = loop - 1; i >= 0; i--)
            {
                _atomic.CompareExchange(50, 100);
                result1 = _atomic.Value;
                _atomic.CompareExchange(100, 50);
                result2 = _atomic.Value;
                _atomic.CompareExchange(50, 100);
                result3 = _atomic.Value;
            }
        }

        private void RunExchange()
        {
            int result1, result2, result3;
            for (int i = loop - 1; i >= 0; i--)
            {
                _atomic.Exchange(30);
                result1 = _atomic.Value;
                _atomic.Exchange(50);
                result2 = _atomic.Value;
                _atomic.Exchange(100);
                result3 = _atomic.Value;
            }
        }

        static void RunAll(Action work, int parallelCount)
        {
            accumulator = new Accumulator();
            Action[] works = new Action[parallelCount];
            for (int i = 0; i < parallelCount; i++)
            {
                works[i] = () => RecordElapsed(work, false);
            }
            RunAll(works);
            Console.WriteLine("{0,25}.{1,-20} (ns): {2,6} Average, {3,6} Minimal, {4,6} Maxmial",
                work.Target, work.Method.Name,
                accumulator.Average, accumulator.Minimal, accumulator.Maximal);
        }

        static void RunAll(ICollection<Action> works)
        {
            int workCount = works.Count;
            Barrier barrier = new Barrier(workCount);

            List<Thread> threads = new List<Thread>(workCount);

            foreach (Action work in works)
            {
                Action action = work;
                var t = new Thread(new ThreadStart(
                    delegate {
                        barrier.WaitUntilOpen();
                        action();
                    }));
                threads.Add(t);
                t.Name = string.Format("Thread {0} of {1}", threads.Count, workCount);
                t.Start();
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }

        public override string ToString()
        {
            return _atomic.GetType().Name;
        }
    }

    internal delegate void Action();

    internal class Accumulator
    {
        private long _minimal = long.MaxValue;
        public long Minimal
        {
            get { lock(this) return _minimal; }
        }

        private long _maximal = long.MinValue;
        public long Maximal
        {
            get { lock (this) return _maximal; }
        }

        private long _count;
        public long Count
        {
            get { lock (this) return _count; }
        }

        private long _total;
        public long Total
        {
            get { lock (this) return _total; }
        }

        public long Average
        {
            get { return _total / _count; }
        }

        public void Accumulate(long element)
        {
            lock (this)
            {
                if (element < _minimal) _minimal = element;
                if (element > _maximal) _maximal = element;
                _count++;
                _total += element;
            }
        }
    }

    internal class Barrier
    {
        private readonly int _totalRunner;
        private int _readyCount;
        public Barrier(int totalRunner)
        {
            _totalRunner = totalRunner;
        }

        public void WaitUntilOpen()
        {
            lock (this)
            {
                if (++_readyCount < _totalRunner) Monitor.Wait(this);
                else Monitor.PulseAll(this);
            }
        }
    }

    internal interface IAtomic
    {
        int Value { get; set; }
        int CompareExchange(int newValue, int expected);
        int Exchange(int newValue);
    }

    internal class InterlockAtomic : IAtomic
    {
        private volatile int _value = 50;

        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public virtual int CompareExchange(int newValue, int expected)
        {
            return Interlocked.CompareExchange(ref _value, newValue, expected);
        }

        public virtual int Exchange(int newValue)
        {
            return Interlocked.Exchange(ref _value, newValue);
        }
    }

    internal class MonitorAtomic : IAtomic
    {

        private int _value = 50;

        public int Value
        {
            get { lock (this) return _value; }
            set { lock (this) _value = value; }
        }

        public virtual int CompareExchange(int newValue, int expected)
        {
            lock (this)
            {
                int orig = _value;
                if (expected == orig) _value = newValue;
                return orig;
            }
        }

        public virtual int Exchange(int newValue)
        {
            lock (this)
            {
                int orig = _value;
                _value = newValue;
                return orig;
            }
        }
    }

    internal class MonitorVolatileAtomic : IAtomic
    {

        private volatile int _value = 50;

        public int Value
        {
            get { return _value; }
            set { lock (this) _value = value; }
        }

        public virtual int CompareExchange(int newValue, int expected)
        {
            lock (this)
            {
                int orig = _value;
                if (expected == orig) _value = newValue;
                return orig;
            }
        }

        public virtual int Exchange(int newValue)
        {
            lock (this)
            {
                int orig = _value;
                _value = newValue;
                return orig;
            }
        }
    }
}
