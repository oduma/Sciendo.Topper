using Sciendo.Topper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service
{
    public interface IPictureReader
    {
        NamedPicture Read(string artistName, string imageData);
    }
}
