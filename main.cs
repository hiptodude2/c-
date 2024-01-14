using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Android.OS;

namespace AdvancedTycoonGame
{
    public interface IMod
    {
        void ApplyMod(List<Business> businesses, UI ui, GUI gui);
    }

    public class UI
    {
        public string GameTitle { get; set; }
        public string MainMenuTitle { get; set; }
        public List<string> MainMenuOptions { get; set; }

        public void DisplayMainMenu()
        {
            Console.WriteLine(GameTitle);
            Console.WriteLine(MainMenuTitle);

            for (int i = 0; i < MainMenuOptions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {MainMenuOptions[i]}");
            }

            Console.Write("Enter your choice: ");
        }
    }

    public class GUI
    {
        public string ModManagementTitle { get; set; }
        public List<string> ModManagementOptions { get; set; }

        public void DisplayModManagementMenu()
        {
            Console.Clear();
            Console.WriteLine(ModManagementTitle);

            for (int i = 0; i < ModManagementOptions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {ModManagementOptions[i]}");
            }

            Console.Write("Enter your choice: ");
        }
    }

    public class Business
    {
        public string Name { get; set; }
        public int Cost { get; }
        public int Income { get; }
        public double IncomeMultiplier { get; }
        public int Quantity { get; set; }

        public Business(string name, int baseCost, int income, double incomeMultiplier)
        {
            Name = name;
            Cost = baseCost;
            Income = income;
            IncomeMultiplier = incomeMultiplier;
            Quantity = 0;
        }
    }

    public class TycoonGame
    {
        private List<Business> businesses;
        private double money = 1000;
        private int day = 1;
        private double difficultyMultiplier = 1.2;
        private string modsDirectory;
        private UI ui;
        private GUI gui;

        public TycoonGame()
        {
            businesses = new List<Business>
            {
                new Business("Generic Business 1", 200, 50, 1.1),
                new Business("Generic Business 2", 300, 70, 1.3),
            };

            ui = new UI
            {
                GameTitle = "Tycoon Game",
                MainMenuTitle = "Tycoon Game",
                MainMenuOptions = new List<string>
                {
                    "Buy Business",
                    "Sell Business",
                    "Sleep",
                    "Display Stats",
                    "Manage Mods",
                    "Quit"
                }
            };

            gui = new GUI
            {
                ModManagementTitle = "Mod Management",
                ModManagementOptions = new List<string>
                {
                    "Apply Mods",
                    "Load Mod",
                    "Unload Mod",
                    "Back"
                }
            };

            SetModsDirectory();
            LoadMods();
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader();
                ui.DisplayMainMenu();

                char choice = Console.ReadKey().KeyChar;

                switch (choice)
                {
                    case '1':
                        BuyBusiness();
                        break;

                    case '2':
                        SellBusiness();
                        break;

                    case '3':
                        MoneyManagement();
                        break;

                    case '4':
                        DisplayStats();
                        break;

                    case '5':
                        ManageMods();
                        break;

                    case '6':
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("\nInvalid choice. Please try again.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        private void DisplayHeader()
        {
            Console.WriteLine($"Day: {day} | Money: ${money:F2}\n");
        }

        private void BuyBusiness()
        {
            Console.Clear();
            DisplayHeader();
            Console.WriteLine("Available Businesses:\n");

            for (int i = 0; i < businesses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {businesses[i].Name} - Cost: ${businesses[i].Cost:F2}");
            }

            Console.Write("Choose a business to buy (or press 'B' to go back): ");
            char subChoice = Console.ReadKey().KeyChar;

            if (subChoice == 'B' || subChoice == 'b')
                return;

            if (char.IsDigit(subChoice) && int.TryParse(subChoice.ToString(), out int subIndex) && subIndex > 0 && subIndex <= businesses.Count)
            {
                Business selectedBusiness = businesses[subIndex - 1];

                if (selectedBusiness.Quantity == 0 && money >= selectedBusiness.Cost)
                {
                    money -= selectedBusiness.Cost;
                    selectedBusiness.Quantity++;
                    Console.WriteLine($"\nYou bought a {selectedBusiness.Name} for ${selectedBusiness.Cost:F2}. " +
                                      $"It generates ${selectedBusiness.Income * selectedBusiness.Quantity * difficultyMultiplier:F2}/day.");
                }
                else if (selectedBusiness.Quantity > 0)
                {
                    Console.WriteLine($"\nYou already own a {selectedBusiness.Name}.");
                }
                else
                {
                    Console.WriteLine("\nNot enough money to buy this business.");
                }
            }
            else
            {
                Console.WriteLine("\nInvalid choice. Please try again.");
            }
        }

        private void SellBusiness()
        {
            Console.Clear();
            DisplayHeader();
            Console.WriteLine("Select a business to sell:\n");

            for (int i = 0; i < businesses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {businesses[i].Name} {(businesses[i].Quantity > 0 ? $"(Owned: {businesses[i].Quantity})" : "")}");
            }

            Console.Write("Enter your choice: ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= businesses.Count)
            {
                Business selectedBusiness = businesses[choice - 1];

                if (selectedBusiness.Quantity > 0)
                {
                    money += selectedBusiness.Cost / 2; // Sell for half the price
                    selectedBusiness.Quantity--;
                    Console.WriteLine($"\nYou sold one {selectedBusiness.Name} for ${selectedBusiness.Cost / 2:F2}. " +
                                      $"Remaining: {selectedBusiness.Quantity}.");
                }
                else
                {
                    Console.WriteLine($"\nYou don't own a {selectedBusiness.Name} to sell.");
                }
            }
            else
            {
                Console.WriteLine("\nInvalid choice. Please try again.");
            }
        }

        private void MoneyManagement()
        {
            Console.WriteLine("\nYou decided to take a break and sleep.");
            Console.WriteLine("Zzz... Zzz... Zzz...");
            Console.WriteLine("You wake up feeling refreshed.");

            // Simulate a day passing only when sleeping
            SimulateDay();
        }

        private void DisplayStats()
        {
            Console.Clear();
            DisplayHeader();
            Console.WriteLine("Business Stats:\n");

            foreach (Business business in businesses)
            {
                Console.WriteLine($"{business.Name} - " +
                                  $"Cost: ${business.Cost:F2}, Income: ${business.Income * business.Quantity:F2}/day" +
                                  $" {(business.Quantity > 0 ? $"(Owned: {business.Quantity})" : "")}");
            }
        }

        private void SimulateDay()
        {
            day++;

            foreach (Business business in businesses)
            {
                money += business.Income * business.Quantity * difficultyMultiplier;
            }
        }

        private void SetModsDirectory()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix ||
                Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                modsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "mods");
            }
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                modsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "mods");
            }
            else
            {
                // Adjust this path according to your Android app structure
                if (Environment.OSVersion.VersionString.Contains("Android"))
                {
                    string androidDataDir = "Android/data/com.companyname.helloworld/cache";
                    modsDirectory = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, androidDataDir, "mods");
                }
                else
                {
                    // Handle other platforms if needed
                }
            }

