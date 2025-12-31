using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using AutobazarPV.Repositories;
using AutobazarPV.Models;

namespace AutobazarPV;

class Program
{
    static void Main(string[] args)
    {
        try 
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(Path.Combine("Configuration", "appsettings.json"), optional: false)
                .Build();

            string connString = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connString)) throw new Exception("Chybí connection string v appsettings.json!");

            ICarRepository repository = new CarRepository(connString);
            
            Menu(repository);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"KRITICKÁ CHYBA PŘI STARTU: {ex.Message}");
            Console.WriteLine("Zkontrolujte, zda máte složku Configuration zkopírovanou u .exe souboru.");
            Console.ReadKey();
        }
    }

    static void Menu(ICarRepository repo)
    {
        while (true)
        {
            try {
                Console.Clear();
                Console.WriteLine("=== AUTOBAZAR PV - ADMIN MENU ===");
                Console.WriteLine("1) Seznam aut skladem");
                Console.WriteLine("2) Prodat auto (Transakce)");
                Console.WriteLine("3) Přidat auto (Vložení do 3 tabulek: Znacka, Auto, Vybava)");
                Console.WriteLine("4) Smazat auto (Kaskádové mazání)");
                Console.WriteLine("5) Report (Agregace z více tabulek)");
                Console.WriteLine("6) Import z JSON (Import do 2 tabulek)");
                Console.WriteLine("0) Konec");
                Console.Write("\nVolba: ");
                
                string volba = Console.ReadLine();
                if (volba == "0") break;

                switch (volba)
                {
                    case "1": VypisAuta(repo); break;
                    case "2": ProdejVozu(repo); break;
                    case "3": PridejVozidlo(repo); break;
                    case "4": SmazVozidlo(repo); break;
                    case "5": repo.GenerujSouhrnnyReport(); Console.ReadKey(); break;
                    case "6": ImportDat(repo); break;
                    default: Console.WriteLine("Neplatná volba."); Console.ReadKey(); break;
                }
            } catch (Exception ex) { 
                Console.WriteLine($"\nCHYBA OPERACE: {ex.Message}"); 
                Console.ReadKey();
            }
        }
    }

    static void VypisAuta(ICarRepository repo) {
        Console.WriteLine("\n--- AUTA SKLADEM ---");
        var auta = repo.GetVsechnaSkladem();
        if(auta.Count == 0) Console.WriteLine("Sklad je prázdný.");
        foreach(var a in auta) 
            Console.WriteLine($"ID: {a.Id,-3} | {a.Model,-20} | {a.Cena:N0} Kč");
        Console.ReadKey();
    }

    static void PridejVozidlo(ICarRepository repo) {
        Console.WriteLine("\n--- PŘIDÁNÍ VOZU ---");
        Console.Write("Značka (text, např. Volvo): "); string znacka = Console.ReadLine();
        Console.Write("Model: "); string model = Console.ReadLine();
        
        Console.Write("Cena: "); 
        if (!decimal.TryParse(Console.ReadLine(), out decimal cena)) throw new Exception("Cena musí být číslo!");
        
        Console.Write("Najeto (km): ");
        if (!float.TryParse(Console.ReadLine(), out float najezd)) throw new Exception("Nájezd musí být číslo!");

        Console.WriteLine("Výbava ID (1-Klima, 2-Tempomat...): ");
        if (!int.TryParse(Console.ReadLine(), out int vybavaId)) vybavaId = 1;

        repo.PridejAutoSVybavou(new Auto { Model = model, Cena = cena, Najezd = najezd }, znacka, vybavaId);
        Console.WriteLine("ÚSPĚCH: Záznamy vytvořeny v tabulkách 'znacky', 'auta' a 'auto_vybaveni'.");
        Console.ReadKey();
    }

    static void ProdejVozu(ICarRepository repo) {
        Console.Write("\nID auta k prodeji: ");
        if (int.TryParse(Console.ReadLine(), out int id)) {
            Console.Write("Jméno zákazníka: ");
            string jmeno = Console.ReadLine();
            repo.ProdejAutaTransakce(id, jmeno);
            Console.WriteLine("Prodej úspěšně dokončen.");
        }
        Console.ReadKey();
    }

    static void SmazVozidlo(ICarRepository repo) {
        Console.Write("\nID auta ke smazání: ");
        if (int.TryParse(Console.ReadLine(), out int id)) {
            repo.SmazAuto(id);
            Console.WriteLine("Auto smazáno (včetně historie servisu a výbavy).");
        }
        Console.ReadKey();
    }

    static void ImportDat(ICarRepository repo) {
        string cesta = Path.Combine(AppContext.BaseDirectory, "Configuration", "auta_import.json");
        if (!File.Exists(cesta)) cesta = Path.Combine(AppContext.BaseDirectory, "auta_import.json");
        
        Console.WriteLine($"Hledám soubor v: {cesta}");
        repo.ImportAutZJson(cesta);
        Console.WriteLine("Import dokončen (data rozdělena do tabulek 'znacky' a 'auta').");
        Console.ReadKey();
    }
}