using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoilTypeClass
{
    public SoilType type;
    public string name;
    public int waterEffect;
    public int fertilizerEffect;
}

[System.Serializable]
public class PlantTypeClass
{
    public PlantType type;
    public string name;
    public int profit;
    public int baseWaterNeed;
    public int baseFertilizerNeed;
    public int quantity;
}