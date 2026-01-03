using AutobazarPV.Models;

namespace AutobazarPV.Repositories;

public interface ICarRepository
{
    List<Auto> GetVsechnaSkladem();
    void ProdejAutaTransakce(int autoId, string zakaznikJmeno, int zamestnanecId = 1);
    void PridejAutoSVybavou(Auto auto, string nazevZnacky, int vybavaId);
    void SmazAuto(int id);
    void GenerujSouhrnnyReport();
    void ImportAutZJson(string cesta);
    List<Zamestnanec> GetProdejci();
    void PridejServis(int autoId, string popis, decimal cena);
    void VypisVykonZamestnancuView();
    void AktualizujAutoIZnacku(int autoId, string novyModel, string novyNazevZnacky);
    void VypisServisniHistoriiView();   
}