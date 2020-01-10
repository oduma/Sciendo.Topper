using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Domain.Entities
{
    public class NamedPicture
    {
        public string Name { get; set; }

        public byte[] Picture { get; set; }
        public string Extension { get; set; }
    }
}
