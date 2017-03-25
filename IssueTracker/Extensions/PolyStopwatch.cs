using System.Collections.Generic;
using System.Diagnostics;

namespace IssueTracker.Extensions
{
    public class PolyStopwatch
    {
        private readonly Dictionary<string, double> _counters;

        private readonly Stopwatch _stopwatch;

        private string _currentLabel;

        public PolyStopwatch()
        {
            _stopwatch = new Stopwatch();
            _counters = new Dictionary<string, double>();
        }

        public void Start(string label)
        {
            if (_currentLabel != null) Stop();

            _currentLabel = label;
            if (!_counters.ContainsKey(label))
                _counters.Add(label, 0);
            _stopwatch.Restart();
        }

        public void Stop()
        {
            if (_currentLabel == null)
                return;

            _stopwatch.Stop();
            _counters[_currentLabel] += _stopwatch.Elapsed.TotalSeconds;
            _currentLabel = null;
        }

        public Dictionary<string,double> FlushTimes()
        {
            if (_currentLabel != null) Stop();

            return _counters;
        }

        public void Delete(string label)
        {
            _counters.Remove(label);
        }

    }
}
