using AutobazarPV.Models;

namespace AutobazarPV.Repositories;

public interface ICarRepository
{
    void PridejAuto(Auto auto);
    List<Auto> GetVsechnaSkladem();
    void ProdejAutaTransakce(int autoId, string jmenoZakaznika);
}