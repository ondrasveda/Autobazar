using Microsoft.Extensions.Configuration;
using AutobazarPV.Repositories;
using AutobazarPV.Models;

namespace AutobazarPV;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        try 
        {
            string configPath = Path.Combine(AppContext.BaseDirectory, "Configuration", "appsettings.json");
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException($"Konfigurační soubor nebyl nalezen na cestě: {configPath}");
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(Path.Combine("Configuration", "appsettings.json"), optional: false)
                .Build();

            string connString = config.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connString)) 
                throw new Exception("Connection string v appsettings.json je prázdný!");

            ICarRepository repository = new CarRepository(connString);
            
            Menu(repository);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nKRITICKÁ CHYBA PŘI STARTU APLIKACE:");
            Console.WriteLine($"Zpráva: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("\nZkontrolujte složku Configuration a soubor appsettings.json.");
            Console.ReadKey();
        }
    }

    static void Menu(ICarRepository repo)
    {
        while (true)
        {
            try {
                Console.Clear();
                Console.WriteLine("===============================================");
                Console.WriteLine("                  AUTOBAZAR                    ");
                Console.WriteLine("===============================================");
                Console.WriteLine("1) Zobrazit auta skladem");
                Console.WriteLine("2) Prodat auto (TRANSAKCE)");
                Console.WriteLine("3) Přidat nové auto (INSERT VÍCE TABULEK)");
                Console.WriteLine("4) Upravit auto a značku (UPDATE VÍCE TABULEK)");
                Console.WriteLine("5) Smazat auto (KASKÁDOVÉ MAZÁNÍ)");
                Console.WriteLine("6) Generovat souhrnný report (AGREGACE)");
                Console.WriteLine("7) Importovat data z JSON (IMPORT)");
                Console.WriteLine("8) Výkon prodejců (VIEW 1)");
                Console.WriteLine("9) Historie servisu skladem (VIEW 2)");
                Console.WriteLine("10) Přidat servisní záznam");
                Console.WriteLine("0) Ukončit program");
                Console.WriteLine("-----------------------------------------------");
                Console.Write("Vaše volba: ");
                
                string volba = Console.ReadLine();
                if (volba == "0") break;

                switch (volba)
                {
                    case "1": VypisAuta(repo); break;
                    case "2": ProdejVozu(repo); break;
                    case "3": PridejVozidlo(repo); break;
                    case "4": UpravVozidlo(repo); break;
                    case "5": SmazVozidlo(repo); break;
                    case "6": repo.GenerujSouhrnnyReport(); Console.ReadKey(); break;
                    case "7": ImportDat(repo); break;
                    case "8": repo.VypisVykonZamestnancuView(); Console.ReadKey(); break;
                    case "9": repo.VypisServisniHistoriiView(); Console.ReadKey(); break;
                    case "10": ServisVozu(repo); break;
                    default: Console.WriteLine("Neplatná volba."); Console.ReadKey(); break;
                }
            } catch (Exception ex) { 
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nCHYBA: {ex.Message}"); 
                Console.ResetColor();
                Console.ReadKey();
            }
        }
    }

    static void VypisAuta(ICarRepository repo) {
        Console.WriteLine("\n--- AKTUÁLNÍ NABÍDKA ---");
        var auta = repo.GetVsechnaSkladem();
        if(auta.Count == 0) Console.WriteLine("Sklad je prázdný.");
        foreach(var a in auta) 
            Console.WriteLine($"ID: {a.Id,-3} | {a.Model,-20} | Cena: {a.Cena:N0} Kč");
        Console.ReadKey();
    }

    static void PridejVozidlo(ICarRepository repo) {
        Console.WriteLine("\n--- PŘIDÁNÍ VOZU (VÍCE TABULEK) ---");
        Console.Write("Značka: "); string znacka = Console.ReadLine();
        Console.Write("Model: "); string model = Console.ReadLine();
        Console.Write("Cena: "); 
        if (!decimal.TryParse(Console.ReadLine(), out decimal cena)) throw new Exception("Neplatná cena.");
        Console.Write("Nájezd (km): ");
        if (!float.TryParse(Console.ReadLine(), out float najezd)) throw new Exception("Neplatný nájezd.");
        Console.Write("ID výbavy (1-5): ");
        int.TryParse(Console.ReadLine(), out int vybavaId);

        repo.PridejAutoSVybavou(new Auto { Model = model, Cena = cena, Najezd = najezd }, znacka, vybavaId);
        Console.WriteLine("Úspěšně vloženo do tabulek: znacky, auta, auto_vybaveni.");
        Console.ReadKey();
    }

    static void UpravVozidlo(ICarRepository repo) {
        Console.WriteLine("\n--- ÚPRAVA (UPDATE VÍCE TABULEK) ---");
        Console.Write("ID auta pro úpravu: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) return;
        Console.Write("Nový model: "); string nModel = Console.ReadLine();
        Console.Write("Nový název značky: "); string nZnacka = Console.ReadLine();

        repo.AktualizujAutoIZnacku(id, nModel, nZnacka);
        Console.WriteLine("Data upravena v obou tabulkách.");
        Console.ReadKey();
    }

    static void ProdejVozu(ICarRepository repo) {
        Console.WriteLine("\n--- PRODEJ (TRANSAKCE) ---");
        var prodejci = repo.GetProdejci();
        foreach(var p in prodejci) Console.WriteLine($"{p.Id}) {p.Jmeno} {p.Prijmeni}");
        
        Console.Write("\nID prodejce: ");
        int.TryParse(Console.ReadLine(), out int zamId);
        Console.Write("ID auta: ");
        if (int.TryParse(Console.ReadLine(), out int autoId)) {
            Console.Write("Jméno zákazníka: ");
            string jmeno = Console.ReadLine();
            repo.ProdejAutaTransakce(autoId, jmeno, zamId);
            Console.WriteLine("Transakce proběhla v pořádku.");
        }
        Console.ReadKey();
    }

    static void SmazVozidlo(ICarRepository repo) {
        Console.Write("\nID auta ke smazání: ");
        if (int.TryParse(Console.ReadLine(), out int id)) {
            repo.SmazAuto(id);
            Console.WriteLine("Auto i kaskádové vazby smazány.");
        }
        Console.ReadKey();
    }

    static void ImportDat(ICarRepository repo) {
        Console.WriteLine("\n--- IMPORT Z JSON ---");
        string cesta = Path.Combine(AppContext.BaseDirectory, "Configuration", "auta_import.json");
        repo.ImportAutZJson(cesta);
        Console.WriteLine("Import proběhl úspěšně.");
        Console.ReadKey();
    }

    static void ServisVozu(ICarRepository repo) {
        Console.WriteLine("\n--- NOVÝ SERVISNÍ ZÁZNAM ---");
        Console.Write("ID auta: ");
        if (!int.TryParse(Console.ReadLine(), out int autoId)) return;
        Console.Write("Popis: "); string popis = Console.ReadLine();
        Console.Write("Cena opravy: "); decimal.TryParse(Console.ReadLine(), out decimal cena);

        repo.PridejServis(autoId, popis, cena);
        Console.WriteLine("Servis zaevidován.");
        Console.ReadKey();
    }
}