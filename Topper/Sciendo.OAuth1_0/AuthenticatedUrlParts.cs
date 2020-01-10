using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.OAuth1_0
{
    internal class AuthenticatedUrlParts
    {
        public string RootUrl { get; set; }

        public string Parameters { get; set; }

        public string Signature { get; set; }
    }
}
