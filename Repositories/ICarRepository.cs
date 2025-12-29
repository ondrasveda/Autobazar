using AutobazarPV.Models;
using System.Collections.Generic;

namespace AutobazarPV.Repositories;

public interface ICarRepository
{
    List<Auto> GetVsechnaSkladem();
    void ProdejAutaTransakce(int autoId, string zakaznikJmeno);
    void PridejAuto(Auto auto, int znackaId);
}