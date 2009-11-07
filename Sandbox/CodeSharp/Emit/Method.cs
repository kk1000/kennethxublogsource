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

        /// <summary>
        /// Set method virtual.
        /// </summary>
        public IMethod Virtual
        {
            get
            {
                _methodAttributes |= MethodAttributes.Virtual;
                return this;
            }
        }

        /// <summary>
        /// Construct a new <see cref="Method"/> of given
        /// <paramref name="name"/>.
        /// </summary>
        /// <param name="returnType">
        /// The return type of the method.
        /// </param>
        /// <param name="name">
        /// The name of the method.
        /// </param>
        /// <param name="parameters">
        /// The parameters of the method.
        /// </param>
        internal Method(Type returnType, string name, params IParameter[] parameters)
            : base(returnType, name, new ParameterList(parameters))
        {
            _methodAttributes = MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Final;
        }

        internal Method(string name, params IParameter[] parameters)
            : this (typeof(void), name, parameters)
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
            var mb = typeBuilder.DefineMethod(_name, _methodAttributes, _returnType, _parameters.GetTypes());
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