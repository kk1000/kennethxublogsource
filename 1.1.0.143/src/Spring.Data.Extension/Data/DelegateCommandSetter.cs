using System;
using System.Data;

namespace Spring.Data
{
    /// <summary>
    /// A convenient class to convert a deleate to <see cref="ICommandSetter"/>
    /// that can be used in various methods of <see cref="IAdoOperations"/>.
    /// </summary>
    public class DelegateCommandSetter : ICommandSetter
    {
        private readonly Action<IDbCommand> _commandSetterDelegate;

        /// <summary>
        /// Construct a new instance of <see cref="DelegateCommandSetter"/>.
        /// </summary>
        /// <param name="commandSetterDelegate"></param>
        public DelegateCommandSetter(Action<IDbCommand> commandSetterDelegate)
        {
            if (commandSetterDelegate == null)
            {
                throw new ArgumentNullException("commandSetterDelegate");
            }
            _commandSetterDelegate = commandSetterDelegate;
        }

        /// <summary>
        /// Calls the underlaying delegate with argument <paramref name="dbCommand"/>.
        /// </summary>
        /// <param name="dbCommand">
        /// A instance of <see cref="IDbCommand"/> to set the values.
        /// </param>
        public void SetValues(IDbCommand dbCommand)
        {
            _commandSetterDelegate(dbCommand);
        }
    }
}