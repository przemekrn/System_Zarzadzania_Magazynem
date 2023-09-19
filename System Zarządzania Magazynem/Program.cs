using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System_Zarządzania_Magazynem
{
    class Program
    {
        static async Task Main()
        {
            var warehouse = new Warehouse();
            var shoppingCart = new Dictionary<int, int>();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Wybierz operację:");
                Console.WriteLine("1. Dodaj produkt do koszyka");
                Console.WriteLine("2. Zobacz dostępność produktów");
                Console.WriteLine("3. Pokaż zawartość koszyka");
                Console.WriteLine("4. Wyjdź z programu");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            ExecuteWithClearConsole(() => AddToCart(warehouse, shoppingCart));
                            break;
                        case 2:
                            ExecuteWithClearConsole(() => ShowAvailability(warehouse));
                            break;
                        case 3:
                            ExecuteWithClearConsole(() => ShowCartContents(shoppingCart, warehouse));
                            break;
                        case 4:
                            Environment.Exit(0);
                            break;
                        default:
                            ErrorLog("Niepoprawny wybór. Wybierz 1, 2, 3 lub 4.");
                            break;
                    }
                }
                else
                {
                    ErrorLog("Niepoprawny wybór. Wybierz 1, 2, 3 lub 4.");
                }
            }
        }

        static void ErrorLog(string source, ConsoleColor? kolor = null)
        {
            Console.ForegroundColor = kolor ?? ConsoleColor.Red;
            Console.WriteLine(source);
            Console.ResetColor();
        }

        static void ShowAvailability(Warehouse warehouse)
        {
            Console.WriteLine("Dostępność produktów:");
            ShowProducts(warehouse);
            Console.ReadKey();
        }

        static void ExecuteWithClearConsole(Action action)
        {
            Console.Clear();
            action();
            Console.Clear();

        }

        static void AddToCart(Warehouse warehouse, Dictionary<int, int> shoppingCart)
        {
            ShowProducts(warehouse);
            Console.Write("Podaj ID produktu do dodania do koszyka: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write("Podaj ilość: ");
                if (int.TryParse(Console.ReadLine(), out int quantity))
                {
                    if (warehouse[id] != null)
                    {
                        int availableQuantity = warehouse[id].Quantity;
                        if (shoppingCart.ContainsKey(id))
                        {
                            if (shoppingCart[id] + quantity <= availableQuantity)
                            {
                                shoppingCart[id] += quantity;
                                Console.WriteLine($"Dodano {quantity} sztuk produktu {warehouse[id].Name} do koszyka.");
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
                                Console.WriteLine($"Dodano {quantity} sztuk produktu {warehouse[id].Name} do koszyka.");
                            }
                            else
                            {
                                ErrorLog($"Nie można dodać więcej niż dostępna ilość ({availableQuantity}).");
                            }
                        }
                    }
                    else
                    {
                        ErrorLog("Produkt o podanym ID nie istnieje.");
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

            Console.ReadKey();
        }

        static void ShowProducts(Warehouse warehouse)
        {
            foreach (var productEntry in warehouse.Products)
            {
                Console.WriteLine($"|ID {productEntry.Value.Id}|{productEntry.Value.Name}: {productEntry.Value.Price}zł,  {productEntry.Value.Quantity} dostępne");
            }
        }

        static void ShowCartContents(Dictionary<int, int> shoppingCart, Warehouse warehouse)
        {
            Console.WriteLine("Zawartość koszyka:");
            decimal totalCost = 0;

            foreach (var kvp in shoppingCart)
            {
                int productId = kvp.Key;
                int quantity = kvp.Value;
                var product = warehouse[productId];

                if (product != null)
                {
                    decimal cost = product.Price * quantity;
                    totalCost += cost;
                    Console.WriteLine($"{product.Name}: {quantity} sztuk - Cena: {cost:C}");
                }
            }

            ErrorLog($"Łączna cena koszyka: {totalCost:C}", ConsoleColor.DarkGreen);
            Console.ReadKey();
        }
    }
}
