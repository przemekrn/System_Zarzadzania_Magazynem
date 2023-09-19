using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System_Zarządzania_Magazynem
{
    public class ProductEventArgs : EventArgs
    {
        public Product Product { get; }

        public ProductEventArgs(Product product)
        {
            Product = product;
        }
    }
}
