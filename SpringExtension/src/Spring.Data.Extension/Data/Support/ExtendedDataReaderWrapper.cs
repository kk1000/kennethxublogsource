using System.Data;

namespace Spring.Data.Support
{
    /// <summary>
    /// An implementation of <see cref="IDataReaderWrapper"/> that supports 
    /// the extended features.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Extended features are:
    /// </para>
    /// <para>
    /// Uses <see cref="IDataRecordOrdinalCache"/> to speed up the call 
    /// to <see cref="GetOrdinal"/> call.
    /// </para>
    /// <para>
    /// Make user of <see cref="RowsExpected"/> to optimize the fetch performance.
    /// </para>
    /// </remarks>
    public class ExtendedDataReaderWrapper : DataReaderWrapperBase
    {
        /// <summary>
        /// The backing field of property <see cref="OrdinalCache"/>.
        /// </summary>
        private IDataRecordOrdinalCache _ordinalCache;

        /// <summary>
        /// The backing field of property <see cref="RowsExpected"/>.
        /// </summary>
        private int _rowsExpected;

        /// <summary>
        /// Gets and sets the ordinal cache. When an ordinal cache is set,
        /// it's <see cref="IDataRecordOrdinalCache.Init"/> method will
        /// be called with the <see cref="WrappedReader"/> if both are not
        /// <c>null</c>.
        /// </summary>
        public virtual IDataRecordOrdinalCache OrdinalCache
        {
            get { return _ordinalCache; }
            set
            {
                _ordinalCache = value;
                IDataReader reader = WrappedReader;
                if (reader != null) _ordinalCache.Init(reader);
            }
        }


        /// <summary>
        /// Gets and sets the number of rows to expect from the 
        /// <see cref="WrappedReader"/>. When the <c>WrappedReader</c> is also 
        /// an <see cref="ExtendedDataReaderWrapper"/> and this property is
        /// set, it will be further propagate into the <c>WrappedReader</c>.
        /// </summary>
        public virtual int RowsExpected
        {
            get { return _rowsExpected; }
            set
            {
                _rowsExpected = value;
                var reader = WrappedReader as ExtendedDataReaderWrapper;
                if (reader != null) reader.RowsExpected = _rowsExpected;
            }
        }

        /// <summary>
        /// Return the index of the named field by looking up in the
        /// <see cref="OrdinalCache"/> when it is not null, otherwise
        /// delegates to <see cref="WrappedReader"/>.
        /// </summary>
        ///
        /// <returns>
        /// The index of the named field.
        /// </returns>
        ///
        /// <param name="name">
        /// The name of the field to find. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public override int GetOrdinal(string name)
        {
            return _ordinalCache == null ?
                base.GetOrdinal(name) : _ordinalCache.GetOrdinal(name);
        }

        /// <summary>
        /// The underlying reader implementation to delegate to for accessing data 
        /// from a returned result sets.
        /// </summary>
        /// <value>
        /// The wrapped reader.
        /// </value>
        public override IDataReader WrappedReader
        {
            get
            {
                return base.WrappedReader;
            }
            set
            {
                base.WrappedReader = value;
                if (_ordinalCache != null) _ordinalCache.Init(value);
                var reader = value as ExtendedDataReaderWrapper;
                if (reader != null) reader.RowsExpected = _rowsExpected;
            }
        }

        /// <summary>
        /// Get the ultimate wrapped inner most reader by recursively gets the 
        /// wrapped reader when itself also a wrapper.
        /// </summary>
        /// <returns>The inner most original <see cref="IDataReader"/>.</returns>
        protected internal virtual IDataReader GetInnerMostReader()
        {
            var wrapped = WrappedReader;
            var wrapper = wrapped as IDataReaderWrapper;
            while (wrapper != null)
            {
                wrapped = wrapper.WrappedReader;
                wrapper = wrapped as IDataReaderWrapper;
            }
            return wrapped;
        }
    }
}
