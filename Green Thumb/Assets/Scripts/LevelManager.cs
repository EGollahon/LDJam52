using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{
    public int nextLevel;
    public int actionsLeft;
    public int waterActionsLeft;
    public int fertilizerActionsLeft;
    public int quota;
    public int currentMoney;

    public GameObject player;
    public GameObject canvas;
    public GameObject plantPrefab;
    public List<GameObject> placedPlants = new List<GameObject>();
    public List<SoilTypeClass> soilTypes = new List<SoilTypeClass>();
    public List<PlantTypeClass> plantTypes = new List<PlantTypeClass>();

    void Start()
    {
        canvas.transform.Find("Exit").gameObject.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("MenuScene"));
        canvas.transform.Find("Restart").gameObject.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
        canvas.transform.Find("Quota").gameObject.GetComponent<TextMeshProUGUI>().text = quota.ToString() + " needed";
        UpdateMoney();
        UpdateSoil();
        canvas.transform.Find("Actions Left").gameObject.GetComponent<TextMeshProUGUI>().text = actionsLeft.ToString() + " actions left";
        UpdateQuantities();
        CheckButtons();
    }

    public void OnMove() {
        SoilType soilType = player.GetComponent<PlayerController>().currentSoil.GetComponent<SoilController>().type;
        int soilIndex = soilTypes.FindIndex(element => element.type == soilType);
        canvas.transform.Find("Soil Info/Soil Type").gameObject.GetComponent<TextMeshProUGUI>().text = soilTypes[soilIndex].name;
        canvas.transform.Find("Soil Info/Water Effect").gameObject.GetComponent<TextMeshProUGUI>().text = soilTypes[soilIndex].name;
        UpdateSoil();
        SubtractAction(ActionType.Plant);
        CheckButtons();
    }

    void SubtractAction(ActionType action) {
        actionsLeft--;
        canvas.transform.Find("Actions Left").gameObject.GetComponent<TextMeshProUGUI>().text = actionsLeft.ToString() + " actions left";
        if (action == ActionType.Water) {
            waterActionsLeft--;
        } else if (action == ActionType.Fertilize) {
            fertilizerActionsLeft--;
        }
        UpdateQuantities();
        if (actionsLeft <= 0) {
            EndLevel();
        }
    }

    void CheckButtons() {
        HideActionDetail();
        HidePlantDetail();
        player.GetComponent<PlayerController>().CheckAllowedActions();
        foreach (Transform button in canvas.transform.Find("Buttons"))
        {
            if (button.parent == canvas.transform.Find("Buttons")) {
                if (button.name == "Water Button") {
                    if (player.GetComponent<PlayerController>().isWateringAllowed && waterActionsLeft > 0 && actionsLeft > 0) {
                        EnableButton(button.gameObject.GetComponent<Button>(), ActionType.Water, PlantType.None);
                    } else {
                        DisableButton(button.gameObject.GetComponent<Button>());
                    }
                } else if (button.name == "Fertilizer Button") {
                    if (player.GetComponent<PlayerController>().isFertilizingAllowed && fertilizerActionsLeft > 0 && actionsLeft > 0) {
                        EnableButton(button.gameObject.GetComponent<Button>(), ActionType.Fertilize, PlantType.None);
                    } else {
                        DisableButton(button.gameObject.GetComponent<Button>());
                    }
                } else {
                    if (player.GetComponent<PlayerController>().isPlantingAllowed && actionsLeft > 0) {
                        if (button.name == "Lettuce Button" && plantTypes[plantTypes.FindIndex(element => element.type == PlantType.Lettuce)].quantity > 0) {
                            EnableButton(button.gameObject.GetComponent<Button>(), ActionType.Plant, PlantType.Lettuce);
                        } else if (button.name == "Tomato Button" && plantTypes[plantTypes.FindIndex(element => element.type == PlantType.Tomato)].quantity > 0) {
                            EnableButton(button.gameObject.GetComponent<Button>(), ActionType.Plant, PlantType.Tomato);
                        } else if (button.name == "Berry Button" && plantTypes[plantTypes.FindIndex(element => element.type == PlantType.Berry)].quantity > 0) {
                            EnableButton(button.gameObject.GetComponent<Button>(), ActionType.Plant, PlantType.Berry);
                        } else if (button.name == "Pumpkin Button" && plantTypes[plantTypes.FindIndex(element => element.type == PlantType.Pumpkin)].quantity > 0) {
                            EnableButton(button.gameObject.GetComponent<Button>(), ActionType.Plant, PlantType.Pumpkin);
                        } else if (button.name == "Corn Button" && plantTypes[plantTypes.FindIndex(element => element.type == PlantType.Corn)].quantity > 0) {
                            EnableButton(button.gameObject.GetComponent<Button>(), ActionType.Plant, PlantType.Corn);
                        } else {
                            DisableButton(button.gameObject.GetComponent<Button>());
                        }
                    } else {
                        DisableButton(button.gameObject.GetComponent<Button>());
                    }
                }
            }
        }
    }

    void EnableButton(Button button, ActionType action, PlantType plant) {
        button.GetComponent<EventTrigger>().triggers.Clear();
        button.onClick.RemoveAllListeners();

        EventTrigger.Entry hoverEvent = new EventTrigger.Entry();
        hoverEvent.eventID = EventTriggerType.PointerEnter;
        EventTrigger.Entry leaveEvent = new EventTrigger.Entry();
        leaveEvent.eventID = EventTriggerType.PointerExit;

        if (action == ActionType.Water || action == ActionType.Fertilize) {
            hoverEvent.callback.AddListener((eventData) => { ShowActionDetail(action); });
            leaveEvent.callback.AddListener((eventData) => { HideActionDetail(); });
            button.onClick.AddListener(() => PlantAction(action));
        } else if (action == ActionType.Plant) {
            hoverEvent.callback.AddListener((eventData) => { ShowPlantDetail(plant); });
            leaveEvent.callback.AddListener((eventData) => { HidePlantDetail(); });
            button.onClick.AddListener(() => CreatePlant(plant));
        }

        button.GetComponent<EventTrigger>().triggers.Add(hoverEvent);
        button.GetComponent<EventTrigger>().triggers.Add(leaveEvent);

        button.GetComponent<Button>().interactable = true;
    }

    void DisableButton(Button button) {
        button.GetComponent<Button>().interactable = false;
        button.GetComponent<EventTrigger>().triggers.Clear();
        button.onClick.RemoveAllListeners();
    }

    void ShowActionDetail(ActionType action) {
        if (action == ActionType.Water) {
            canvas.transform.Find("Action Detail").gameObject.GetComponent<TextMeshProUGUI>().text = "Water";
        } else if (action == ActionType.Fertilize) {
            canvas.transform.Find("Action Detail").gameObject.GetComponent<TextMeshProUGUI>().text = "Fertilize";
        }
    }

    void HideActionDetail() {
        canvas.transform.Find("Action Detail").gameObject.GetComponent<TextMeshProUGUI>().text = "";
    }

    void ShowPlantDetail(PlantType plantType) {
        PlantTypeClass plantTemplate = plantTypes[plantTypes.FindIndex(element => element.type == plantType)];
        canvas.transform.Find("Plant Detail").gameObject.GetComponent<TextMeshProUGUI>().text = plantTemplate.name;
        canvas.transform.Find("Plant Water Icon").gameObject.SetActive(true);
        canvas.transform.Find("Plant Water Need").gameObject.GetComponent<TextMeshProUGUI>().text = plantTemplate.baseWaterNeed.ToString();
        canvas.transform.Find("Plant Fertilizer Icon").gameObject.SetActive(true);
        canvas.transform.Find("Plant Fertilizer Need").gameObject.GetComponent<TextMeshProUGUI>().text = plantTemplate.baseFertilizerNeed.ToString();
        canvas.transform.Find("Plant Profit Icon").gameObject.SetActive(true);
        canvas.transform.Find("Plant Profit Value").gameObject.GetComponent<TextMeshProUGUI>().text = plantTemplate.profit.ToString();
    }

    void HidePlantDetail() {
        canvas.transform.Find("Plant Detail").gameObject.GetComponent<TextMeshProUGUI>().text = "";
        canvas.transform.Find("Plant Water Icon").gameObject.SetActive(false);
        canvas.transform.Find("Plant Water Need").gameObject.GetComponent<TextMeshProUGUI>().text = "";
        canvas.transform.Find("Plant Fertilizer Icon").gameObject.SetActive(false);
        canvas.transform.Find("Plant Fertilizer Need").gameObject.GetComponent<TextMeshProUGUI>().text = "";
        canvas.transform.Find("Plant Profit Icon").gameObject.SetActive(false);
        canvas.transform.Find("Plant Profit Value").gameObject.GetComponent<TextMeshProUGUI>().text = "";
    }

    void CreatePlant(PlantType plantType) {
        PlantTypeClass plantTemplate = plantTypes[plantTypes.FindIndex(element => element.type == plantType)];
        plantTemplate.quantity--;
        SoilType currentSoil = player.GetComponent<PlayerController>().currentSoil.GetComponent<SoilController>().type;
        SoilTypeClass soilTemplate = soilTypes[soilTypes.FindIndex(element => element.type == currentSoil)];

        GameObject newPlant = Instantiate(plantPrefab, player.transform.position, Quaternion.identity);
        newPlant.GetComponent<PlantController>().InitializePlant(plantTemplate, soilTemplate);
        placedPlants.Add(newPlant);
        
        player.GetComponent<PlayerController>().currentSoil.GetComponent<SoilController>().ownPlant = newPlant;
        SubtractAction(ActionType.Plant);
        CheckButtons();
        UpdateMoney();
        UpdateQuantities();
    }

    void PlantAction(ActionType action) {
        player.GetComponent<PlayerController>().currentSoil.GetComponent<SoilController>().ownPlant.GetComponent<PlantController>().CareForPlant(action);
        SubtractAction(action);
        CheckButtons();
        UpdateMoney();
        UpdateQuantities();
    }

    void UpdateMoney() {
        int newMoneyAmount = 0;
        for (int i = 0; i < placedPlants.Count; i++) {
            PlantController plant = placedPlants[i].GetComponent<PlantController>();
            if (plant.growthLevel == 3) {
                newMoneyAmount += plant.profit;
            }
        }
        
        currentMoney = newMoneyAmount;
        canvas.transform.Find("Current Money").gameObject.GetComponent<TextMeshProUGUI>().text = currentMoney.ToString() + " earned";
        if (currentMoney >= quota) {
            EndLevel();
        }
    }

    void UpdateQuantities() {
        foreach (Transform button in canvas.transform.Find("Buttons")) {
            if (button.parent == canvas.transform.Find("Buttons")) {
                if (button.name == "Water Button") {
                    button.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = waterActionsLeft.ToString();
                } else if (button.name == "Fertilizer Button") {
                    button.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = fertilizerActionsLeft.ToString();
                } else if (button.name == "Lettuce Button") {
                    int plantIndex = plantTypes.FindIndex(element => element.type == PlantType.Lettuce);
                    button.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = plantTypes[plantIndex].quantity.ToString();
                } else if (button.name == "Tomato Button") {
                    int plantIndex = plantTypes.FindIndex(element => element.type == PlantType.Tomato);
                    button.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = plantTypes[plantIndex].quantity.ToString();
                } else if (button.name == "Berry Button") {
                    int plantIndex = plantTypes.FindIndex(element => element.type == PlantType.Berry);
                    button.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = plantTypes[plantIndex].quantity.ToString();
                } else if (button.name == "Pumpkin Button") {
                    int plantIndex = plantTypes.FindIndex(element => element.type == PlantType.Pumpkin);
                    button.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = plantTypes[plantIndex].quantity.ToString();
                } else if (button.name == "Corn Button") {
                    int plantIndex = plantTypes.FindIndex(element => element.type == PlantType.Corn);
                    button.Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = plantTypes[plantIndex].quantity.ToString();
                }
            }
        }
    }

    void UpdateSoil() {
        SoilType soilType = player.GetComponent<PlayerController>().currentSoil.GetComponent<SoilController>().type;
        int soilIndex = soilTypes.FindIndex(element => element.type == soilType);
        canvas.transform.Find("Soil Info/Soil Type").gameObject.GetComponent<TextMeshProUGUI>().text = soilTypes[soilIndex].name;

        if (soilTypes[soilIndex].waterEffect > 0) {
            canvas.transform.Find("Soil Info/Water Effect").gameObject.GetComponent<TextMeshProUGUI>().text = "+" + soilTypes[soilIndex].waterEffect.ToString();
        } else {
            canvas.transform.Find("Soil Info/Water Effect").gameObject.GetComponent<TextMeshProUGUI>().text = soilTypes[soilIndex].waterEffect.ToString();
        }

        if (soilTypes[soilIndex].fertilizerEffect > 0) {
            canvas.transform.Find("Soil Info/Fertilizer Effect").gameObject.GetComponent<TextMeshProUGUI>().text = "+" + soilTypes[soilIndex].fertilizerEffect.ToString();
        } else {
            canvas.transform.Find("Soil Info/Fertilizer Effect").gameObject.GetComponent<TextMeshProUGUI>().text = soilTypes[soilIndex].fertilizerEffect.ToString();
        }
    }

    void EndLevel() {
        if (currentMoney >= quota) {
            canvas.transform.Find("Level Success Popup").gameObject.SetActive(true);
            if (nextLevel == 6) {
                canvas.transform.Find("Level Success Popup/Frame/Next Level Button").gameObject.SetActive(false);
            } else {
                string sceneName = "Level" + nextLevel.ToString() + "Scene";
                canvas.transform.Find("Level Success Popup/Frame/Next Level Button").gameObject.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(sceneName));
            }
            canvas.transform.Find("Level Success Popup/Frame/Main Menu Button").gameObject.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("MenuScene"));
        } else {
            canvas.transform.Find("Level Failure Popup").gameObject.SetActive(true);
            canvas.transform.Find("Level Failure Popup/Frame/Try Again Button").gameObject.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
            canvas.transform.Find("Level Failure Popup/Frame/Main Menu Button").gameObject.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("MenuScene"));
        }
    }
}
