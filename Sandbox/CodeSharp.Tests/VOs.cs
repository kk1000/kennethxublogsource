using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSharp
{
    public interface IMinimalMethod
    {
        void MinimalMethod();
    }

    public interface IParameterlessMethod
    {
        string ParameterlessMethod();
    }

    public interface IFoo
    {
        int Bar { get; set; }
        //long this[long l] { get; set; }
        //string this[string s, int i] { get; set; }
        void BarVoid();
        string BarString(int i, out int x, ref long y);
    }

}
