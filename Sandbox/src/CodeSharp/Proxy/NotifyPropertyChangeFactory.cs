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
using System.ComponentModel;
using CodeSharp.Proxy.NPC;

namespace CodeSharp.Proxy
{
    /// <summary>
    /// Factory class to create composite based proxy that implements
    /// <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    /// <author>Kenneth Xu</author>
    public static class NotifyPropertyChangeFactory
    {
        ///<summary>
        /// The default method name used to trigger the 
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        ///</summary>
        public const string DefaultOnPropertyChangedMethodName = "OnPropertyChanged";

        #region SetBaseType Methods

        /// <summary>
        /// Set the base class for all the generated proxies.
        /// </summary>
        /// <remarks>
        /// <typeparamref name="TBase"/> must be a class that is not sealed and
        /// implements <see cref="INotifyPropertyChanged"/>. It also must has a
        /// non-abstract method with signature: <c>OnPropertyChanged(string)</c>
        /// that is accessable by the derived class.
        /// </remarks>
        /// <typeparam name="TBase">
        /// Type of the base class.
        /// </typeparam>
        public static void SetBaseType<TBase>() where TBase : class, INotifyPropertyChanged
        {
            SetBaseType<TBase>(DefaultOnPropertyChangedMethodName);
        }

        /// <summary>
        /// Set the base class for all the generated proxies.
        /// </summary>
        /// <remarks>
        /// <typeparamref name="TBase"/> must be a class that is not sealed and
        /// implements <see cref="INotifyPropertyChanged"/>. It also must has a
        /// non-abstract method that take one string parameter. The name of the
        /// method is specified by <paramref name="onPropertyChangeMethod"/>.
        /// </remarks>
        /// <typeparam name="TBase">
        /// Type of the base class.
        /// </typeparam>
        /// <param name="onPropertyChangeMethod">
        /// The name of the method to raise the 
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </param>
        public static void SetBaseType<TBase>(string onPropertyChangeMethod)
            where TBase : class, INotifyPropertyChanged
        {
            Factory.SetBaseType(typeof(TBase), onPropertyChangeMethod);
        }

        #endregion

        #region SetMarkingAttribute Methods

        /// <summary>
        /// Indicates the given attribute type <typeparamref name="TA"/>
        /// marks the types that should be wrapped by its proxy.
        /// </summary>
        /// <remarks>
        /// <para>
        /// By defaul, <see cref="NotifyPropertyChangeFactory"/> recognizes
        /// <see cref="NotifyPropertyChangeAttribute"/>. This method allows
        /// developer to replace it with another custom attribute.
        /// </para>
        /// <para>
        /// Use <see cref="SetMarkingAttribute{TA}(Converter{TA,Type})"/>
        /// if the custom attribute also carries base type information.
        /// </para>
        /// <para>
        /// <see cref="NotifyPropertyChangeFactory"/> will also look for a
        /// property of "OnPropertyChangedMethodName" on the attribute 
        /// <typeparamref name="TA"/>. If one exist, user can optionally
        /// use that property to override the event raising method specified
        /// in <see cref="SetBaseType{TBase}(string)"/>.
        /// </para>
        /// <para>
        /// This method can only be called before any proxy is generated.
        /// Otherwise <see cref="InvalidOperationException"/> is thrown.
        /// </para>
        /// </remarks>
        /// <typeparam name="TA">Type of the attribute.</typeparam>
        public static void SetMarkingAttribute<TA>()
            where TA : Attribute
        {
            SetMarkingAttribute<TA>(null);
        }

        /// <summary>
        /// Indicates the given attribute type <typeparamref name="TA"/>
        /// marks the types that should be wrapped by its proxy.
        /// </summary>
        /// <remarks>
        /// <para>
        /// By defaul, <see cref="NotifyPropertyChangeFactory"/> recognizes
        /// <see cref="NotifyPropertyChangeAttribute"/>. This method allows
        /// developer to replace it with another custom attribute.
        /// </para>
        /// <para>
        /// <see cref="NotifyPropertyChangeFactory"/> will also look for a
        /// property of "OnPropertyChangedMethodName" on the attribute 
        /// <typeparamref name="TA"/>. If one exist, user can optionally
        /// use that property to override the event raising method specified
        /// in <see cref="SetBaseType{TBase}(string)"/>.
        /// </para>
        /// <para>
        /// This method can only be called before any proxy is generated.
        /// Otherwise <see cref="InvalidOperationException"/> is thrown.
        /// </para>
        /// </remarks>
        /// <typeparam name="TA">Type of the attribute.</typeparam>
        /// <param name="baseType">
        /// A delegate that retrieves the base type information from given
        /// custom attribute type <typeparamref name="TA"/>. Or <c>null</c>
        /// to disable this feature.
        /// </param>
        public static void SetMarkingAttribute<TA>(Converter<TA, Type> baseType)
            where TA : Attribute
        {
            Factory.SetMarkingAttribute(baseType);
        }

