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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace CodeSharp
{
    /// <author>Kenneth Xu</author>
    public abstract class ChangeTrackerBase : IChangeTrackerBase, ICloneable
    {
        #region Private Class BitMapManager
        private class BitMapManager
        {
            private static Dictionary<Type, Dictionary<string, int>> _bitMapKeeper
                = new Dictionary<Type, Dictionary<string, int>>();

            public static Dictionary<string, int> GetBitMap(Type type)
            {
                lock (_bitMapKeeper)
                {
                    if (_bitMapKeeper.ContainsKey(type))
                        return _bitMapKeeper[type];
                }


                Dictionary<string, int> bitMap = new Dictionary<string, int>();
                string[] props = Array.ConvertAll(type.GetProperties(),
                                                  delegate(PropertyInfo pi)
                                                      { return pi.Name; });
                Array.Sort(props);

                for (int bitIndex = 0; bitIndex < props.Length; bitIndex++)
                {
                    bitMap[props[bitIndex]] = bitIndex;
                    //bitMap.Add(props[bitIndex], bitIndex);
                }

                lock (_bitMapKeeper)
                {
                    if (!_bitMapKeeper.ContainsKey(type))
                        _bitMapKeeper.Add(type, bitMap);
                }


                return bitMap;
            }
        }
        #endregion

        #region Private Instance Fields
        private Dictionary<string, int> _bitArrayIndexLookup;
        private BitArray _dirtyBit;
        private bool _isDirty;
        private bool _trackingDirtyBit;
        private object _clonedObject;
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns true if any of the object property is changed. false otherwise.
        /// </summary>
        public virtual bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
        } // --

        /// <summary>
        /// Gets or sets whether the object should start tracking changes or not.
        /// When set to true, a copy of the current object will be cloned.
        /// When set to false, the cloned object will be set to null.
        /// </summary>
        [XmlIgnore]
        public virtual bool TrackingDirtyBit
        {
            get { return _trackingDirtyBit; }
            set
            {
                //[DN] This setter method is now used to reset the cloned object and set IsDirty property to False.
                //This allowes to reset dirty dits tracking after a successful update.
                if (value)
                {
                    _isDirty = false;
                    _clonedObject = Clone();
                    _dirtyBit.SetAll(false);
                }
                else _clonedObject = null;
                _trackingDirtyBit = value;
            }
        } // --

        /// <summary>
        /// Gets the BitArray's property name and index map
        /// </summary>
        [XmlIgnore]
        public virtual Dictionary<string, int> BitArrayIndexLookup
        {
            get { return _bitArrayIndexLookup; }
        } // --

        /// <summary>
        /// Gets the cloned object of the current class.
        /// </summary>
        [XmlIgnore]
        protected virtual object ClonedObject
        {
            get { return _clonedObject; }
        } // --

        [XmlAttribute]
        public virtual string DirtyBit
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (bool bit in _dirtyBit)
                {
                    sb.Append(bit ? "1" : "0");
                }
                return sb.ToString();
            }
            set
            {
                _dirtyBit.SetAll(false);
                int cnt = 0;
                foreach (char ch in value)
                {
                    _dirtyBit[cnt++] = (ch == '1' ? true : false);
                }
            }
        }

        #endregion

        #region Public Constructors
        /// <summary>
        /// Constructor, also calls the method DirtyBitArraySet
        /// </summary>
        public ChangeTrackerBase()
        {
            _clonedObject = null;
            DirtyBitArraySetup();
        } // --
        #endregion

        #region Public Instance Methods
        /// <summary>
        /// Reset the values of properties to make it back in the initial priestine state.
        /// </summary>
        public virtual void ResetChanges()
        {
            if (IsDirty && _clonedObject != null)
            {
                foreach (KeyValuePair<string, object> kvp in GetDirtyProperties())
                {
                    GetType().GetProperty(kvp.Key).SetValue(this, GetOriginalValue(kvp.Key), null);
                }
            }

            _dirtyBit.SetAll(false);
        }

        /// <summary>
        /// Accept changes and reset the track bits.
        /// </summary>
        public virtual void AcceptChanges()
        {
            TrackingDirtyBit = false;
            TrackingDirtyBit = true;
        }

        /// <summary>
        /// Convenience method to check whether or not the property is valid for tracking
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual bool IsPropertyValidForTracking(string propertyName)
        {
            return _bitArrayIndexLookup.ContainsKey(propertyName);
        }

        /// <summary>
        /// Returns true
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns>Returns false if TrackingDirtyBit is false or if the current value of the property is the same as the original value of the property</returns>
        /// <exception cref="NullReferenceException">If the method DirtyBitArraySetup(object) was not called prior to this call or the property name is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the property name does not exist in the internal lookup dictionary</exception>
        public virtual bool IsPropertyDiry(string propertyName)
        {
            //[DN] No need to check _trackingDirtyBit because this method can be used on the Server
            //after all the changes have been already tracked, object serialized and then deserialized
            //back on the Server side with _dirtyBit array set to propper value.
            if (_dirtyBit == null || _bitArrayIndexLookup == null)
                throw new NullReferenceException("Dirty bit array was not properly setup.");

            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            if (!_bitArrayIndexLookup.ContainsKey(propertyName))
                throw new ArgumentOutOfRangeException(String.Format("Property name {0} is invalid for dirty bit tracking.", propertyName));

            return _dirtyBit[_bitArrayIndexLookup[propertyName]];
        } // --

        /// <summary>
        /// Clones the current object
        /// </summary>
        /// <returns>Clone of the current object</returns>
        public virtual object Clone()
        {
            return MemberwiseClone();
        } // --

        /// <summary>
        /// Gets the original value of the property (value before the property TrackingDirtyBit was set)
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>if TrackingDirtyBit is false, returns null; Otherwise, returns the original value of the property</returns>
        public virtual object GetOriginalValue(string propertyName)
        {
            if (!_trackingDirtyBit)
                return null;

            return _clonedObject.GetType().GetProperty(propertyName).GetValue(_clonedObject, null);
        } // --

        /// <summary>
        /// Gets the current value of the specified Propery.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Value of the specified Propery.</returns>
        public virtual object GetCurrentValue(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            return GetType().GetProperty(propertyName).GetValue(this, null);
        } // --

        /// <summary>
        /// Sets the <paramref name="propertyName"/>'s current value with <paramref name="newValue"/>
        /// </summary>
        /// <param name="propertyName">name of the property</param>
        /// <param name="newValue">new value to set</param>
        public virtual void SetCurrentValue(string propertyName, object newValue)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            GetType().GetProperty(propertyName).SetValue(this, newValue, null);            
        }

        /// <summary>
        /// Gets a Dictionary of dirty Properties' names and their Current (New) values.
        /// </summary>
        /// <returns>IDictionary of dirty Properties' names and their Current (New) values.</returns>
        public virtual IDictionary<string, object> GetDirtyProperties()
        {
            IDictionary<string, object> list = new Dictionary<string, object>();
            foreach (string name in BitArrayIndexLookup.Keys)
            {
                if (IsPropertyDiry(name))
                {
                    list.Add(name, GetCurrentValue(name));
                }
            }

            return list;
        }

        public virtual void CopyDirtyProperties(object target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            foreach (KeyValuePair<string, object> kvp in GetDirtyProperties())
            {
                target.GetType().GetProperty(kvp.Key).SetValue(target, kvp.Value, null);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that reports the detail information
        /// of the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that reports the detail information 
        /// of the current <see cref="T:System.Object"></see>.
        /// </returns>
        public virtual string Dump()
        {
            return Dump(new StringBuilder()).ToString();
        }

        protected virtual StringBuilder Dump(StringBuilder sb)
        {
            sb.AppendFormat("{1}, Dirty Tracking? {2}, Is Dirty? {3}{0}[IsDirty]-[Property].[New Value].[Orig Value]{0}",
                            Environment.NewLine, base.ToString(), _trackingDirtyBit, _isDirty);
            foreach (string propName in _bitArrayIndexLookup.Keys)
            {
                sb.Append('[').Append(IsPropertyDiry(propName) ? "Dirty" : "Clean").Append("]-[").Append(propName).Append("].[");
                Dump(GetCurrentValue(propName), sb);
                sb.Append("].[");
                Dump(GetOriginalValue(propName), sb);
                sb.Append(']').Append(Environment.NewLine);
            }
            return sb;
        }

        protected static void Dump(object o, StringBuilder sb)
        {
            if (o != null)
            {
                ChangeTrackerBase tracker = o as ChangeTrackerBase;
                if (tracker != null) tracker.Dump(sb);
                else sb.Append(o);
            }
        }

        /// <summary>
        /// Sets up the dirty bit array, including a BitArray and a Dictionary&lt;string, int&gt; that keeps the BitArray index map 
        /// based on the property names.
        /// </summary>
        protected void DirtyBitArraySetup()
        {
            _dirtyBit = new BitArray(GetType().GetProperties().Length, false);
            _bitArrayIndexLookup = BitMapManager.GetBitMap(GetType());
        } // --

        /// <summary>
        /// This method tracks the dirty bit array of the given property name.
        /// 
        /// If TrackingDirtyBit is false, this method does nothing.
        /// 
        /// If the method DirtyBitArraySetup was not called prior to this method call, 
        /// the method will attempt to call the method DirtyBitArraySetup first with the given obj.
        /// 
        /// Then the method will get the original value of the property by calling method GetOriginalValue.
        /// 
        /// Then the method will set the dirty bit based on the comparison between the new value and the original value.
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <exception cref="NullReferenceException">If the method DirtyBitArraySetup(object) was not called prior to this call or the property name is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the property name does not exist in the internal lookup dictionary</exception>
        /// <seealso cref="DirtyBitArraySetup"/>
        /// <seealso cref="GetOriginalValue"/>
        public virtual void TrackDirty(string propertyName)
        {
            if (_trackingDirtyBit)
            {
                if (_dirtyBit == null || _bitArrayIndexLookup == null)
                    throw new NullReferenceException("Dirty bit array was not properly setup.");

                if (propertyName == null)
                    throw new ArgumentNullException("propertyName");

                if (!_bitArrayIndexLookup.ContainsKey(propertyName))
                    throw new ArgumentOutOfRangeException(String.Format("Property name {0} is invalid for dirty bit tracking.", propertyName));

                int bitIndex = _bitArrayIndexLookup[propertyName];
                object origVal = GetOriginalValue(propertyName);
                object newVal = GetType().GetProperty(propertyName).GetValue(this, null);

                if (newVal == null)
                {
                    if (origVal == null)
                        _dirtyBit[bitIndex] = false;
                    else _dirtyBit[bitIndex] = true;
                }
                else if (newVal.Equals(origVal))
                    _dirtyBit[bitIndex] = false;
                else _dirtyBit[bitIndex] = true;

                _isDirty = false;
                foreach (bool bit in _dirtyBit)
                {
                    _isDirty = _isDirty || bit;
                    if (_isDirty)
                        break;
                }
            }
        } // --
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        # region Private Methods
        protected virtual void FirePropertyChanging(string propertyName)
        {
        }

        protected virtual void FirePropertyChanged(string propertyName)
        {
            TrackDirty(propertyName);
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public virtual void FirePropertyChangedWithoutTrackingDirty(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        # endregion
    }
}
