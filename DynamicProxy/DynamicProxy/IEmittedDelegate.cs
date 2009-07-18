using System;
using System.Reflection;

namespace DynamicProxy
{
    public interface IEmittedDelegate
    {
        Type Type { get; }
        MethodInfo InvokeMethod { get; }
        int Identifier { get; }
    }
}