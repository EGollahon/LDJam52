using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilController : MonoBehaviour
{
    SpriteRenderer soilRenderer;
    public SoilType type;
    public GameObject ownPlant;

    void Start()
    {
        soilRenderer = GetComponent<SpriteRenderer>();
        Sprite[] soilSprites = Resources.LoadAll<Sprite>("soil");

        if (type == SoilType.Regular) {
            soilRenderer.sprite = soilSprites[0];
        } else if (type == SoilType.Sandy) {
            soilRenderer.sprite = soilSprites[1];
        } else if (type == SoilType.Clay) {
            soilRenderer.sprite = soilSprites[2];
        } else if (type == SoilType.Silt) {
            soilRenderer.sprite = soilSprites[3];
        } else if (type == SoilType.Loam) {
            soilRenderer.sprite = soilSprites[4];
        }
    }
}
