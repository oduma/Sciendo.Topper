using System.Collections.Generic;

namespace Sciendo.OAuth1_0
{


    /// <summary>
    /// Comparer class used to perform the sorting of the query parameters
    /// </summary>
    internal class QueryParameterComparer : IComparer<QueryParameter>
    {

        #region IComparer<QueryParameter> Members

        public int Compare(QueryParameter x, QueryParameter y)
        {
            if (x.Name == y.Name)
            {
                return string.Compare(x.Value, y.Value);
            }
            else
            {
                return string.Compare(x.Name, y.Name);
            }
        }

        #endregion
    }
}
