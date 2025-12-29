using MySql.Data.MySqlClient;
using AutobazarPV.Models;

namespace AutobazarPV.Repositories;

public class CarRepository : ICarRepository
{
    private readonly string _connectionString;

    public CarRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void PridejAuto(Auto auto, int znackaId)
    {
        using var conn = new MySqlConnection(_connectionString);
        conn.Open();
        string sql = "INSERT INTO auta (znacka_id, model, najezd_km, je_skladem, stav, cena, datum_prijeti) " +
                     "VALUES (@zid, @mod, @naj, 1, @stav, @cena, NOW())";
    
        using var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@zid", znackaId);
        cmd.Parameters.AddWithValue("@mod", auto.Model);
        cmd.Parameters.AddWithValue("@naj", auto.NajezdKm);
        cmd.Parameters.AddWithValue("@stav", auto.Stav.ToString());
        cmd.Parameters.AddWithValue("@cena", auto.Cena);
    
        cmd.ExecuteNonQuery();
    }

    public void ProdejAutaTransakce(int autoId, string zakaznikJmeno)
{
    using var conn = new MySqlConnection(_connectionString);
    conn.Open();
    using var transakce = conn.BeginTransaction();

    try
    {
        string sqlCena = "SELECT cena FROM auta WHERE id = @id AND je_skladem = 1";
        using var cmdCena = new MySqlCommand(sqlCena, conn, transakce);
        cmdCena.Parameters.AddWithValue("@id", autoId);
        
        object result = cmdCena.ExecuteScalar();
        if (result == null) 
        {
            throw new Exception("Auto neexistuje nebo je jiz prodane!");
        }
        decimal aktualniCena = Convert.ToDecimal(result);

        string sqlUpdate = "UPDATE auta SET je_skladem = 0 WHERE id = @id";
        using var cmdUpdate = new MySqlCommand(sqlUpdate, conn, transakce);
        cmdUpdate.Parameters.AddWithValue("@id", autoId);
        cmdUpdate.ExecuteNonQuery();

        string sqlInsert = "INSERT INTO prodeje (auto_id, zakaznik_jmeno, prodejni_cena, datum_prodeje) " +
                           "VALUES (@aid, @zak, @cena, NOW())";
        using var cmdInsert = new MySqlCommand(sqlInsert, conn, transakce);
        cmdInsert.Parameters.AddWithValue("@aid", autoId);
        cmdInsert.Parameters.AddWithValue("@zak", zakaznikJmeno);
        cmdInsert.Parameters.AddWithValue("@cena", aktualniCena);
        cmdInsert.ExecuteNonQuery();

        transakce.Commit();
    }
    catch (Exception ex)
    {
        transakce.Rollback();
        throw new Exception("Transakce selhala: " + ex.Message);
    }
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
}