using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework2
{
    class Shop
    {
        private object lockObj = new();

        public Dictionary<Product, int> Products { get; } = new();

        public void AddProduct(Product p, int quantity = 1)
        {
            if (quantity < 1)
            {
                throw new ArgumentOutOfRangeException($"Quantity can't be 0 or negative!");
            }

            lock (lockObj)
            {
                if (!this.Products.ContainsKey(p))
                {
                    this.Products[p] = 0;
                }
            
                this.Products[p] += quantity;
            }
        }

        public bool SellProduct(Product p, int quantity = 1)
        {
            if (quantity < 1)
            {
                throw new ArgumentOutOfRangeException($"Quantity can't be 0 or negative!");
            }
            
            if (this.Products[p] - quantity < 0)
            {
                return false;
            }

            lock (lockObj)
            {
                this.Products[p] -= quantity;
            }

            return true;
        }
    }
}
