using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System_Zarządzania_Magazynem
{

    internal class StoreData
    {
        public Store Store { get; set; }
        public Dictionary<int, int> ShoppingCart { get; set; }
    }
}
