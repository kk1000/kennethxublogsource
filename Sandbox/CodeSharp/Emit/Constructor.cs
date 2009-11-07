using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    class Constructor : Invokable<IConstructor>, IConstructor
    {
        protected ConstructorBuilder _constructorBuilder;

        internal Constructor(params IParameter[] parameters)
            :base(null, ".ctor", new ParameterList(parameters))
        {
            _methodAttributes = MethodAttributes.HideBySig;
        }

        public override void EmitDefinition(TypeBuilder typeBuilder)
        {
            if (typeBuilder == null) throw new ArgumentNullException("typeBuilder");
            var mb = typeBuilder.DefineConstructor(_methodAttributes, CallingConventions.HasThis, _parameters.ToTypes());
            _constructorBuilder = mb;
            _parameters.Emit(this);
        }

        protected override ILGenerator GetILGenerator()
        {
            return _constructorBuilder.GetILGenerator();
        }

        public override ParameterBuilder DefineParameter(int i, ParameterAttributes attributes, string name)
        {
            return _constructorBuilder.DefineParameter(i, attributes, name);
        }

        protected override IConstructor Self
        {
            get { return this; }
        }

        public override void EmitCode()
        {
            var type = _constructorBuilder.DeclaringType.BaseType;
            var mi = type.GetConstructor(Type.EmptyTypes);
            var il = GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, mi);
            base.EmitCode();
        }
    }
}