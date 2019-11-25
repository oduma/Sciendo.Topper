using Sciendo.Topper.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Service
{
    public class AdminService : IAdminService
    {
        public int[] GetHistoryYears()
        {
            return new[]{ 2018, 2019};
        }
    }
}
