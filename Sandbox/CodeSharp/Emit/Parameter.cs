using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    /// <summary>
    /// Definition of parameter of method, delegate, indexer and etc.
    /// </summary>
    class Parameter : Operand, IParameter
    {
        private readonly Type _type;
        private readonly string _name;
        private readonly ParameterDirection _direction;
        private ParameterBuilder _parameterBuilder;

        //TODO_Future
        ///// <summary>
        ///// Create an out parameter.
        ///// </summary>
        ///// <typeparam name="T">Type of the parameter.</typeparam>
        ///// <param name="name">name of the parameter</param>
        ///// <returns>
        ///// An out parameter definition.
        ///// </returns>
        //public static IParameter Out<T>(string name)
        //{
        //    return new Parameter(typeof(T), name, ParameterDirection.Out);
        //}

        /// <summary>
        /// Gets the type of parameter.
        /// </summary>
        public override Type Type
        {
            get
            {
                bool isDef = _direction == ParameterDirection.Out || _direction == ParameterDirection.Ref;
                return isDef ? _type.MakeByRefType() : _type;
            }
        }

        /// <summary>
        /// Construct a new instance of <see cref="Parameter"/>
        /// </summary>
        /// <param name="type">
        /// Type of the parameter.
        /// </param>
        /// <param name="name">
        /// Name of the parameter.
        /// </param>
        /// <param name="direction">
        /// The direction of the parameter.
        /// </param>
        public Parameter(Type type, string name, ParameterDirection direction)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (name == null) throw new ArgumentNullException("name");
            //TODO_Future if (type.IsByRef) type = type.GetElementType();
            _type = type;
            _name = name;
            _direction = direction;
        }

        /// <summary>
        /// Defines the parameter.
        /// </summary>
        /// <param name="i">
        /// Position of parameter.
        /// </param>
        /// <param name="invokable">
        /// The method that this parameter is defined. 
        /// </param>
        internal void Emit(int i, Invokable invokable)
        {
            ParameterAttributes attributes = _direction == ParameterDirection.Out ? ParameterAttributes.Out : 0;
            _parameterBuilder = invokable.DefineParameter(i, attributes, _name);
        }

        internal override void EmitGet(ILGenerator il)
        {
            ushort position = (ushort)_parameterBuilder.Position;
            switch (position)
            {
                case 0:
                    il.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    il.Emit(OpCodes.Ldarg_S, position);
                    break;
            }
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            throw new NotImplementedException();
        }
    }
}