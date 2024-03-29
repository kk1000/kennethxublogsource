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
using System.ComponentModel;

namespace CodeSharp
{
    /// <author>Kenneth Xu</author>
    public interface IChangeTrackerBase : IObjectChangeTracker, INotifyPropertyChanged
    {
        /// <summary>
        /// Convenience method to check whether or not the property is valid for tracking
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        bool IsPropertyValidForTracking(string propertyName);


        /// <summary>
        /// Sets the <paramref name="propertyName"/>'s current value with <paramref name="newValue"/>
        /// </summary>
        /// <param name="propertyName">name of the property</param>
        /// <param name="newValue">new value to set</param>
        void SetCurrentValue(string propertyName, object newValue);

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
        /// <seealso cref="ChangeTrackerBase.DirtyBitArraySetup"/>
        /// <seealso cref="ChangeTrackerBase.GetOriginalValue"/>
        void TrackDirty(string propertyName);

        void FirePropertyChangedWithoutTrackingDirty(string propertyName);
    }
}
