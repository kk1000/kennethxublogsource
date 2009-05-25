using System.Threading;

namespace InterlockVsMonitor
{
    internal class Barrier
    {
        private int _parties;
        public Barrier(int parties)
        {
            _parties = parties;
        }

        public void Wait()
        {
            lock (this)
            {
                if (--_parties > 0)
                    do Monitor.Wait(this); while (_parties > 0);
                else 
                    Monitor.PulseAll(this);
            }
        }
    }
}