using System;
using System.Collections.Generic;

namespace Svn2Svn
{
    public class RevisionMap
    {
        private readonly Dictionary<long, long> _revisionMap = new Dictionary<long, long>();
        private readonly List<long> _revisionHistory = new List<long>();

        public long CheckResyncRevision(long sourceRevision)
        {
            long destRevision = 0;
            if (_revisionHistory.Count == 0) return -1;
            if (sourceRevision >= _revisionHistory[0] &&
                !_revisionMap.TryGetValue(sourceRevision, out destRevision))
            {
                throw new InvalidOperationException(
                    "No matching destination revision for source revision number "
                    + sourceRevision);
            }
            return destRevision;
        }

        public void TrackRevision(long sourceRevision, long destinationReivison)
        {
            _revisionHistory.Add(sourceRevision);
            _revisionMap[sourceRevision] = destinationReivison;
        }

        public long FindDestinationRevision(long revision)
        {
            long result;
            if (_revisionMap.TryGetValue(revision, out result)) return result;
            for (int i = _revisionHistory.Count - 1; i >= 0; i--)
            {
                if (_revisionHistory[i] < revision) return _revisionMap[_revisionHistory[i]];
            }
            return -1;
        }

        public long GetLastRevision()
        {
            return (_revisionHistory.Count == 0) ? 0 : _revisionHistory[_revisionHistory.Count - 1];
        }
    }
}