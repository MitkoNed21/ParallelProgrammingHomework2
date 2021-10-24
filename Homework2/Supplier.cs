using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework2
{
    class Supplier
    {
        public string Name { get; set; }

        public void SupplyProduct(Product p, int quantity, Shop shop)
        {
            try
            {
                shop.AddProduct(p, quantity);
                Console.WriteLine($"{Name} supplied shop with {quantity} of {p.Name}.");
            } catch (Exception e)
            {
                Console.WriteLine($"{Name} couldn't supply product {p.Name}: {e.Message}");
            }
        }

        public void SupplyProduct(Product p, Shop shop)
        {
            shop.AddProduct(p);
        }

        public Supplier(string name) => this.Name = name;

        public override string ToString()
        {
            return Name;
        }
    }
}
