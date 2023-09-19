using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System_Zarządzania_Magazynem
{
    public class ProductManager
    {
        private readonly Warehouse warehouse;
        private readonly object lockObject = new object();

        public ProductManager(Warehouse warehouse)
        {
            this.warehouse = warehouse;
        }

        public async Task AddProductAsync(Product product)
        {
            await Task.Run(() =>
            {
                lock (lockObject)
                {
                    if (!warehouse.Products.ContainsKey(product.Id))
                    {
                        warehouse.Products[product.Id] = product;
                        warehouse.OnProductAdded(product);
                    }
                    else
                    {
                        warehouse.Products[product.Id].Quantity += product.Quantity;
                    }
                }
            });
        }

        public async Task RemoveProductAsync(int productId, int quantity)
        {
            await Task.Run(() =>
            {
                lock (lockObject)
                {
                    if (warehouse.Products.ContainsKey(productId))
                    {
                        if (warehouse.Products[productId].Quantity >= quantity)
                        {
                            warehouse.Products[productId].Quantity -= quantity;
                            if (warehouse.Products[productId].Quantity == 0)
                            {
                                warehouse.Products.Remove(productId);
                            }
                            warehouse.OnProductRemoved(new ProductEventArgs(warehouse.Products[productId]));
                        }
                    }
                }
            });
        }
    }


}
