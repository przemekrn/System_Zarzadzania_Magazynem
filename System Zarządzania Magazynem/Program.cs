using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace System_Zarządzania_Magazynem
{
    class Program
    {
        private const string DataFilePath = "store_data.json";
        static async Task Main()
        {
            Store store;
            var shoppingCart = new Dictionary<int, int>();
            if (File.Exists(DataFilePath))
            {
                string jsonData = File.ReadAllText(DataFilePath);
                var data = JsonConvert.DeserializeObject<StoreData>(jsonData);
                store = data.Store;
                shoppingCart = data.ShoppingCart;
            }
            else
            {
                store = new Store();
                shoppingCart = new Dictionary<int, int>();
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Wybierz operację:");
                Console.WriteLine("1. Dodaj produkt do koszyka");
                Console.WriteLine("2. Zobacz dostępność produktów w sklepie");
                Console.WriteLine("3. Pokaż zawartość koszyka");
                Console.WriteLine("4. Dodaj nowy produkt do sklepu");
                Console.WriteLine("5. Wyjdź z programu");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            ExecuteWithClearConsole(() => AddToCart(store, shoppingCart));
                            break;
                        case 2:
                            ExecuteWithClearConsole(() => ShowAvailability(store));
                            break;
                        case 3:
                            ExecuteWithClearConsole(() => ShowCartContents(shoppingCart, store));
                            break;
                        case 4:
                            ExecuteWithClearConsole(() => AddNewProductToStore(store));
                            break;
                        case 5:
                            Environment.Exit(0);
                            break;
                        default:
                            ErrorLog("Niepoprawny wybór. Wybierz liczbę od 1 do 5.");
                            break;
                    }
                }
                else
                {
                    ErrorLog("Niepoprawny wybór. Wybierz liczbę od 1 do 5.");
                }
            }
        }

        static void ErrorLog(string source, ConsoleColor? kolor = null)
        {
            Console.ForegroundColor = kolor ?? ConsoleColor.Red;
            Console.WriteLine(source);
            Console.ResetColor();
            Console.ReadKey();
        }

        static void ShowAvailability(Store store)
        {
            Console.WriteLine("Dostępność produktów w sklepie:");
            ShowProducts(store.Products.Values);
            Console.ReadKey();
        }

        static void ExecuteWithClearConsole(Action action)
        {
            Console.Clear();
            action();
        }

        static void AddToCart(Store store, Dictionary<int, int> shoppingCart)
        {
            ShowProducts(store.Products.Values);

            // Dodawanie produktu do koszyka
            Console.Write("Podaj ID produktu do dodania do koszyka: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write("Podaj ilość: ");
                if (int.TryParse(Console.ReadLine(), out int quantity))
                {
                    var product = store.GetAvailableProduct(id);
                    if (product != null)
                    {
                        int availableQuantity = product.Quantity;
                        if (shoppingCart.ContainsKey(id))
                        {
                            if (shoppingCart[id] + quantity <= availableQuantity)
                            {
                                shoppingCart[id] += quantity;
                                Console.WriteLine($"Dodano {quantity} sztuk produktu {product.Name} do koszyka.");
                            }
                            else
                            {
                                ErrorLog($"Nie można dodać więcej niż dostępna ilość ({availableQuantity}).");
                            }
                        }
                        else
                        {
                            if (quantity <= availableQuantity)
                            {
                                shoppingCart[id] = quantity;
                                Console.WriteLine($"Dodano {quantity} sztuk produktu {product.Name} do koszyka.");
                            }
                            else
                            {
                                ErrorLog($"Nie można dodać więcej niż dostępna ilość ({availableQuantity}).");
                            }
                        }
                    }
                    else
                    {
                        ErrorLog("Produkt o podanym ID nie istnieje w sklepie.");
                    }
                }
                else
                {
                    ErrorLog("Niepoprawna ilość.");
                }
            }
            else
            {
                ErrorLog("Niepoprawne ID produktu.");
            }
        }

        static void ShowProducts(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                Console.WriteLine($"|ID {product.Id}|{product.Name}: {product.Price}zł,  {product.Quantity} dostępne");
            }
        }

        static void ShowCartContents(Dictionary<int, int> shoppingCart, Store store)
        {
            Console.WriteLine("Zawartość koszyka:");
            decimal totalCost = 0;

            foreach (var kvp in shoppingCart)
            {
                int productId = kvp.Key;
                int quantity = kvp.Value;
                var product = store.GetAvailableProduct(productId);

                if (product != null)
                {
                    decimal cost = product.Price * quantity;
                    totalCost += cost;
                    Console.WriteLine($"{product.Name}: {quantity} sztuk - Cena: {cost:C}");
                }
            }

            ErrorLog($"Łączna cena koszyka: {totalCost:C}", ConsoleColor.DarkGreen);
        }

        static void AddNewProductToStore(Store store)
        {
            Console.Write("Podaj nazwę nowego produktu: ");
            string name = Console.ReadLine();

            Console.Write("Podaj cenę nowego produktu: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.Write("Podaj ilość nowego produktu: ");
                if (int.TryParse(Console.ReadLine(), out int quantity))
                {
                    int maxProductId = store.Products.Keys.Max();
                    int newProductId = maxProductId + 1;
                    var newProduct = new Product { Id = newProductId, Name = name, Price = price, Quantity = quantity };

                    store.Products.Add(newProductId, newProduct);
                    Console.WriteLine($"Dodano nowy produkt do sklepu: {name}");
                }
                else
                {
                    ErrorLog("Niepoprawna ilość.");
                }
            }
            else
            {
                ErrorLog("Niepoprawna cena.");
            }

            Console.ReadKey();
        }
    }
}
