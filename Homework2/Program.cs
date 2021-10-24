using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Homework2
{
    class Program
    {
        private static List<Thread> clientsThreads = new();
        private static List<Thread> suppliersThreads = new();

        private static Shop shop = new Shop();
        private static Random random = new Random();

        private static List<Client> clients = new();
        private static List<Supplier> suppliers = new();
        private static List<Product> products = new();

        private static Dictionary<Supplier, Dictionary<Product, int>> supplierSuppliedProducts { get; } = new();

        static void Main(string[] args)
        {
            InitialSetup();

            /*\
             * Clients will buy 1-7 items
             * Suppliers will supply 20-30 items 5 times as less as clients buy
             * Time between purchases will be random 200-1000ms
             * Time between supplies will be random 1000-5000ms
             * 
            \*/

            Console.WriteLine("Clients:\n\t" + String.Join("\n\t", clients));
            Console.WriteLine(new String('=', 40));
            Console.WriteLine("Suppliers:\n\t" + String.Join("\n\t", suppliers));
            Console.WriteLine(new String('=', 40));
            Console.WriteLine("Shop:\n\t" + String.Join("\n\t", shop.Products.Select(
                productInfoKVP => $"{productInfoKVP.Key.Name}: {productInfoKVP.Value}"
            )));
            Console.WriteLine(new String('=', 40));
            Console.WriteLine("Running simulation...");

            for (int i = 0; i < clients.Count; i++)
            {
                clientsThreads[i].Start(clients[i]);
            }

            for (int i = 0; i < suppliers.Count; i++)
            {
                suppliersThreads[i].Start(suppliers[i]);
            }

            var finished = false;
            while(!finished)
            {
                if (!clientsThreads.Any(t => !t.Join(0)) &&
                    !suppliersThreads.Any(t => !t.Join(0)))
                {
                    finished = true;
                }
            }

            Console.WriteLine("Results:");
            Console.WriteLine(new String('=', 40));
            Console.WriteLine("Client purchases:");
            foreach (var client in clients)
            {
                Console.WriteLine(client.Name);
                Console.WriteLine("\t" + String.Join("\n\t", client.BoughtProducts.Select(
                    productInfoKVP => $"{productInfoKVP.Key.Name}: {productInfoKVP.Value}"
                )));
            }
            Console.WriteLine(new String('=', 40));
            Console.WriteLine("Suppliers' supplies:");
            foreach (var (supplier, productsInfo) in supplierSuppliedProducts)
            {
                Console.WriteLine(supplier.Name);
                Console.WriteLine("\t" + String.Join("\n\t", productsInfo.Select(
                    productInfoKVP => $"{productInfoKVP.Key.Name}: {productInfoKVP.Value}"
                )));
            }
            Console.WriteLine(new String('=', 40));
            Console.WriteLine("Shop:\n\t" + String.Join("\n\t", shop.Products.Select(
                kvp => $"{kvp.Key.Name}: {kvp.Value}"
            )));

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }


        private static List<Product> CreateProducts()
        {
            return new List<Product>()
            {
                new Product("iPhone 12",           price: 2000, discount: 200),
                new Product("Acer Laptop",         price: 1500, discount: 140),
                new Product("A++ Fridge",          price: 980,  discount: 130),
                new Product("Custom PC",           price: 1300, discount: 210),
                new Product("Samsung Galaxy s20",  price: 1400, discount: 280),
                new Product("Huawei Tablet",       price: 560,  discount: 90),
                new Product("Samsung 42-in TV",    price: 830,  discount: 150),
                new Product("Family Tent",         price: 240,  discount: 45),
                new Product("HP Printer",          price: 120,  discount: 30),
                new Product("3D Printer",          price: 5000, discount: 500),
                new Product("Bike",                price: 254,  discount: 46),
                new Product("Smart Watch",         price: 340,  discount: 38),
                new Product("A+ Air Conditioner",  price: 630,  discount: 78),
                new Product("Air Fryer",           price: 320,  discount: 50),
                new Product("Canon Camera",        price: 650,  discount: 110),
                new Product("Wireless Headphones", price: 70,   discount: 15),
                new Product("Smart Lights",        price: 130,  discount: 24),
                new Product("Microsoft Surface",   price: 2400, discount: 240),
                new Product("Apple Mac",           price: 4300, discount: 400),
                new Product("iPad Pro",            price: 2500, discount: 380)
            };
        }

        private static void InitialSetup()
        {
            products = CreateProducts();
            foreach (var p in products)
            {
                shop.AddProduct(p, random.Next(40, 310));
            }

            var clientsCount = random.Next(56, 101);
            var suppliersCount = random.Next(1, 6);

            var firstNames = new[]
            {
                "Yulian","Tihomir","Panayot","Yordan","Zdravko","Zhivko","Vasil","Martin","Ivaylo","Mitko","Dimitar", "Daniel", "Valentin", "Slav", "Iliyan", "Ivan", "Kaloyan", "Stoyan", "Petar", "Georgi", "Konstantin", "Mihail", "Radomir", "Nikolay"
            };

            var lastNames = new[]
            {
                "Yulianov","Tihomirov","Panayotov","Yordanov","Zdravkov","Zhivkov","Vasilev","Martinov","Ivaylov","Mitev","Dimitrov", "Danielov","Valentinov", "Slavov", "Iliyanov", "Ivanov", "Kaloyanov", "Stoyanov", "Petrov", "Georgiev", "Konstantinov", "Mihailov", "Radomirov", "Nikolaev"
            };

            for (int i = 0; i < clientsCount; i++)
            {
                var fName = firstNames[random.Next(0, firstNames.Length)];
                var lName = lastNames[random.Next(0, lastNames.Length)];
                var client = new Client(fName + " " + lName);

                clients.Add(client);
                clientsThreads.Add(new Thread(ClientWorker));
            }

            for (int i = 0; i < suppliersCount; i++)
            {
                var fName = firstNames[random.Next(0, firstNames.Length)];
                var lName = lastNames[random.Next(0, lastNames.Length)];
                var supplier = new Supplier(fName + " " + lName);

                suppliers.Add(supplier);
                suppliersThreads.Add(new Thread(SupplierWorker));
            }
        }

        static public void ClientWorker(object clientObj)
        {
            var client = (Client)clientObj;

            var purchasesCount = random.Next(25, 76);

            for (int i = 0; i < purchasesCount; i++)
            {
                var boughtProductsInCurrentSession = new List<Product>();

                // Client buys at most 3 different products in a session
                var productsToBuyCount = random.Next(1, 4);
                for (int j = 0; j < productsToBuyCount; j++)
                {
                    var product = products[random.Next(0, products.Count)];
                    var quantity = random.Next(1, 7);

                    if (boughtProductsInCurrentSession.Contains(product))
                    {
                        j--;
                        continue;
                    }

                    client.BuyProduct(product, quantity, shop);
                    boughtProductsInCurrentSession.Add(product);
                }

                // simulate time passing
                Thread.Sleep(random.Next(2, 10));
            }
        }

        static public void SupplierWorker(object supplierObj)
        {
            var supplier = (Supplier)supplierObj;

            var suppliesCount = random.Next(12, 28);
            suppliesCount *= Math.Max(1, 2 - suppliers.Count);

            for (int i = 0; i < suppliesCount; i++)
            {
                var suppliedProductsInCurrentSupply = new List<Product>();

                // Supplier sends 7 different items each when supplying
                for (int j = 0; j < 7; j++)
                {
                    // Compiler can't see that variable "product"
                    // will always be initialized
                    Product product = null;
                    var quantity = random.Next(14, 65);
                    var foundUrgentSupply = false;

                    foreach (var productInfo in shop.Products)
                    {
                        var shopQuantity = productInfo.Value;
                        if (shopQuantity < 10)
                        {
                            product = productInfo.Key;
                            quantity *= 2;
                            foundUrgentSupply = true;
                            suppliedProductsInCurrentSupply.Add(product);
                            break;
                        }
                    }
                    
                    if (!foundUrgentSupply)
                    {
                        product = products[random.Next(0, products.Count)];

                        if (suppliedProductsInCurrentSupply.Contains(product))
                        {
                            j--;
                            continue;
                        }
                    }

                    // if less suppliers, more products supplies
                    quantity *= Math.Max(1, 3 - suppliers.Count);

                    supplier.SupplyProduct(product, quantity, shop);
                    if (!supplierSuppliedProducts.ContainsKey(supplier))
                    {
                        supplierSuppliedProducts.Add(supplier, new Dictionary<Product, int>());
                    }
                    
                    if (!supplierSuppliedProducts[supplier].ContainsKey(product))
                    {
                        supplierSuppliedProducts[supplier][product] = 0;
                    }

                    supplierSuppliedProducts[supplier][product] += quantity;
                    suppliedProductsInCurrentSupply.Add(product);
                }

                // simulate time passing
                // if less suppliers, more frequent supplies
                var minTime = 8 / Math.Max(1, 2 - suppliers.Count);
                var maxTime = 40 / Math.Max(1, 2 - suppliers.Count);
                Thread.Sleep(random.Next(minTime, maxTime));
            }
        }
    }
}
