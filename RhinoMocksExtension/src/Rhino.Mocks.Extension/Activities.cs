using System.Text;
using Rhino.Mocks.Exceptions;

namespace Rhino.Mocks.Extension
{
    public class Activities
    {
        private readonly ExpectationViolationException _exception;

        internal Activities(ExpectationViolationException exception) {
            _exception = exception;
        }

        public Activities Or(Activities other) {
            if (Occured) return this;
            if (other.Occured) return other;
            return new Activities(new ExpectationViolationException(
                _exception.Message + "\nor\n" + other._exception.Message));
        }

        public bool Occured
        {
            get { return _exception == null; }
        }

        public void AssertOccured()
        {
            if (_exception != null) throw _exception;
        }

        public static implicit operator bool(Activities activities)
        {
            return activities.Occured;
        }

        public static Activities operator |(Activities a1, Activities a2) {
            return a1.Or(a2);
        }

        public static Activities operator ^(Activities a1, Activities a2) {
            return ExactOneOf(a1, a2);
        }

        public static bool operator true(Activities a) {
            return a.Occured;
        }

        public static bool operator false(Activities a) {
            return !a.Occured;
        }

        internal static Activities ExactOneOf(params Activities[] activitiesList)
        {
            Activities one = null;

            foreach (var activities in activitiesList)
            {
                if (!activities.Occured) continue;
                if (one == null) one = activities;
                else
                    return new Activities(
                        new ExpectationViolationException(
                            "More then one in the activities list was called"));
            }
            if (one != null) return one;

            StringBuilder sb = new StringBuilder("None of below is satisfied:");
            foreach (var activities in activitiesList)
            {
                sb.Append('\n').Append(activities._exception.Message);
            }
            return new Activities(new ExpectationViolationException(sb.ToString()));
        }
    }
}
