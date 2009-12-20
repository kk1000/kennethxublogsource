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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSharp
{
    /// <author>Kenneth Xu</author>
    public interface IObjectChangeTracker
    {
        // Methods
        void AcceptChanges();
        object Clone();
        void CopyDirtyProperties(object target);
        object GetCurrentValue(string propertyName);
        IDictionary<string, object> GetDirtyProperties();
        object GetOriginalValue(string propertyName);
        bool IsPropertyDiry(string propertyName);
        void ResetChanges();

        // Properties
        bool IsDirty { get; set; }
        bool TrackingDirtyBit { get; set; }
    }
}
