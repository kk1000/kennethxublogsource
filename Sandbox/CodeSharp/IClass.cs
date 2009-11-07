using System;

namespace CodeSharp
{
    /// <summary>
    /// Represents a class.
    /// </summary>
    public interface IClass
    {
        /// <summary>
        /// Implements the interface.
        /// </summary>
        /// <param name="interface">
        /// The interface to be implemented.
        /// </param>
        /// <returns>
        /// This object itself.
        /// </returns>
        IClass Implements(Type @interface);

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
    }
}