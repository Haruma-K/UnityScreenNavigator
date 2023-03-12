using System;
using System.Collections.Generic;

namespace Demo.Subsystem.Misc
{
    public interface IDisposableCollectionHolder
    {
        ICollection<IDisposable> GetDisposableCollection();
    }

    public static class DisposableExtensions
    {
        public static void AddTo(this IDisposable disposable, IDisposableCollectionHolder holder)
        {
            if (disposable == null) throw new ArgumentNullException(nameof(disposable));
            if (holder == null) throw new ArgumentNullException(nameof(holder));

            holder.GetDisposableCollection().Add(disposable);
        }
    }
}
