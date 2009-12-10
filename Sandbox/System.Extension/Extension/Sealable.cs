using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;
//using Common.Logging;

namespace System.Extension
{
    /// <summary>
    /// A convenient abstract class to be inherited by classes implementing 
    /// <see cref="ISealable"/>. It also provides utiltiy methods to deep 
    /// seal objects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This abstract class serves three purposes that are detailed in the 
    /// following paragraphs.
    /// </para>
    /// <list type="number">
    /// <item>
    /// As an example to other classes that need to implement the 
    /// <c>ISealable</c> from scratch.
    /// </item>
    /// <item>
    /// To simply the task of creating classes that implements the 
    /// <see cref="ISealable"/>.
    /// </item>
    /// <item>
    /// A utiltiy classes that provides methods to deep seal objects.
    /// </item>
    /// </list>
    /// <para>
    /// This class uses a <c>volatile bool</c> instance field to keep track
    /// the sealable state. It is important to use this pattern in the 
    /// sealables will be used in multi-threaded environment, which is true
    /// by the natural of having a sealable object.
    /// </para>
    /// <para>
    /// Instead of implementing the <c>ISealable</c> interface directly, new 
    /// sealable classes can be defined by inheriting from <c>Sealable</c>
    /// class.
    /// </para>
    /// <example>A concrete sealable class inherited from <c>Sealable</c>
    /// <code>
    /// public class SealableObject : Sealable
    /// {
    ///    private object _myObject;
    ///
    ///    public object MyObject
    ///    {
    ///        get { return _myObject; }
    ///        set { FailIfSealed(); _myObject = value; }
    ///    }
    /// }
    /// </code>
    /// Please notice that the <see cref="FailIfSealed"/> method is called
    /// in the property setter.
    /// </example>
    /// <para>
    /// <c>Sealable</c> class also has two static utility methods to help
    /// recursively seal objects. Both methods are able to detect the looped
    /// references and handle them properly.
    /// </para>
    /// <para>
    /// <c>DeepSeal</c> recursively seals all objects assignable to 
    /// <see cref="ISealable"/>. It walks through all the items in a sealable
    /// collection, all the keys and values in a sealable dictionary, or all
    /// the properties of the object if it is neither collection nor dictionary.
    /// It stops walking further down when it reaches a non <c>ISealable</c> 
    /// object. See <see cref="DeepSeal"/> for more detail.
    /// </para>
    /// <para>
    /// <c>DeeperSeal</c> does more then <c>DeepSeal</c>. It walks through 
    /// every object it encounters regardless of whether it is an <c>ISealable</c>
    /// or not. See <see cref="DeeperSeal"/> for more detail.
    /// </para>
    /// </remarks>
    /// <seealso cref="ISealable"/>
    /// <author>Kenneth Xu</author>
    public abstract class Sealable : ISealable
    {

        #region Public Static Methods
        /// <summary>
        /// Recursively deep seal an instance of <see cref="ISealable"/> object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <c>DeepSeal</c> walks through the collection items or object properties 
        /// to find all <c>ISealable</c> objects. It recursively deep seals
        /// each <c>ISealable</c> it found. Then it seals the object itself.
        /// </para>
        /// <para>
        /// This methods only seals and recusively seals the <c>ISealable</c> object
        /// and its <c>ISealable</c> members. In other words, the recursively walks
        /// through elements of arrays, collections and properties, but the recursivity
        /// stops at any element that is not <c>ISealable</c>. 
        /// <see cref="DeeperSeal"/> take one step
        /// further that it works on any object and seach for all its decending
        /// objects for <c>ISealable</c> objects and then seal them.
        /// </para>
        /// <para>
        /// Below is the pseudo code of deep sealing logic. Please note that the
        /// implementation can be very different as long as the same result is
        /// achieved.
        /// </para>
        /// <code>
        /// public static void DeepSeal(ISealalbe o) {
        ///   if (o is re-entered) return; // handles the looped reference
        ///   if (o is generic IDictionary(of TKey, TValue)) {
        ///     foreach (pair in o) {
        ///       if (TKey is assignable to ISealable)
        ///         DeepSeal(pair.key);
        ///       if (TValue is assignalbe to ISealable)
        ///         DeepSeal(pair.value);
        ///     }
        ///   } else if (o is generic ICollection(of T) and T assingable to ISealable) { 
        ///     // this handles array as well, because array is a collection
        ///     foreach (item in o) DeepSeal(item);
        ///   } else if (o is non-generic IDictionary) {
        ///     foreach (entry in o) {
        ///       if (entry.key is ISealable) DeepSeal(entry.key);
        ///       if (entry.value is ISealable) DeepSeal(entry.value);
        ///     }
        ///   } else if (o is non-generic collection) {
        ///     foreach (item in o) {
        ///       if (item is ISealable) DeepSeal(item);
        ///     }
        ///   } else {
        ///     foreach (public property of o) {
        ///       if (type of property is assignable to ISealable) DeepSeal(property);
        ///     }
        ///   }
        ///   if (!o.isSealed) o.Seal()
        /// }
        /// </code>
        /// </remarks>
        /// <param name="o">the sealable to be deep sealed</param>
        /// <seealso cref="DeeperSeal"/>
        public static void DeepSeal(ISealable o)
        {
            new DeepSealer(false).SealRecursively(o);
        }

