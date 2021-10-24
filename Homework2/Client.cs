using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework2
{
    class Client
    {
        public string Name { get; set; }

        public Dictionary<Product, int> BoughtProducts { get; } = new();

        public Client(string Name) => this.Name = Name;

        public void BuyProduct(Product p, int quantity, Shop shop)
        {
            try
            {
                var successfullyBought = shop.SellProduct(p, quantity);

                if (successfullyBought)
                {
                    if (!this.BoughtProducts.ContainsKey(p))
                    {
                        this.BoughtProducts[p] = 0;
                    }

                    this.BoughtProducts[p] += quantity;
                    Console.WriteLine($"\t{Name} bought {p.Name} x {quantity}");
                }
                else
                {
                    Console.WriteLine($"\t{Name} couldn't buy {p.Name} x {quantity}: Not enough in shop.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\t{Name} couldn't buy product {p.Name}: {e.Message}");
            }
        }

        public void BuyProduct(Product p, Shop shop)
        {
            BuyProduct(p, 1, shop);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
