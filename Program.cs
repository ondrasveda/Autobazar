using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using AutobazarPV.Repositories;
using AutobazarPV.Models;

namespace AutobazarPV;

class Program
{
    static void Main(string[] args)
    {
        string baseDir = AppContext.BaseDirectory;
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(baseDir)
            .AddJsonFile(Path.Combine("Configuration", "appsettings.json"), optional: true);

        var configuration = configBuilder.Build();
        string connString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connString))
        {
            Console.WriteLine("CHYBA: Konfigurace nenalezena v Configuration/appsettings.json");
            return;
        }

        ICarRepository repository = new CarRepository(connString);

        bool konec = false;
        while (!konec)
        {
            Console.Clear();
            Console.WriteLine("=== AUTOBAZAR PV ===");
            Console.WriteLine("1) Zobrazit vozy skladem");
            Console.WriteLine("2) Prodat vůz");
            Console.WriteLine("3) Přidat nový vůz");
            Console.WriteLine("0) Konec");
            Console.Write("\nVolba: ");

            switch (Console.ReadLine())
            {
                case "1": VypisAuta(repository); break;
                case "2": ProdejVozu(repository); break;
                case "3": PridejNoveAuto(repository); break;
                case "0": konec = true; break;
            }
        }
    }

    private static void VypisAuta(ICarRepository repository)
    {
        Console.WriteLine("\n--- SKLADEM ---");
        var auta = repository.GetVsechnaSkladem();
        if (auta.Count == 0) Console.WriteLine("Sklad je prázdný.");
        else
        {
            foreach (var a in auta) 
                Console.WriteLine($"ID: {a.Id} | {a.Model} | Cena: {a.Cena:N0} Kč");
        }
        Console.ReadKey();
    }

    private static void ProdejVozu(ICarRepository repository)
    {
        Console.Write("\nID vozu: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            Console.Write("Jméno zákazníka: ");
            string jmeno = Console.ReadLine();
            try {
                repository.ProdejAutaTransakce(id, jmeno);
                Console.WriteLine("Prodej úspěšný!");
            } catch (Exception ex) {
                Console.WriteLine($"Chyba: {ex.Message}");
            }
        }
        Console.ReadKey();
    }

    private static void PridejNoveAuto(ICarRepository repository)
    {
        Console.WriteLine("\n--- NOVÉ AUTO ---");
        Console.Write("Model: "); string m = Console.ReadLine();
        Console.Write("Cena: "); decimal c = decimal.Parse(Console.ReadLine());
        Console.Write("Najeto (km): "); float n = float.Parse(Console.ReadLine());
        Console.Write("ID Značky (1-Skoda, 2-VW): "); int z = int.Parse(Console.ReadLine());

        repository.PridejAuto(new Auto { Model = m, Cena = c, NajezdKm = n }, z);
        Console.WriteLine("Uloženo!");
        Console.ReadKey();
    }
}