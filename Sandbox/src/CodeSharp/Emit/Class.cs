#region License

/*
 * Copyright (C) 2009-2010 the original author or authors.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeSharp.Emit
{
    /// <summary>
    /// Definition of a class.
    /// </summary>
    /// <author>Kenneth Xu</author>
    class Class : IClass
    {
        readonly List<Type> _interfaces = new List<Type>();
        readonly List<Method> _methods = new List<Method>();
        readonly List<Field> _fields = new List<Field>();
        readonly List<Constructor> _constuctors = new List<Constructor>();
        readonly List<Property> _properties = new List<Property>();

        private TypeAttributes _typeAttributes;
        internal TypeBuilder TypeBuilder { get; private set; }

        /// <summary>
        /// Get all fields defined in this class.
        /// </summary>
        public ICollection<Field> Fields
        {
            get { return _fields; }
        }

        /// <summary>
        /// Gets the interfaces that this class should implement.
        /// </summary>
        public Type[] Interfaces 
        { 
            get { return _interfaces.ToArray(); }
        }

        /// <summary>
        /// The base type of this class.
        /// </summary>
        public Type BaseType { get; private set;}


        /// <summary>
        /// Gets the full name of the class.
        /// </summary>
        public string FullName
        {
            get { return Namespace + "." + Name; }
        }

        /// <summary>
        /// The namespace of current class.
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        /// Gets the name of the class.
        /// </summary>
        public string Name { get; private set; }

        private Class()
        {
            _typeAttributes = TypeAttributes.Class | TypeAttributes.BeforeFieldInit;
            BaseType = typeof (object);
        }

        /// <summary>
        /// Construct a new instance of <see cref="Class"/>
        /// </summary>
        /// <param name="name">
        /// The name of the class.
        /// </param>
        public Class(string name) : this()
        {
            if (name == null) throw new ArgumentNullException("name");
            Name = name;
        }

        /// <summary>
        /// Implements the interface.
        /// </summary>
        /// <param name="interface">
        /// The interface to be implemented.
        /// </param>
        /// <returns>
        /// This object itself.
        /// </returns>
        public IClass Implements(Type @interface)
        {
            if (@interface == null) throw new ArgumentNullException("interface");
            if (!@interface.IsInterface)
            {
                throw new ArgumentException(@interface.Name + " is not an interfaces.", "interface");
            }
            _interfaces.Add(@interface);
            return this;
        }

        public IClass Implements(params Type[] interfaces)
        {
            foreach (var type in interfaces)
            {
                Implements(type);
            }
            return this;
        }

        public IClass Inherits<T>()
            where T : class 
        {
            BaseType = typeof (T);
            return this;
        }

        public IClass Inherits(Type baseType)
        {
            BaseType = baseType;
            return this;
        }

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
        /// <returns></returns>
        public IMethod Method(Type returnType, string name, params IParameter[] parameters)
        {
            return AddMethod(new Method(returnType, name, parameters));
        }

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
        public IMethod Method(string name, params IParameter[] parameters)
        {
            return Method(typeof (void), name, parameters);
        }

        public IMethod Method(MethodInfo methodInfo)
        {
            return AddMethod(new Method(methodInfo));
        }

        private IMethod AddMethod(Method method)
        {
            _methods.Add(method);
            return method;
        }

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
        public IField Field(Type type, string name)
        {
            var field = new Field(type, name);
            _fields.Add(field);
            return field;
        }

        /// <summary>
        /// Define a new constructor in class.
        /// </summary>
        /// <param name="parameters">
        /// Parameters of the constructor.
        /// </param>
        /// <returns>
        /// A constructor definition object.
        /// </returns>
        public IConstructor Constructor(params IParameter[] parameters)
        {
            var constructor = new Constructor(parameters);
            _constuctors.Add(constructor);
            return constructor;
        }

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
        public IProperty Property(Type type, string name)
        {
            return AddProperty(new Property(type, name));
        }

        public IProperty Indexer(Type type, params IParameter[] parameters)
        {
            return AddProperty(new Property(type, parameters));
        }

        public IProperty Property(PropertyInfo propertyInfo)
        {
            return AddProperty(new Property(propertyInfo));
        }

        private IProperty AddProperty(Property property)
        {
            _properties.Add(property);
            return property;
        }

        /// <summary>
        /// Set the namespace of current class.
        /// </summary>
        /// <param name="namespace">
        /// The namespace name to set.
        /// </param>
        /// <returns>
        /// The current class.
        /// </returns>
        public IClass In(string @namespace)
        {
            if(@namespace==null) throw new ArgumentNullException("namespace");
            Namespace = @namespace;
            return this;
        }

        /// <summary>
        /// Set class to be public.
        /// </summary>
        public IClass Public
        {
            get
            {
                _typeAttributes |= TypeAttributes.Public;
                return this;
            }
        }

        public IOperand This
        {
            get { return new ThisOperand(this); }
        }

        /// <summary>
        /// Generate the class.
        /// </summary>
        /// <returns>
        /// The type object corresponding to the generated class.
        /// </returns>
        public Type Generate(ModuleBuilder moduleBuilder)
        {
            if (moduleBuilder == null) throw new ArgumentNullException("moduleBuilder");
            TypeBuilder tb = moduleBuilder.DefineType(FullName, _typeAttributes, BaseType, Interfaces);
            TypeBuilder = tb;
            foreach (var field in Fields)
            {
                field.Emit(tb);
            }

            foreach (var constructor in _constuctors)
            {
                constructor.EmitDefinition(tb);
            }

            foreach (var property in _properties)
            {
                property.EmitDefinition(tb);
            }

            foreach (var method in _methods)
            {
                method.EmitDefinition(tb);
            }

            foreach (var constructor in _constuctors)
            {
                constructor.EmitCode();
            }

            foreach (var property in _properties)
            {
                property.EmitCode(tb);
            }

            foreach (var method in _methods)
            {
                method.EmitCode();
            }

            return tb.CreateType();
        }
    }

    internal class ThisOperand : Operand
    {
        private readonly Class _class;

        public ThisOperand(Class @class)
        {
            _class = @class;
        }

        public override Type Type {
            get { return _class.TypeBuilder; }
        }

        internal override void EmitGet(ILGenerator il)
        {
            il.Emit(OpCodes.Ldarg_0);
        }

        internal override void EmitSet(ILGenerator il, Operand value)
        {
            throw new InvalidOperationException("Cannot assign to this.");
        }
    }
}
