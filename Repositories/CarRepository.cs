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
        //pridat logiku
    }

    public List<Auto> GetVsechnaSkladem()
    {
        return new List<Auto>(); // zatim nic nepise
    }

    public void ProdejAutaTransakce(int autoId, string jmenoZakaznika)
    {
        // pridat logiku
    }
}