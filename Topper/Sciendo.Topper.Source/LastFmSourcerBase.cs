using System;
using System.Collections.Generic;
using System.Text;
using Sciendo.Last.Fm;
using Sciendo.Topper.Contracts;

namespace Sciendo.Topper.Source
{
    public abstract class LastFmSourcerBase<T>:ILastFmSourcer where T:class, new()
    {
        protected readonly IContentProvider<T> ContentProvider;

        protected abstract string LastFmMethod { get; }

        protected abstract string AdditionalParameters { get; }

        protected LastFmSourcerBase(IContentProvider<T> contentProvider)
        {
            ContentProvider = contentProvider;
        }

        public abstract List<TopItem> GetItems(string userName);
        public abstract void MergeSourceProperties(TopItem fromItem, TopItem toItem);
    }
}
