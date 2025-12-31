using AutobazarPV.Models;
using System.Collections.Generic;

namespace AutobazarPV.Repositories;

public interface ICarRepository
{
    List<Auto> GetVsechnaSkladem();
    void ProdejAutaTransakce(int autoId, string zakaznikJmeno, int zamestnanecId = 1);
    void PridejAutoSVybavou(Auto auto, string nazevZnacky, int vybavaId);
    void SmazAuto(int id);
    void GenerujSouhrnnyReport();
    void ImportAutZJson(string cesta);
}