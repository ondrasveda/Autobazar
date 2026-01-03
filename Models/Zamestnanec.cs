namespace AutobazarPV.Models;

public class Zamestnanec
{
    public int Id { get; set; }
    public string Jmeno { get; set; }
    public string Prijmeni { get; set; }
    public string CeleJmeno => $"{Jmeno} {Prijmeni}";
}