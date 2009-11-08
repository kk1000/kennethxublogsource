using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    /// <summary>
    /// Method definition.
    /// </summary>
    class Method : Invokable<IMethod>, IMethod
    {
        protected MethodBuilder _methodBuilder;

        internal Method(MethodInfo methodInfo)
            : this(methodInfo.ReturnType, methodInfo.Name, Parameter.From(methodInfo.GetParameters()))
        {
        }

        internal Method(Type returnType, string name, ParameterList parameters)
            : base(returnType, name, parameters)
        {
            _methodAttributes = MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final;
        }

        internal Method(Type returnType, string name, params IParameter[] parameters)
            : this(returnType, name, new ParameterList(parameters))
        {
        }

        internal Method(string name, ParameterList parameters)
            : this(typeof(void), name, parameters)
        {
        }

        /// <summary>
        /// Emit the IL code to define this method.
        /// </summary>
        /// <param name="typeBuilder">
        /// The type that this field will be defined.
        /// </param>
        public override void EmitDefinition(TypeBuilder typeBuilder)
        {
            if (typeBuilder == null) throw new ArgumentNullException("typeBuilder");
            var mb = typeBuilder.DefineMethod(_name, _methodAttributes, _returnType, _parameters.ToTypes());
            _methodBuilder = mb;
            _parameters.Emit(this);
        }

        protected override ILGenerator GetILGenerator()
        {
            return _methodBuilder.GetILGenerator();
        }

        public override ParameterBuilder DefineParameter(int i, ParameterAttributes attributes, string name)
        {
            return _methodBuilder.DefineParameter(i, attributes, name);
        }

        protected override IMethod Self
        {
            get { return this; }
        }
    }
}