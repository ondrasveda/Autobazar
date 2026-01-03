using MySql.Data.MySqlClient;
using AutobazarPV.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AutobazarPV.Repositories;

public class CarRepository : ICarRepository
{
    private readonly string _connectionString;

    public CarRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Auto> GetVsechnaSkladem()
    {
        var seznam = new List<Auto>();
        using var conn = new MySqlConnection(_connectionString);
        conn.Open();
        string sql = "SELECT id, model, cena FROM auta WHERE je_skladem = 1";
        using var cmd = new MySqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            seznam.Add(new Auto {
                Id = reader.GetInt32("id"),
                Model = reader.GetString("model"),
                Cena = reader.GetDecimal("cena")
            });
        }
        return seznam;
    }

    public void PridejAutoSVybavou(Auto auto, string nazevZnacky, int vybavaId)
    {
        using var conn = new MySqlConnection(_connectionString);
        conn.Open();
        using var trans = conn.BeginTransaction();
        try {
            string sqlZnacka = "INSERT IGNORE INTO znacky (nazev) VALUES (@zn); SELECT id FROM znacky WHERE nazev = @zn;";
            var cmdZ = new MySqlCommand(sqlZnacka, conn, trans);
            cmdZ.Parameters.AddWithValue("@zn", nazevZnacky);
            int znackaId = Convert.ToInt32(cmdZ.ExecuteScalar());

            string sqlAuto = "INSERT INTO auta (znacka_id, model, najezd_km, je_skladem, stav, cena, datum_prijeti) " +
                             "VALUES (@zid, @m, @n, 1, 'Ojete', @c, NOW()); SELECT LAST_INSERT_ID();";
            var cmdA = new MySqlCommand(sqlAuto, conn, trans);
            cmdA.Parameters.AddWithValue("@zid", znackaId);
            cmdA.Parameters.AddWithValue("@m", auto.Model);
            cmdA.Parameters.AddWithValue("@n", auto.Najezd);
            cmdA.Parameters.AddWithValue("@c", auto.Cena);
            int noveId = Convert.ToInt32(cmdA.ExecuteScalar());

            string sqlVyb = "INSERT INTO auto_vybaveni (auto_id, vybaveni_id) VALUES (@aid, @vid)";
            var cmdV = new MySqlCommand(sqlVyb, conn, trans);
            cmdV.Parameters.AddWithValue("@aid", noveId);
            cmdV.Parameters.AddWithValue("@vid", vybavaId);
            cmdV.ExecuteNonQuery();

            trans.Commit();
        } catch { trans.Rollback(); throw; }
    }

    public void ProdejAutaTransakce(int autoId, string zakaznikJmeno, int zamestnanecId = 1)
    {
        using var conn = new MySqlConnection(_connectionString);
        conn.Open();
        using var trans = conn.BeginTransaction();
        try {
            string sqlCena = "SELECT cena FROM auta WHERE id = @id AND je_skladem = 1";
            var cmdC = new MySqlCommand(sqlCena, conn, trans);
            cmdC.Parameters.AddWithValue("@id", autoId);
            object res = cmdC.ExecuteScalar();
            if (res == null) throw new Exception("Auto není skladem nebo neexistuje!");
            decimal cena = Convert.ToDecimal(res);

            new MySqlCommand($"UPDATE auta SET je_skladem = 0 WHERE id = {autoId}", conn, trans).ExecuteNonQuery();

            string sqlIns = "INSERT INTO prodeje (auto_id, zamestnanec_id, zakaznik_jmeno, prodejni_cena) VALUES (@aid, @zid, @zak, @p)";
            var cmdI = new MySqlCommand(sqlIns, conn, trans);
            cmdI.Parameters.AddWithValue("@aid", autoId);
            cmdI.Parameters.AddWithValue("@zid", zamestnanecId);
            cmdI.Parameters.AddWithValue("@zak", zakaznikJmeno);
            cmdI.Parameters.AddWithValue("@p", cena);
            cmdI.ExecuteNonQuery();

            trans.Commit();
            File.AppendAllText("prodeje_log.txt", $"{DateTime.Now}: Prodáno ID {autoId}.\n");
        } catch { trans.Rollback(); throw; }
    }

    public void SmazAuto(int id)
    {
        using var conn = new MySqlConnection(_connectionString);
        conn.Open();
        string sql = "DELETE FROM auta WHERE id = @id";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        if (cmd.ExecuteNonQuery() == 0) throw new Exception("Auto s tímto ID neexistuje.");
    }

    public void GenerujSouhrnnyReport()
    {
        using var conn = new MySqlConnection(_connectionString);
        conn.Open();
        string sql = @"SELECT z.nazev, COUNT(a.id) as Pocet, COALESCE(SUM(p.prodejni_cena),0) as Obrat 
                       FROM znacky z LEFT JOIN auta a ON z.id = a.znacka_id 
                       LEFT JOIN prodeje p ON a.id = p.auto_id GROUP BY z.nazev";
        using var reader = new MySqlCommand(sql, conn).ExecuteReader();
        
        Console.WriteLine("\n--- REPORT DLE ZNAČEK ---");
        while (reader.Read()) 
            Console.WriteLine($"{reader["nazev"],-15} | Aut: {reader["Pocet"],-3} | Obrat: {reader["Obrat"]:N0} Kč");
    }

    public void ImportAutZJson(string cesta)
    {
        if (!File.Exists(cesta)) throw new FileNotFoundException($"Soubor {cesta} nebyl nalezen.");
        
        string jsonContent = File.ReadAllText(cesta);
        var data = JsonSerializer.Deserialize<List<AutoImportModel>>(jsonContent);

        using var conn = new MySqlConnection(_connectionString);
        conn.Open();
        foreach (var item in data)
        {
            string sqlZ = "INSERT IGNORE INTO znacky (nazev) VALUES (@n); SELECT id FROM znacky WHERE nazev = @n;";
            var cmdZ = new MySqlCommand(sqlZ, conn);
            cmdZ.Parameters.AddWithValue("@n", item.NazevZnacky);
            int znackaId = Convert.ToInt32(cmdZ.ExecuteScalar());

            string sqlA = "INSERT INTO auta (znacka_id, model, najezd_km, cena, je_skladem, stav, datum_prijeti) " +
                          "VALUES (@zid, @m, @naj, @c, 1, 'Ojete', NOW())";
            var cmdA = new MySqlCommand(sqlA, conn);
            cmdA.Parameters.AddWithValue("@zid", znackaId);
            cmdA.Parameters.AddWithValue("@m", item.Model);
            cmdA.Parameters.AddWithValue("@naj", item.Najezd);
            cmdA.Parameters.AddWithValue("@c", item.Cena);
            cmdA.ExecuteNonQuery();
        }
    }
    
    public List<Zamestnanec> GetProdejci() {
        var list = new List<Zamestnanec>();
        using var conn = new MySqlConnection(_connectionString);
        conn.Open();
                    using var cmd = new MySqlCommand("SELECT id, jmeno, prijmeni FROM zamestnanci", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read()) {
            list.Add(new Zamestnanec { Id = r.GetInt32(0), Jmeno = r.GetString(1), Prijmeni = r.GetString(2) });
        }
        return list;
    }

    public void VypisVykonZamestnancuView() {
        using var conn = new MySqlConnection(_connectionString);
        conn.Open();
        using var cmd = new MySqlCommand("SELECT * FROM v_vykon_zamestnancu", conn);
        using var r = cmd.ExecuteReader();
        Console.WriteLine("\n--- STATISTIKY PRODEJCŮ (SQL VIEW) ---");
        while (r.Read()) {
            Console.WriteLine($"{r["jmeno"]} {r["prijmeni"]}: {r["pocet_prodeju"]} prodaných aut | Tržba: {r["celkova_trzba"]} Kč");
        }
    }
    
    public void PridejServis(int autoId, string popis, decimal cena) {
        using var conn = new MySqlConnection(_connectionString);
        conn.Open();
        string sql = "INSERT INTO servisni_zaznamy (auto_id, popis_opravy, datum_servisu, cena_opravy) VALUES (@aid, @p, NOW(), @c)";
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@aid", autoId);
        cmd.Parameters.AddWithValue("@p", popis);
        cmd.Parameters.AddWithValue("@c", cena);
        cmd.ExecuteNonQuery();
    }
    
    public void AktualizujAutoIZnacku(int autoId, string novyModel, string novyNazevZnacky)
    {
        using var conn = new MySqlConnection(_connectionString);
        conn.Open();
        using var trans = conn.BeginTransaction();
        try {
            string sqlZ = "UPDATE znacky SET nazev = @n WHERE id = (SELECT znacka_id FROM auta WHERE id = @aid)";
            var cmdZ = new MySqlCommand(sqlZ, conn, trans);
            cmdZ.Parameters.AddWithValue("@n", novyNazevZnacky);
            cmdZ.Parameters.AddWithValue("@aid", autoId);
            cmdZ.ExecuteNonQuery();

            string sqlA = "UPDATE auta SET model = @m WHERE id = @aid";
            var cmdA = new MySqlCommand(sqlA, conn, trans);
            cmdA.Parameters.AddWithValue("@m", novyModel);
            cmdA.Parameters.AddWithValue("@aid", autoId);
            cmdA.ExecuteNonQuery();

            trans.Commit();
        } catch { trans.Rollback(); throw; }
    }
    
    public void VypisServisniHistoriiView()
    {
        using var conn = new MySqlConnection(_connectionString);
        conn.Open();
        string sql = "SELECT * FROM v_servisni_historie_skladem";
        using var cmd = new MySqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();

        Console.WriteLine("\n--- HISTORIE SERVISU (AUTA SKLADEM) ---");
        if (!r.HasRows) Console.WriteLine("Žádné záznamy k zobrazení.");
    
        while (r.Read()) {
            Console.WriteLine($"Vůz: {r["model"],-15} | Oprava: {r["popis_opravy"],-25} | Cena: {r["cena_opravy"]} Kč");
        }
    }
}