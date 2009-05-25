using System;
using System.Threading;

namespace InterlockVsMonitor
{
    class Program
    {
        private static int _loop = 1000000;
        private static bool _verbose;
        private static int _threadCount = 1;

        private IAtomic _atomic;
        private Action _action;
        private Accumulator _accumulator;

        static void Main(string[] args)
        {
            if (args.Length > 0) _threadCount = int.Parse(args[0]);
            if (args.Length > 1) _verbose = "-v".Equals(args[1]);

            var a = new Program { _atomic = new InterlockedAtomic() };
            var b = new Program { _atomic = new MonitorOnlyAtomic() };
            var c = new Program { _atomic = new MonitorVolatileAtomic() };
            a.RunCompareExchangeInParallel();
            b.RunCompareExchangeInParallel();
            c.RunCompareExchangeInParallel();
            a.RunExchangeInParallel();
            b.RunExchangeInParallel();
            c.RunExchangeInParallel();
            a.RunIncrementInParallel();
            b.RunIncrementInParallel();
            c.RunIncrementInParallel();
        }


        void RunTestInParallel()
        {
            _accumulator = new Accumulator();
            Action[] works = new Action[_threadCount];
            for (int i = 0; i < _threadCount; i++) works[i] = RecordElapsed;

            ThreadingHelper.RunWorksInParallel(works);
            Console.WriteLine("{0,23}.{1,-18} (ns):{2,6} Average,{3,6} Minimal,{4,6} Maxmial,{5,3} Threads",
                _atomic.GetType().Name, _action.Method.Name,
                _accumulator.Average, _accumulator.Minimal, 
                _accumulator.Maximal, _accumulator.Count);
        }

        private void RecordElapsed()
        {

            long elapsed = ThreadingHelper.RecordElapsed(_action);
            _accumulator.Accumulate(elapsed);
            if (_verbose)
            {
                Console.WriteLine("{0}: {1}.{2} : {3}ns",
                    Thread.CurrentThread.Name,
                    _atomic.GetType().Name,
                    _action.Method.Name,
                    elapsed * (1000000.0 / _loop));
            }
        }

        private void RunCompareExchangeInParallel()
        {
            _atomic.Value = 50;
            _action = RunCompareExchange;
            RunTestInParallel();
        }

        private void RunExchangeInParallel()
        {
            _atomic.Value = 0;
            _action = RunExchange;
            RunTestInParallel();
        }

        private void RunIncrementInParallel()
        {
            _atomic.Value = 0;
            _action = RunIncrement;
            RunTestInParallel();
            if (_atomic.Value != _loop * 3 * _threadCount)
            {
                throw new Exception("Increment count error.");
            }
        }

        private void RunCompareExchange()
        {
            int result1, result2, result3;
            for (int i = _loop - 1; i >= 0; i--)
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
            for (int i = _loop - 1; i >= 0; i--)
            {
                _atomic.Exchange(30);
                result1 = _atomic.Value;
                _atomic.Exchange(50);
                result2 = _atomic.Value;
                _atomic.Exchange(100);
                result3 = _atomic.Value;
            }
        }

        private void RunIncrement()
        {
            int result1, result2, result3;
            for (int i = _loop - 1; i >= 0; i--)
            {
                _atomic.Increment();
                result1 = _atomic.Value;
                _atomic.Increment();
                result2 = _atomic.Value;
                _atomic.Increment();
                result3 = _atomic.Value;
            }
        }
    }

    internal delegate void Action();
}