        /// <summary>
        /// Recursively deeper seal an object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <c>DeeperSeal</c> walks through all decending collection items or properties 
        /// It recursively deeper seals each decending object it found. Then it seals 
        /// the object itself if the object is <c>ISealable</c>.
        /// </para>
        /// <para>
        /// This methods is different from <see cref="DeepSeal"/>, which only seals and 
        /// recusively seals the <c>ISealable</c> object
        /// and its <c>ISealable</c> members. In stead, <c>DeaperSeal</c> takes one step
        /// further that it works on any object and seach for all its decending
        /// objects for <c>ISealable</c> objects and then seal them.
        /// </para>
        /// <para>
        /// Below is the pseudo code of deeper sealing logic. Please note that the real
        /// implementation can be very different as long as the same result is
        /// achieved.
        /// </para>
        /// <code>
        /// public static void DeeperSeal(object o) {
        ///    if (o is re-entered) return; // handles the looped reference
        ///    if (o is generic or non-generic IDictionary) {
        ///     foreach (entry in o) {
        ///       DeeperSeal(entry.key);
        ///       DeeperSeal(entry.value);
        ///     }
        ///   } else if (o is generic or non-generic collection) {
        ///     // this handles arrays as well
        ///     foreach (item in o) {
        ///       DeeperSeal(item);
        ///     }
        ///   } else {
        ///     foreach (public property of o) {
        ///       DeeperSeal(property);
        ///     }
        ///   }
        ///   if (o is ISealable &amp;&amp; !o.isSealed) o.Seal();
        /// }
        /// </code>
        /// </remarks>
        /// <param name="o">the object to be deeper sealed</param>
        /// <seealso cref="DeepSeal"/>
        public static void DeeperSeal(object o)
        {
            new DeepSealer(true).SealRecursively(o);
        }
        
        #endregion

        #region ISealable Members

        /// <summary>
        /// implements <see cref="ISealable.Seal"/>
        /// </summary>
        public virtual void Seal()
        {
            m_Sealed = true;
        }

        /// <summary>
        /// implements <see cref="ISealable.IsSealed"/>
        /// </summary>
        public bool IsSealed
        {
            get { return m_Sealed; }
        }

        #endregion

        /// <summary>
        /// A convenient method that throws <c>InstanceSealedException</c> when
        /// the instance is sealed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// See documentation of the class <see cref="Sealable"/> for an example.
        /// </para>
        /// </remarks>
        /// <exception cref="InstanceSealedException">when instance is sealed</exception>
        protected void FailIfSealed()
        {
            if (m_Sealed) throw new InstanceSealedException();
        }

        /// <summary>
        /// A volatile flag to indicate that the instance is sealed.
        /// </summary>
        protected volatile bool m_Sealed = false;
        
        /// <summary>
        /// A private helper class used by both <see cref="DeepSeal"/> 
        /// and <see cref="DeeperSeal"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance of this class are designed to be short lived and single threaded.
        /// It is for one time deep or deeper seal object only.
        /// </para>
        /// </remarks>
        /// <seealso cref="DeepSeal"/>
        /// <seealso cref="DeeperSeal"/>
        private class DeepSealer
        {
            #region Private Instance Fields
            /// <summary>
            /// to distinguish a deeper seal from deep seal.
            /// </summary>
            private bool deeper = false;

            /// <summary>
            /// a stack to avoid recursive re-entry.
            /// </summary>
            private Stack<object> processStack = new Stack<object>();

            ///// <summary>
            ///// cache of the debug state
            ///// </summary>
            //private bool isDebug = s_log.IsDebugEnabled;

            #endregion

