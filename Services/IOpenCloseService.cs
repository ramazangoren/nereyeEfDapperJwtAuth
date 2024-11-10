using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Services
{
    public interface IOpenCloseService
    {
        bool? CheckIfOpenOrClosed(int restaurantId); // Allow null to indicate not found
    }
}
