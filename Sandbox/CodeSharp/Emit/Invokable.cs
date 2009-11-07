using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    /// <summary>
    /// Base class of <see cref="Method"/> and <see cref="Constructor"/>
    /// </summary>
    abstract class Invokable : IInvokable
    {
        protected Type _returnType;
        protected string _name;

        protected ParameterList _parameters;
        private MethodCode _methodCode;
        protected MethodAttributes _methodAttributes;

        protected Invokable(Type returnType, string name, ParameterList parameters)
        {
            if (name == null) throw new ArgumentNullException("name");
            _returnType = returnType ?? typeof(void);
            _name = name;
            _parameters = parameters;
        }



        /// <summary>
        /// Parameter list of the method.
        /// </summary>
        public IParameterList Arg
        {
            get { return _parameters;}
        }

        IMethodCode IInvokable.Code()
        {
            return Code();
        }

        /// <summary>
        /// Get the <see cref="MethodCode"/> associated with this method
        /// definition.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="MethodCode"/>.
        /// </returns>
        public MethodCode Code()
        {
            if (_methodCode == null)
            {
                _methodCode = new MethodCode();
            }
            return _methodCode;
        }

        /// <summary>
        /// Emit the IL code to define this method.
        /// </summary>
        /// <param name="typeBuilder">
        /// The type that this field will be defined.
        /// </param>
        public abstract void EmitDefinition(TypeBuilder typeBuilder);

        /// <summary>
        /// Emit the IL code of method body.
        /// </summary>
        public virtual void EmitCode()
        {
            Code().Emit(GetILGenerator());
        }

        protected abstract ILGenerator GetILGenerator();

        public abstract ParameterBuilder DefineParameter(int i, ParameterAttributes attributes, string name);
    }

    internal abstract class Invokable<T> : Invokable
    where T : IInvokable
    {
        protected Invokable(Type returnType, string name, ParameterList parameters) 
            : base(returnType, name, parameters)
        {
        }

        /// <summary>
        /// Set method as public.
        /// </summary>
        public T Public
        {
            get
            {
                _methodAttributes |= MethodAttributes.Public;
                return Self;
            }
        }

        protected abstract T Self { get; }
    }
}