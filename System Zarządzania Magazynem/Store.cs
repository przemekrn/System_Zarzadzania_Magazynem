using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System_Zarządzania_Magazynem
{
    public class Store
    {
        private Dictionary<int, Product> products = new Dictionary<int, Product>();
        private readonly object lockObject = new object();

        public Store()
        {
            AddDefaultProducts();
        }

        private void AddDefaultProducts()
        {
            products.Add(1, new Product { Id = 1, Name = "Monitor", Price = 300, Quantity = 10 });
            products.Add(2, new Product { Id = 2, Name = "Klawiatura", Price = 50, Quantity = 20 });
            products.Add(3, new Product { Id = 3, Name = "Myszka", Price = 30, Quantity = 15 });
        }

        public Product this[int productId]
        {
            get
            {
                lock (lockObject)
                {
                    if (products.ContainsKey(productId))
                    {
                        return products[productId];
                    }
                    return null;
                }
            }
        }

        public Dictionary<int, Product> Products
        {
            get { return products; }
        }

        public event EventHandler<ProductEventArgs> ProductAdded;

        public Product GetAvailableProduct(int productId)
        {
            lock (lockObject)
            {
                if (products.ContainsKey(productId) && products[productId].Quantity > 0)
                {
                    return products[productId];
                }
                return null;
            }
        }

        public void AddProduct(Product product)
        {
            lock (lockObject)
            {
                if (!products.ContainsKey(product.Id))
                {
                    products[product.Id] = product;
                    OnProductAdded(product);
                }
                else
                {
                    products[product.Id].Quantity += product.Quantity;
                }
            }
        }

        public virtual void OnProductAdded(Product product)
        {
            ProductAdded?.Invoke(this, new ProductEventArgs(product));
        }

        public void RemoveProduct(int productId, int quantity)
        {
            lock (lockObject)
            {
                if (products.ContainsKey(productId) && products[productId].Quantity >= quantity)
                {
                    products[productId].Quantity -= quantity;
                    if (products[productId].Quantity == 0)
                    {
                        products.Remove(productId);
                    }
                }
            }
        }
    }
}
