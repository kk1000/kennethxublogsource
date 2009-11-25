using CodeSharp.Emit;

namespace CompareIL
{
    public class WriteOnlyPropertyImpl : IWriteOnlyProperty
    {
        private readonly IWriteOnlyProperty _wrapped;

        public WriteOnlyPropertyImpl(IWriteOnlyProperty wrapped)
        {
            _wrapped = wrapped;
        }

        public int WriteOnlyProperty
        {
            set { _wrapped.WriteOnlyProperty = value; }
        }
    }
}