            if (!Directory.Exists(modsDirectory))
                Directory.CreateDirectory(modsDirectory);
        }

        private void LoadMods()
        {
            string[] modFiles = Directory.GetFiles(modsDirectory, "*.*");

            foreach (string modFile in modFiles)
            {
                try
                {
                    string modScript = File.ReadAllText(modFile);
                    var compiledMod = CompileMod(modScript);
                    ApplyMod(compiledMod);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading mod from file {modFile}: {ex.Message}");
                }
            }
        }

        private void ApplyMod(dynamic compiledMod)
        {
            if (compiledMod is IMod mod)
            {
                mod.ApplyMod(businesses, ui, gui);
            }
            else
            {
                Console.WriteLine("Invalid mod format. Mods must implement the IMod interface.");
            }
        }

        private void ManageMods()
        {
            while (true)
            {
                Console.Clear();
                DisplayHeader();
                gui.DisplayModManagementMenu();

                char modChoice = Console.ReadKey().KeyChar;

                switch (modChoice)
                {
                    case '1':
                        ApplyMods();
                        break;

                    case '2':
                        LoadMod();
                        break;

                    case '3':
                        UnloadMod();
                        break;

                    case '4':
                        return;

                    default:
                        Console.WriteLine("\nInvalid choice. Please try again.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        private void ApplyMods()
        {
            LoadMods();
            Console.WriteLine("\nMods applied successfully!");
        }

        private void LoadMod()
        {
            Console.Clear();
            Console.WriteLine("Enter the name of the mod file to load (with or without extension): ");
            string modFileName = Console.ReadLine();

            string modFilePath = Path.Combine(modsDirectory, modFileName);

            if (File.Exists(modFilePath))
            {
                try
                {
                    string modScript = File.ReadAllText(modFilePath);
                    var compiledMod = CompileMod(modScript);
                    ApplyMod(compiledMod);
                    Console.WriteLine($"\nMod '{modFileName}' loaded successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading mod '{modFileName}': {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Mod '{modFileName}' not found.");
            }
        }

        private void UnloadMod()
        {
            Console.Clear();
            Console.WriteLine("Enter the name of the mod file to unload (with or without extension): ");
            string modFileName = Console.ReadLine();

            if (businesses.Any(b => b.Name == modFileName))
            {
                Console.WriteLine($"Cannot unload mod '{modFileName}' as it is applied directly in the game code.");
            }
            else
            {
                var modToRemove = businesses.OfType<IMod>().FirstOrDefault(mod => mod.GetType().Name == modFileName);

                if (modToRemove != null)
                {
                    businesses.Remove(modToRemove as Business);
                    Console.WriteLine($"Mod '{modFileName}' unloaded successfully!");
                }
                else
                {
                    Console.WriteLine($"Mod '{modFileName}' not found.");
                }
            }
        }

        private dynamic CompileMod(string modScript)
        {
            // For simplicity, we won't implement dynamic compilation here.
            // In a real scenario, you may use Roslyn scripting or a similar approach.

            // For now, we'll look for mod classes in the current assembly
            // This assumes that the mod classes are defined in the same assembly as the game
            Type modType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name.Equals(modScript));

            if (modType != null && typeof(IMod).IsAssignableFrom(modType))
            {
                return Activator.CreateInstance(modType);
            }
            else
            {
                Console.WriteLine($"Invalid mod format or mod class not found for '{modScript}'.");
                return null;
            }
        }
    }

    class ExampleMod : IMod
    {
        public void ApplyMod(List<Business> businesses, UI ui, GUI gui)
        {
            // Modify UI elements
            ui.GameTitle = "Custom Tycoon Game";
            ui.MainMenuTitle = "Custom Tycoon Game";
            ui.MainMenuOptions.Add("Custom Option");

            // Modify GUI elements
            gui.ModManagementTitle = "Custom Mod Management";
            gui.ModManagementOptions.Add("Custom Mod Option");

            // Add or modify businesses
            businesses.Add(new Business("Custom Business", 300, 70, 1.3));
        }
    }

    class NewBusinessMod : IMod
    {
        public void ApplyMod(List<Business> businesses, UI ui, GUI gui)
        {
            // Add a new business
            businesses.Add(new Business("New Business", 400, 80, 1.4));
        }
    }

    static class Program
    {
        static void Main()
        {
            TycoonGame game = new TycoonGame();
            game.Run();
        }
    }
}
