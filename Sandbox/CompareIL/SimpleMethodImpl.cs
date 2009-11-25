using CodeSharp.Emit;

namespace CompareIL
{
    public class SimpleMethodImpl : ISimpleMethod
    {
        private readonly ISimpleMethod _wrapped;

        public SimpleMethodImpl(ISimpleMethod wrapped)
        {
            _wrapped = wrapped;
        }

        public string SimpleMethod(int i)
        {
            return _wrapped.SimpleMethod(i);
        }
    }
}