            /// <summary>
            /// Constructs an instance of <c>DeepSealer</c> to help
            /// <list type="bullet">
            /// <item><c>DeepSeal</c> when parameter <c>deeper</c> is <c>false</c></item>
            /// <item><c>DeeperSeal</c> when parameter <c>deeper</c> is <c>true</c></item>
            /// </list>
            /// </summary>
            /// <param name="deeper">
            /// true to construct a <c>DeeperSeal</c> helper and false to construct a
            /// <c>DeepSeal</c> helper.
            /// </param>
            public DeepSealer(bool deeper)
            {
                this.deeper = deeper;
            }

            /// <summary>
            /// Recursively seal an object
            /// </summary>
            /// <param name="o">object to be recursively sealed</param>
            public void SealRecursively(object o)
            {
                // stop when we detect re-entry (reference loop)
                if (o == null || processStack.Contains(o)) return;

                // stops if o is not ISealable and we are not in deeper mode
                if (!(deeper || o is ISealable)) return;

                //if (isDebug) s_log.Debug("DeepSeal object: [" + o + "]");

                // keep track of objects being processed, so we can detect re-entry
                processStack.Push(o);

                try
                {
                    // we need to do generic collection first for two reasons:
                    // 1. this is by contract in none deeper mode. see the 
                    //    documentation of DealSeal methods for detail.
                    // 2. if a dictionary implements both generic and non-generic,
                    //    foreach returns KeyValuePair instead of DictionaryEntry.
                    bool isProcessed = TryProcessGenericCollection(o);

                    // try to process as none generic collection
                    if (!isProcessed) isProcessed = TryProcessCollection(o);

                    // if o is not a collection, seal its all properties.
                    if (!isProcessed) DeepSealProperties(o);

                    // seal the object itself
                    ISealable s = o as ISealable;
                    if (s != null && !s.IsSealed) s.Seal();
                }
                finally
                {
                    processStack.Pop();
                }
            }

            #region Private Instance Methods

