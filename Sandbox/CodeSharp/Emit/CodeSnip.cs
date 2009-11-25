using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    internal abstract class CodeSnip
    {
        public abstract void Emit(ILGenerator il);
    }
}