        #endregion

        #region SetEventRaiserAttribute Methods

        /// <summary>
        /// Set the attribute that is applied to the members of the proxy
        /// target to specify a different method, other then the method 
        /// specified in the <see cref="SetBaseType{TBase}(string)"/>,
        /// to be used to raise property change event(s).
        /// </summary>
        /// <remarks>
        /// <see cref="NotifyPropertyChangeFactory"/> will search for the
        /// specified attribute on the memebers of the proxy target. If one
        /// is found, it then uses the <paramref name="onPropertyChanged"/>
        /// delegate to retrieve the name of the method to be used to raise
        /// the event. If the name of the method is null, no event will be
        /// raised.
        /// </remarks>
        /// <typeparam name="TA">
        /// The type of attribute to be used by <see cref="NotifyPropertyChangeFactory"/>.
        /// </typeparam>
        /// <param name="onPropertyChanged">
        /// The delegate to retrieve the event raiser name from the attribute.
        /// </param>
        public static void SetEventRaiserAttribute<TA>(Converter<TA, string> onPropertyChanged)
            where TA : Attribute
        {
            Factory.SetEventRaiserAttribute(onPropertyChanged);
        }

        #endregion

        /// <summary>
        /// Create a new instance of proxy for given <paramref name="target"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the proxy to generated. It must be an interface.
        /// </typeparam>
        /// <param name="target">
        /// The target of proxy.
        /// </param>
        /// <returns>
        /// A newly created proxy instance.
        /// </returns>
        public static T NewProxy<T>(T target) where T : class 
        {
            return Factory.Generator<T>.NewProxy(target);
        }

        #region GetProxy Methods

        /// <summary>
        /// Get an instance of proxy for given <paramref name="target"/>.
        /// </summary>
        /// <remarks>
        /// The method try to find the proxy in cache. If one doesn't exist,
        /// create it and cache it.
        /// </remarks>
        /// <typeparam name="T">
        /// Type of the proxy to generated. It must be an interface.
        /// </typeparam>
        /// <param name="target">
        /// The target of proxy.
        /// </param>
        /// <returns>
        /// The proxy for given instance.
        /// </returns>
        public static T GetProxy<T>(T target) where T : class
        {
            return Factory.Generator<T>.GetProxy(target);
        }

        /// <summary>
        /// Get an enumerator of proxies of the given <paramref name="targets"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the enumerator elements.
        /// </typeparam>
        /// <param name="targets">
        /// An enumerator of original target objects.
        /// </param>
        /// <returns>
        /// An enumerator of proxies of targets or targets itself if it is already
        /// an enumerator of proxies.
        /// </returns>
        public static IEnumerator<T> GetProxy<T>(IEnumerator<T> targets)
            where T : class
        {
            return EnumeratorProxy<T>.GetProxy(targets);
        }

        /// <summary>
        /// Get an enumerable of proxies of the given <paramref name="targets"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the enumerable elements.
        /// </typeparam>
        /// <param name="targets">
        /// An enumerable of original target objects.
        /// </param>
        /// <returns>
        /// An enumerable of proxies of targets or targets itself if it is already
        /// an enumerable of others.
        /// </returns>
        public static IEnumerable<T> GetProxy<T>(IEnumerable<T> targets)
            where T : class
        {
            return EnumerableProxy<T>.GetProxy(targets);
        }

        /// <summary>
        /// Get a collection of proxies of the given <paramref name="targets"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the collection elements.
        /// </typeparam>
        /// <param name="targets">
        /// A collection of original target objects.
        /// </param>
        /// <returns>
        /// A collection of proxies of targets or targets itself if it is already
        /// a collection of others.
        /// </returns>
        public static ICollection<T> GetProxy<T>(ICollection<T> targets)
            where T : class
        {
            return CollectionProxy<T>.GetProxy(targets);
        }

        /// <summary>
        /// Get a list of proxies of the given <paramref name="targets"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the list elements.
        /// </typeparam>
        /// <param name="targets">
        /// A list of original target objects.
        /// </param>
        /// <returns>
        /// A list of proxies of targets or targets itself if it is already
        /// a list of others.
        /// </returns>
        public static IList<T> GetProxy<T>(IList<T> targets)
            where T : class
        {
            return ListProxy<T>.GetProxy(targets);
        }

