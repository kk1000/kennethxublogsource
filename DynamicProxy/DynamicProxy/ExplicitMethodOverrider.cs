using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicProxy
{
    public class ExplicitMethodOverrider
    {
        private static readonly MethodInfo ConsoleWriteLine =
            typeof (Console).GetMethod("WriteLine", new Type[] {typeof (string)});

        private readonly string _fieldName;
        private readonly Delegate _delegate;

        public ExplicitMethodOverrider(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder, MethodInfo interfaceMethod)
        {
            var explictMethod = typeBuilder.BaseType.GetMethod(
                interfaceMethod.DeclaringType.FullName.Replace('+', '.') + "." + interfaceMethod.Name,
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Build the delegate type. TODO: optimize it to cache by return/parameter types.
            var emittedDelegate = moduleBuilder.GenerateDelegateType(explictMethod, true);

            // Build the static field to hold the delegate
            _fieldName = "Base_" + emittedDelegate.Identifier + "_" + interfaceMethod.Name;
            FieldBuilder fieldBuilder = typeBuilder.DefineField(
                _fieldName, emittedDelegate.Type, FieldAttributes.Static | FieldAttributes.Private);

            Type[] parameterTypes = interfaceMethod.GetParamTypes();
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                interfaceMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual,
                interfaceMethod.ReturnType, parameterTypes);
            bool hasReturn = (interfaceMethod.ReturnType != typeof (void));

            ILGenerator ilg = methodBuilder.GetILGenerator();

            // int result;
            if (hasReturn) ilg.DeclareLocal(interfaceMethod.ReturnType);

            //  Console.WriteLine("Proxy before call " + explictMethod);
            ilg.Emit(OpCodes.Ldstr, "Proxy before call " + explictMethod);
            ilg.Emit(OpCodes.Call, ConsoleWriteLine);

            // num = Base_2_BarMethod(this, arg1);
            ilg.Emit(OpCodes.Ldsfld, fieldBuilder);
            for (int i = 0; i < parameterTypes.Length + 1; i++)
            {
                ilg.Emit(OpCodes.Ldarg, (ushort)i);
            }
            ilg.Emit(OpCodes.Callvirt, emittedDelegate.InvokeMethod);
            // save return result if any.
            if (hasReturn) ilg.Emit(OpCodes.Stloc_0);

            //  Console.WriteLine("Proxy after call " + explictMethod);
            ilg.Emit(OpCodes.Ldstr, "Proxy after call " + explictMethod);
            ilg.Emit(OpCodes.Call, ConsoleWriteLine);

            // return
            if (hasReturn) ilg.Emit(OpCodes.Ldloc_0);
            ilg.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, interfaceMethod);
            _delegate = Delegate.CreateDelegate(emittedDelegate.Type, explictMethod);
        }

        public void InitializeDelegate(Type type)
        {
            type.GetField(_fieldName, BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, _delegate);
        }
    }
}