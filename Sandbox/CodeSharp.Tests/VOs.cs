using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSharp
{
    public interface IReadOnlyProperty
    {
        int ReadOnlyProperty { get; }
    }

    public interface IWriteOnlyProperty
    {
        int WriteOnlyProperty { set; }
    }

    public interface IProperty
    {
        int Property { get; set; }
    }

    public interface IMinimalMethod
    {
        void MinimalMethod();
    }

    public interface IParameterlessMethod
    {
        string ParameterlessMethod();
    }

    public interface ISimpleMethod
    {
        string SimpleMethod(int i);
    }

    public interface IRefParamMethod
    {
        void RefParamMethod(ref long y);
    }

    public interface IOutParamMethod
    {
        void OutParamMethod(out int i);
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
