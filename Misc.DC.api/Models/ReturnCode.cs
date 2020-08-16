using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Misc.DC.api.Models
{
    public enum ReturnCode
    {
        ServerError = 400,
        ServerOK = 200,
        FitalError = 500,
        ProcessExisted = 222
    }
}
