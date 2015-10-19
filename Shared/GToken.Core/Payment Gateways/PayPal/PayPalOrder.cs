using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform.Core.Payment_Gateways.PayPal
{
    public class PayPalOrder
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public float Price { get; set; }


        public PayPalOrder(string itemId,string itemName, float price)
        {
            ItemId = itemId;
            ItemName = itemName;
            Price = price;
        }
    }
}
