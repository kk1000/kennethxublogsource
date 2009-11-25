using CodeSharp.Emit;

namespace CompareIL
{
    public class ReadOnlyPropertyImpl : IReadOnlyProperty
    {
        private readonly IReadOnlyProperty _wrapped;

        public ReadOnlyPropertyImpl(IReadOnlyProperty wrapped)
        {
            _wrapped = wrapped;
        }

        public int ReadOnlyProperty
        {
            get { return _wrapped.ReadOnlyProperty; }
        }
    }
}
