using System;

namespace AutobazarPV.Models;

public class Auto
{
    public int Id { get; set; }
    public string Model { get; set; }
    public float Najezd { get; set; }
    public bool JeSkladem { get; set; }
    public string Stav { get; set; } //enum v databazi
    public decimal Cena { get; set; }
}