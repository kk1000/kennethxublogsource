using System.Collections.Generic;
using System.Collections;
using System.Extension;

namespace Common.Collection
{
    /// <summary>
    /// A <see cref="ISealableCollection"/> is a <see cref="ICollection"/> 
    /// that can be made readonly when it is <see cref="ISealable.Seal"/>ed.
    /// </summary>
    public interface ISealableCollection : ICollection, ISealable
    {
    }

    /// <summary>
    /// A <see cref="ISealableCollection{T}"/> is a <see cref="ICollection{T}"/> 
    /// that can be made readonly when it is <see cref="ISealable.Seal"/>ed.
    /// </summary>
    public interface ISealableCollection<T> : ICollection<T>, ISealable
    {
    }
}
