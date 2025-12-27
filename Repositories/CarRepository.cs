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

    public void PridejAuto(Auto auto)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string query = "INSERT INTO auta (znacka_id, model, najezd_km, je_skladem, stav, cena, datum_prijeti) " +
                           "VALUES (@znacka, @model, @najezd, @skladem, @stav, @cena, @datum)";
        
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@znacka", 1); // Pro testování fixní značka
                cmd.Parameters.AddWithValue("@model", auto.Model);
                cmd.Parameters.AddWithValue("@najezd", auto.NajezdKm);
                cmd.Parameters.AddWithValue("@skladem", auto.JeSkladem);
                cmd.Parameters.AddWithValue("@stav", auto.Stav.ToString());
                cmd.Parameters.AddWithValue("@cena", auto.Cena);
                cmd.Parameters.AddWithValue("@datum", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
        }
    }

    public List<Auto> GetVsechnaSkladem()
    {
        return new List<Auto>(); // zatim nic nepise
    }

    public void ProdejAutaTransakce(int autoId, string jmenoZakaznika)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var cmdCena = new MySqlCommand("SELECT cena FROM auta WHERE id = @id", connection, transaction);
                    cmdCena.Parameters.AddWithValue("@id", autoId);
                    decimal cena = (decimal)cmdCena.ExecuteScalar();

                    var cmdProdej = new MySqlCommand(
                        "INSERT INTO prodeje (auto_id, zakaznik_jmeno, prodejni_cena) VALUES (@aId, @jmeno, @cena)", 
                        connection, transaction);
                    cmdProdej.Parameters.AddWithValue("@aId", autoId);
                    cmdProdej.Parameters.AddWithValue("@jmeno", jmenoZakaznika);
                    cmdProdej.Parameters.AddWithValue("@cena", cena);
                    cmdProdej.ExecuteNonQuery();

                    var cmdUpdate = new MySqlCommand("UPDATE auta SET je_skladem = 0 WHERE id = @id", connection, transaction);
                    cmdUpdate.Parameters.AddWithValue("@id", autoId);
                    cmdUpdate.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}