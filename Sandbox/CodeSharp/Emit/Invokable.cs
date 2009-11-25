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
        private Code _code;
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
        public IParameterList Args
        {
            get { return _parameters;}
        }

        ICode IInvokable.Code()
        {
            return Code();
        }

        /// <summary>
        /// Get the <see cref="Emit.Code"/> associated with this method
        /// definition.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="Emit.Code"/>.
        /// </returns>
        public Code Code()
        {
            if (_code == null)
            {
                _code = new Code();
            }
            return _code;
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
            var il = GetILGenerator();
            Code().Emit(il);
            if (_returnType == typeof(void)) il.Emit(OpCodes.Ret);
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
        /// Mark the member public.
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