using System;
using System.Threading;

namespace DapperExtensions.Internal
{
    /// <summary>
    /// 线程安全帮助程序，用于管理 线程安全的值
    ///  Thread-safe helper to manage a value.
    /// </summary>
    internal class InterlockedInt32
    {
        #region Fields
        private int _value;
        #endregion

        #region Constructors
        public InterlockedInt32(int initialValue)
        {
            _value = initialValue;
        }
        #endregion

        #region Properties
        public int Value
        {
            get { return Interlocked.CompareExchange(ref _value, 0, 0); }
        }
        #endregion

        #region Methods
        public bool TryChange(int toValue)
        {
            return Interlocked.Exchange(ref _value, toValue) != toValue;
        }

        public bool TryChange(int fromValue, int toValue)
        {
            if (fromValue == toValue)
            {
                throw new ArgumentException("fromValue and toValue must be different.");
            }
            return Interlocked.CompareExchange(ref _value, toValue, fromValue) == fromValue;
        }
        #endregion
    }
}