            /// <summary>
            /// Try to process the object as non-generic collection, including
            /// non-generic dictionary and array.
            /// </summary>
            /// <param name="o">the object to be processed</param>
            /// <returns>true if the object is a non-generic collection</returns>
            private bool TryProcessCollection(object o)
            {
                if (o is IDictionary)
                {
                    //s_log.Debug("process object as IDictionary.");

                    foreach (DictionaryEntry entry in (IDictionary)o)
                    {
                        //if (isDebug) s_log.Debug("processing dictionary key " + entry.Key);
                        SealRecursively(entry.Key);

                        //if (isDebug) s_log.Debug("processing dictionary value " + entry.Value);
                        SealRecursively(entry.Value);
                    }
                    return true;
                }
                if (o is ICollection)
                {
                    //s_log.Debug("process object as ICollection");

                    foreach (object item in (ICollection)o)
                    {
                        //if (isDebug) s_log.Debug("processing collection element " + item);
                        SealRecursively(item);
                    }
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Try to process the object as generic collection, including
            /// generic dictionary and array.
            /// </summary>
            /// <param name="o">the object to be processed</param>
            /// <returns>true if the object is a generic collection</returns>
            private bool TryProcessGenericCollection(object o)
            {
                Type oType = o.GetType();
                if (oType.IsGenericType)
                {
                    Type[] typeArgs = oType.GetGenericArguments();

                    if (typeArgs.Length==2 && typeof(IDictionary<,>).MakeGenericType(typeArgs).IsAssignableFrom(oType))
                    {
                        //if(isDebug) s_log.Debug("process object as generic dictionary: [" + oType.FullName + "]");
                        MethodInfo sealer = GENERIC_DICTIONARY_SEALER.MakeGenericMethod(typeArgs);
                        sealer.Invoke(this, new object[] { o });
                        return true;
                    }

                    if (typeArgs.Length==1 && typeof(ICollection<>).MakeGenericType(typeArgs).IsAssignableFrom(oType))
                    {
                        //if(isDebug) s_log.Debug("process object as generic collection: [" + oType.FullName + "]");
                        MethodInfo sealer = GENERIC_COLLECTION_SEALER.MakeGenericMethod(typeArgs);
                        sealer.Invoke(this, new object[] { o });
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Find public properties of the object and recursively seal them. It only act
            /// upon properties with type assignalbe to <see cref="ISealable"/> when it is
            /// not in deeper mode.
            /// </summary>
            /// <param name="o">the object whose properties are being sealed</param>
            private void DeepSealProperties(object o)
            {
                //s_log.Debug("process properites of object");

                foreach (PropertyInfo pi in o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    Type propertyType = pi.PropertyType;
                    bool isIndexed = pi.GetIndexParameters().Length > 0;

                    if (isIndexed || SkipType(propertyType))
                    {
                        //if (isDebug) s_log.Debug( "Property '" + pi + "' is " + 
                        //    (isIndexed ? "indexed property" : "primitive type"));
                        continue;
                    }

                    //if(isDebug) s_log.Debug("processing property " + pi.Name);
                    if (deeper || typeof(ISealable).IsAssignableFrom(pi.PropertyType))
                    {
                        SealRecursively(pi.GetValue(o, null));
                    }
                    //else if (isDebug)
                    //{
                    //    s_log.Debug("type of property " + pi.Name + " is not ISealable");
                    //}
                }
            }

            /// <summary>
            /// Iterate through all the elements in a generic collection and recursively
            /// seal them. In non-deeper mode, no action is taken if generic type parameter
            /// is not assignable to <see cref="ISealable"/>
            /// </summary>
            /// <typeparam name="T">the type of elements in the collection</typeparam>
            /// <param name="c">the collection object deep sealed</param>
            private void DeepSealGenericCollection<T>(ICollection<T> c)
            {
                if (typeof(ISealable).IsAssignableFrom(typeof(T)) || deeper)
                {
                    foreach (T item in c)
                    {
                        //if (isDebug) s_log.Debug("processing collection element " + item);
                        SealRecursively(item);
                    }
                }
                //else if (isDebug)
                //{
                //    s_log.Debug("neither in deeper mode, nor type parameter is assignable to ISealable, " +
                //                "skipping this generic collection");
                //}
            }

            /// <summary>
            /// Iterate through all the keys and values in a generic dictionary and
            /// recursively seal them. In non-deeper mode, in only acts on the keys or 
            /// values of which the type parameter is assignable to <see cref="ISealable"/>.
            /// </summary>
            /// <typeparam name="TKey">the type of keys in the dictionary</typeparam>
            /// <typeparam name="TValue">the type of values in the dictionary</typeparam>
            /// <param name="d">the generic dictionary to be deep sealed</param>
            private void DeepSealGenericDictionary<TKey, TValue>(IDictionary<TKey, TValue> d)
            {
                bool keySealable = typeof(ISealable).IsAssignableFrom(typeof(TKey));
                bool valueSealable = typeof(ISealable).IsAssignableFrom(typeof(TValue));

                if (keySealable || valueSealable || deeper)
                {
                    foreach (KeyValuePair<TKey, TValue> pair in d)
                    {
                        if (keySealable || deeper)
                        {
                            //if (isDebug) s_log.Debug("processing dictionary key " + pair.Key);
                            SealRecursively(pair.Key);
                        }
                        if (valueSealable || deeper)
                        {
                            //if (isDebug) s_log.Debug("processing dictionary value " + pair.Value);
                            SealRecursively(pair.Value);
                        }
                    }
                }
                //else if (isDebug)
                //{
                //    s_log.Debug("neither in deeper mode, nor any of type parameters is assignable to ISealable, " +
                //                "skipping this generic dictionary");
                //}
            }

            private static bool SkipType(Type t)
            {
                return s_skipTypeSet.ContainsKey(t) || 
                    typeof(Exception).IsAssignableFrom(t) ||
                    typeof(Delegate).IsAssignableFrom(t);
            }

            #endregion

            #region Static Constructor
            static DeepSealer()
            {
                foreach (Type t in s_skipTypes)
                {
                    s_skipTypeSet[t] = t;
                }
            }
            #endregion


            #region Private Static Fields

            /// <summary>
            /// used to perform generic method invokation via reflection
            /// </summary>
            private static readonly MethodInfo GENERIC_COLLECTION_SEALER =
                typeof(Sealable.DeepSealer).GetMethod("DeepSealGenericCollection", 
                    BindingFlags.Instance | BindingFlags.NonPublic);

            /// <summary>
            /// used to perform generic method invokation via reflection
            /// </summary>
            private static readonly MethodInfo GENERIC_DICTIONARY_SEALER =
                typeof(Sealable.DeepSealer).GetMethod("DeepSealGenericDictionary", 
                    BindingFlags.Instance | BindingFlags.NonPublic);

            private static readonly Type[] s_skipTypes = {
                typeof(int), typeof(long), typeof(short), typeof(sbyte),
                typeof(uint), typeof(ulong), typeof(ushort), typeof(byte),
                typeof(char), typeof(float), typeof(double), typeof(bool),
                typeof(decimal), typeof(string), typeof(StringBuilder), typeof(Type), 
                typeof(Assembly), typeof(DateTime), typeof(Enum),
            };

            private static readonly Dictionary<Type, Type> s_skipTypeSet = 
                new Dictionary<Type, Type>();

            //private static ILog s_log = LogManager.GetLogger(typeof(DeepSealer));

            #endregion
        }

    }
}