        /// <summary>
        /// Get a dictionary of proxies of the given <paramref name="targets"/>.
        /// </summary>
        /// <typeparam name="TKey">
        /// Type of the dictionary key.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// Type of the dictionary value.
        /// </typeparam>
        /// <param name="targets">
        /// A dictionary of original target objects.
        /// </param>
        /// <returns>
        /// A dictionary of proxies of targets or targets itself if it is already
        /// a dictionary of others.
        /// </returns>
        public static IDictionary<TKey, TValue> GetProxy<TKey, TValue>(IDictionary<TKey, TValue> targets)
            where TValue : class
        {
            return DictionaryProxy<TKey, TValue>.GetProxy(targets);
        }

        #endregion

        #region GetTarget Methods

        /// <summary>
        /// Retrieve the target instance from given <paramref name="proxy"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Target type.
        /// </typeparam>
        /// <param name="proxy">
        /// The proxy object.
        /// </param>
        /// <returns>
        /// The target instance wrapped by the <paramref name="proxy"/>, or
        /// <paramref name="proxy"/> itself if it is actaully not a proxy.
        /// </returns>
        public static T GetTarget<T>(T proxy) where T : class
        {
            return Factory.Generator<T>.GetTarget(proxy);
        }

        /// <summary>
        /// Retrieve the target enumerator wrapped in <paramref name="proxies"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the enumerator elements.
        /// </typeparam>
        /// <param name="proxies">
        /// An proxy enumerator returned by <see cref="GetProxy{T}(IEnumerator{T})"/>.
        /// </param>
        /// <returns>
        /// The target enumerator instance wrapped by <paramref name="proxies"/>,
        /// or <paramref name="proxies"/> itself if it is actually not a proxy.
        /// </returns>
        public static IEnumerator<T> GetTarget<T>(IEnumerator<T> proxies)
            where T : class
        {
            return EnumeratorProxy<T>.GetTarget(proxies);
        }

        /// <summary>
        /// Retrieve the target enumerable wrapped in <paramref name="proxies"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the enumerable elements.
        /// </typeparam>
        /// <param name="proxies">
        /// An proxy enumerable returned by <see cref="GetProxy{T}(IEnumerable{T})"/>.
        /// </param>
        /// <returns>
        /// The target enumerable instance wrapped by <paramref name="proxies"/>,
        /// or <paramref name="proxies"/> itself if it is actually not a proxy.
        /// </returns>
        public static IEnumerable<T> GetTarget<T>(IEnumerable<T> proxies)
            where T : class
        {
            return EnumerableProxy<T>.GetTarget(proxies);
        }

        /// <summary>
        /// Retrieve the target collection wrapped in <paramref name="proxies"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the collection elements.
        /// </typeparam>
        /// <param name="proxies">
        /// An proxy collection returned by <see cref="GetProxy{T}(ICollection{T})"/>.
        /// </param>
        /// <returns>
        /// The target collection instance wrapped by <paramref name="proxies"/>,
        /// or <paramref name="proxies"/> itself if it is actually not a proxy.
        /// </returns>
        public static ICollection<T> GetTarget<T>(ICollection<T> proxies)
            where T : class
        {
            return CollectionProxy<T>.GetTarget(proxies);
        }

        /// <summary>
        /// Retrieve the target list wrapped in <paramref name="proxies"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the list elements.
        /// </typeparam>
        /// <param name="proxies">
        /// An proxy list returned by <see cref="GetProxy{T}(IList{T})"/>.
        /// </param>
        /// <returns>
        /// The target list instance wrapped by <paramref name="proxies"/>,
        /// or <paramref name="proxies"/> itself if it is actually not a proxy.
        /// </returns>
        public static IList<T> GetTarget<T>(IList<T> proxies)
            where T : class
        {
            return ListProxy<T>.GetTarget(proxies);
        }

        /// <summary>
        /// Retrieve the target dictionary wrapped in <paramref name="proxies"/>.
        /// </summary>
        /// <typeparam name="TKey">
        /// Type of the dictionary keys.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// Type of hte dictionary values.
        /// </typeparam>
        /// <param name="proxies">
        /// An proxy dictionary returned by <see cref="GetProxy{TKey,TValue}(IDictionary{TKey,TValue})"/>.
        /// </param>
        /// <returns>
        /// The target dictionary instance wrapped by <paramref name="proxies"/>,
        /// or <paramref name="proxies"/> itself if it is actually not a proxy.
        /// </returns>
        public static IDictionary<TKey, TValue> GetTarget<TKey, TValue>(IDictionary<TKey, TValue> proxies)
            where TValue : class
        {
            return DictionaryProxy<TKey, TValue>.GetTarget(proxies);
        }

        #endregion

        /// <summary>
        /// Save all generated proxy types in an assembly.
        /// </summary>
        public static void SaveAssembly()
        {
            Factory.SaveAssembly();
        }
    }
}
