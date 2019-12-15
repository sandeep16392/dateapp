using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Date.API.Configs
{
    public interface ICommonConfigurations
    {
        string Token { get; }
        string ConnectionString { get;  }
    }
}
