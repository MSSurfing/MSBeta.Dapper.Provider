using System;
using System.Threading;

namespace DapperExtensions.Internal
{
    /// <summary>
    /// 引用&释放 计数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class ReferenceCounted<T> where T : class, IDisposable
    {
        #region Fields
        private readonly T _instance;
        private readonly Action<T> _release;
        private int _referenceCount;
        #endregion

        #region  Ctor
        public ReferenceCounted(T instance)
            : this(instance, x => x.Dispose())
        {
        }

        public ReferenceCounted(T instance, Action<T> release)
        {
            _instance = instance;
            _release = release;
            _referenceCount = 1;
        }
        #endregion

        #region Properties
        public int ReferenceCount
        {
            get { return Interlocked.CompareExchange(ref _referenceCount, 0, 0); }
        }

        public T Instance
        {
            get { return _instance; }
        }
        #endregion

        #region Methods
        public void DecrementReferenceCount()
        {
            var value = Interlocked.Decrement(ref _referenceCount);
            if (value == 0)
            {
                _release(_instance);
            }
        }

        public void IncrementReferenceCount()
        {
            Interlocked.Increment(ref _referenceCount);
        }
        #endregion
    }
}
