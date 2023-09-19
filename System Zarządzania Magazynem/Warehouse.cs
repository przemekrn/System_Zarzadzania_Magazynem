using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System_Zarządzania_Magazynem
{
    public class Warehouse
    {
        private Dictionary<int, Product> products = new Dictionary<int, Product>();
        private readonly object lockObject = new object();

        public Warehouse()
        {
            AddDefaultProducts();
        }

        private void AddDefaultProducts()
        {
            products.Add(1, new Product { Id = 1, Name = "Laptop", Price = 1000, Quantity = 5 });
            products.Add(2, new Product { Id = 2, Name = "Smartphone", Price = 500, Quantity = 8 });
            products.Add(3, new Product { Id = 3, Name = "Telewizor", Price = 1500, Quantity = 3 });
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
        public event EventHandler<ProductEventArgs> ProductRemoved;

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

        public void RemoveProduct(int productId, int quantity)
        {
            lock (lockObject)
            {
                if (products.ContainsKey(productId))
                {
                    if (products[productId].Quantity >= quantity)
                    {
                        products[productId].Quantity -= quantity;
                        if (products[productId].Quantity == 0)
                        {
                            products.Remove(productId);
                        }
                        OnProductRemoved(new ProductEventArgs(products[productId]));
                    }
                }
            }
        }

        public virtual void OnProductAdded(Product product)
        {
            ProductAdded?.Invoke(this, new ProductEventArgs(product));
        }

        public virtual void OnProductRemoved(ProductEventArgs e)
        {
            ProductRemoved?.Invoke(this, e);
        }
    }

}
