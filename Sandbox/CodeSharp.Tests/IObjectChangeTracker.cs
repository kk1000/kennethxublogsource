using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeSharp
{
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
