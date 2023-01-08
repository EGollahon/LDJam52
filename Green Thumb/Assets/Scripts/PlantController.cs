using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlantController : MonoBehaviour
{
    SpriteRenderer plantRenderer;
    public PlantType plantType;
    public string plantName;
    public int waterNeeded;
    public int fertilizerNeeded;
    public int growthLevel;
    public int profit;

    public void InitializePlant(PlantTypeClass plantTemplate, SoilTypeClass soilTemplate) {
        plantType = plantTemplate.type;
        plantName = plantTemplate.name.ToLower();
        waterNeeded = plantTemplate.baseWaterNeed + soilTemplate.waterEffect;
        fertilizerNeeded = plantTemplate.baseFertilizerNeed + soilTemplate.fertilizerEffect;
        growthLevel = 1;
        profit = plantTemplate.profit;

        if (waterNeeded == 0 && fertilizerNeeded == 0) {
            growthLevel = 3;
        }

        SetPlantSprite();
        UpdateNeedsIndicator();
    }

    void SetPlantSprite() {
        plantRenderer = GetComponent<SpriteRenderer>();
        Sprite[] plantSprites = Resources.LoadAll<Sprite>(plantName);

        if (growthLevel == 1) {
            plantRenderer.sprite = plantSprites[0];
        } else if (growthLevel == 2) {
            plantRenderer.sprite = plantSprites[1];
        } else if (growthLevel == 3) {
            plantRenderer.sprite = plantSprites[2];
        }
    }

    public void CareForPlant(ActionType action) {
        if (action == ActionType.Water && waterNeeded > 0) {
            waterNeeded--;
        } else if (action == ActionType.Fertilize && fertilizerNeeded > 0) {
            fertilizerNeeded--;
        }

        if (growthLevel == 1) {
            if (waterNeeded == 0 && fertilizerNeeded == 0) {
                growthLevel = 3;
            } else {
                growthLevel = 2;
            }
            SetPlantSprite();
        } else if (growthLevel == 2 && waterNeeded == 0 && fertilizerNeeded == 0) {
            growthLevel = 3;
            SetPlantSprite();
        }

        UpdateNeedsIndicator();
    }

    void UpdateNeedsIndicator() {
        Sprite[] indicatorSprites = Resources.LoadAll<Sprite>("plant-needs-frame");
        Sprite[] iconSprites = Resources.LoadAll<Sprite>("icons");

        if (waterNeeded > 0 && fertilizerNeeded > 0) {
            transform.Find("Canvas/Needs").gameObject.GetComponent<Image>().sprite = indicatorSprites[1];
            transform.Find("Canvas/Needs/Slot 1").gameObject.GetComponent<Image>().sprite = iconSprites[1];
            transform.Find("Canvas/Needs/Quantity 1").gameObject.GetComponent<TextMeshProUGUI>().text = fertilizerNeeded.ToString();
            transform.Find("Canvas/Needs/Slot 2").gameObject.GetComponent<Image>().sprite = iconSprites[0];
            transform.Find("Canvas/Needs/Quantity 2").gameObject.GetComponent<TextMeshProUGUI>().text = waterNeeded.ToString();
        } else if (waterNeeded > 0 && fertilizerNeeded <= 0) {
            transform.Find("Canvas/Needs").gameObject.GetComponent<Image>().sprite = indicatorSprites[0];
            transform.Find("Canvas/Needs/Slot 1").gameObject.GetComponent<Image>().sprite = iconSprites[0];
            transform.Find("Canvas/Needs/Quantity 1").gameObject.GetComponent<TextMeshProUGUI>().text = waterNeeded.ToString();
            transform.Find("Canvas/Needs/Slot 2").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("blank-icon");
            transform.Find("Canvas/Needs/Quantity 2").gameObject.GetComponent<TextMeshProUGUI>().text = "";
        } else if (fertilizerNeeded > 0 && waterNeeded <= 0) {
            transform.Find("Canvas/Needs").gameObject.GetComponent<Image>().sprite = indicatorSprites[0];
            transform.Find("Canvas/Needs/Slot 1").gameObject.GetComponent<Image>().sprite = iconSprites[1];
            transform.Find("Canvas/Needs/Quantity 1").gameObject.GetComponent<TextMeshProUGUI>().text = fertilizerNeeded.ToString();
            transform.Find("Canvas/Needs/Slot 2").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("blank-icon");
            transform.Find("Canvas/Needs/Quantity 2").gameObject.GetComponent<TextMeshProUGUI>().text = "";
        } else if (waterNeeded <= 0 && fertilizerNeeded <= 0) {
            transform.Find("Canvas/Needs").gameObject.GetComponent<Image>().sprite = indicatorSprites[0];
            transform.Find("Canvas/Needs/Slot 1").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("coin-icon");
            transform.Find("Canvas/Needs/Quantity 1").gameObject.GetComponent<TextMeshProUGUI>().text = profit.ToString();
            transform.Find("Canvas/Needs/Slot 2").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("blank-icon");
            transform.Find("Canvas/Needs/Quantity 2").gameObject.GetComponent<TextMeshProUGUI>().text = "";
        }
    }
}
