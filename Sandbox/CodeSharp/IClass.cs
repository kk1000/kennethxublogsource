using System;
using System.Reflection;

namespace CodeSharp
{
    /// <summary>
    /// Represents a class.
    /// </summary>
    public interface IClass
    {
        /// <summary>
        /// Inherits from <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">
        /// The type to inherit from.
        /// </typeparam>
        /// <returns>
        /// This <see cref="IClass"/> instance.
        /// </returns>
        IClass Inherits<T>() where T : class;

        /// <summary>
        /// Inherits from <paramref name="baseType"/>.
        /// </summary>
        /// <returns>
        /// This <see cref="IClass"/> instance.
        /// </returns>
        IClass Inherits(Type baseType);

        /// <summary>
        /// Implements the interface.
        /// </summary>
        /// <param name="interface">
        /// The interface to be implemented.
        /// </param>
        /// <returns>
        /// This <see cref="IClass"/> itself.
        /// </returns>
        IClass Implements(Type @interface);

        /// <summary>
        /// Implements the interfaces.
        /// </summary>
        /// <param name="interfaces">
        /// The interfaces to be implemented.
        /// </param>
        /// <returns>
        /// This <see cref="IClass"/> itself.
        /// </returns>
        IClass Implements(params Type[] interfaces);

        /// <summary>
        /// Define a new field in class.
        /// </summary>
        /// <param name="type">
        /// The type of the field
        /// </param>
        /// <param name="name">
        /// The name of the field
        /// </param>
        /// <returns>
        /// A new field definition object
        /// </returns>
        IField Field(Type type, string name);

        /// <summary>
        /// Define a new method in the class.
        /// </summary>
        /// <param name="returnType">
        /// The return type of the defined method.
        /// </param>
        /// <param name="name">
        /// The name of the new method.
        /// </param>
        /// <param name="parameters">
        /// Parameters of the method.
        /// </param>
        /// <returns>
        /// A new method definition.
        /// </returns>
        IMethod Method(Type returnType, string name, params IParameter[] parameters);

        /// <summary>
        /// Define a new method in the class with void return type.
        /// </summary>
        /// <param name="name">
        /// The name of the new method.
        /// </param>
        /// <param name="parameters">
        /// Parameters of the method.
        /// </param>
        /// <returns>
        /// A new method definition.
        /// </returns>
        IMethod Method(string name, params IParameter[] parameters);

        /// <summary>
        /// Define a new method in the class according the given
        /// <paramref name="methodInfo"/>.
        /// </summary>
        /// <param name="methodInfo">
        /// The propertyInfo to defined the new method.
        /// </param>
        /// <returns>
        /// A new method definition.
        /// </returns>
        IMethod Method(MethodInfo methodInfo);

        /// <summary>
        /// Define a new constructor in class.
        /// </summary>
        /// <param name="parameters">
        /// Parameters of the constructor.
        /// </param>
        /// <returns>
        /// A constructor definition object.
        /// </returns>
        IConstructor Constructor(params IParameter[] parameters);

        /// <summary>
        /// Define a new property in class
        /// </summary>
        /// <param name="type">
        /// Type of the property.
        /// </param>
        /// <param name="name">
        /// Name of the property.
        /// </param>
        /// <returns>
        /// The property definition.
        /// </returns>
        IProperty Property(Type type, string name);

        /// <summary>
        /// Define a new indexer in class
        /// </summary>
        /// <param name="type">
        /// The type of indexer.
        /// </param>
        /// <param name="parameters">
        /// Parameter list of indexer.
        /// </param>
        /// <returns>
        /// The indexer definition.
        /// </returns>
        IProperty Indexer(Type type, params IParameter[] parameters);

        /// <summary>
        /// Define a new property or indexer in class according to the given
        /// <paramref name="propertyInfo"/>
        /// The <see cref="PropertyInfo"/> to define the property.
        /// </summary>
        /// <returns>
        /// The property definition.
        /// </returns>
        IProperty Property(PropertyInfo propertyInfo);

        /// <summary>
        /// Set the namespace of current class.
        /// </summary>
        /// <param name="namespace">
        /// The namespace name to set.
        /// </param>
        /// <returns>
        /// The current class.
        /// </returns>
        IClass In(string @namespace);

        /// <summary>
        /// Mark the class public.
        /// </summary>
        IClass Public { get;}

        /// <summary>
        /// Represents the current instance of class.
        /// </summary>
        IOperand This { get; }
    }
}