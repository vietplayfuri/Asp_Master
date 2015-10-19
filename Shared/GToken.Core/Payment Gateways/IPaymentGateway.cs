using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Core.Payment_Gateways
{
    interface IPaymentGateway
    {
        bool Test1();
        bool Test2();
    }
}
