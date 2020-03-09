using Sciendo.Topper.Domain;
using System.Collections.Generic;

namespace Sciendo.Topper.Store
{
    internal class TopItemEqualityComparer : IEqualityComparer<TopItem>
    {
        public bool Equals(TopItem x, TopItem y)
        {
            if (x == null)
                return false;
            if (y == null)
                return false;
            if (x.Name == y.Name)
                return true;
            return false;
        }

        public int GetHashCode(TopItem obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}