using System;

namespace AutobazarPV.Models;


public enum StavVozidla 
{ 
    Nove, 
    Ojete, 
    Poskozene 
}

public class Auto
{
    public int Id { get; set; }
    public string Model { get; set; }
    
    public float NajezdKm { get; set; }
    
    public bool JeSkladem { get; set; }
    
    //tohlencto je ten enum
    public StavVozidla Stav { get; set; }
    
    public decimal Cena { get; set; }
    
    public DateTime DatumPrijeti { get; set; }
}