using System.Collections.Generic;

namespace Common.Collection
{
    /// <summary>
    /// A <see cref="ISealableList"/> is a <see cref="System.Collections.IList"/> 
    /// that can be made readonly when it is <see cref="ISealable.Seal"/>ed.
    /// </summary>
    public interface ISealableList : System.Collections.IList, ISealableCollection
    {
    }

    /// <summary>
    /// A <see cref="ISealableList{T}"/> is a <see cref="IList{T}"/> 
    /// that can be made readonly when it is <see cref="ISealable.Seal"/>ed.
    /// </summary>
    public interface ISealableList<T> : IList<T>, ISealableCollection<T>
    {
    }
}
