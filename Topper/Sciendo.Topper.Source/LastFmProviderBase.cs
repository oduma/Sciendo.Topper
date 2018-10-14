using System;
using System.Collections.Generic;
using System.Text;
using Sciendo.Last.Fm;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Source
{
    public abstract class LastFmProviderBase<T>:ITopItemsProvider where T:class, new()
    {
        protected readonly IContentProvider<T> ContentProvider;

        protected abstract string LastFmMethod { get; }

        protected abstract string AdditionalParameters { get; }

        protected LastFmProviderBase(IContentProvider<T> contentProvider)
        {
            ContentProvider = contentProvider ?? throw new ArgumentNullException(nameof(contentProvider));
        }

        public abstract List<TopItem> GetItems(string userName);
        public abstract void MergeSourceProperties(TopItem fromItem, TopItem toItem);
    }
}
