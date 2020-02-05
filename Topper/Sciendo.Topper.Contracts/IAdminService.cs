using Sciendo.Topper.Contracts.DataTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sciendo.Topper.Contracts
{
    public interface IAdminService
    {
        int[] GetHistoryYears();

        ServerConfig GetServerConfig();
    }
}
