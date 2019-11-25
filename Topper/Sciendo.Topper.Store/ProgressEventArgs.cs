using System;
using Sciendo.Topper.Domain;

namespace Sciendo.Topper.Store
{
    public class ProgressEventArgs:EventArgs
    {
        public TopItem TopItem { get; private set; }

        public Status Status { get; private set; }

        public ProgressEventArgs(TopItem topItem, Status status)
        {
            Status = status;
            TopItem = topItem;
        }
    }
}