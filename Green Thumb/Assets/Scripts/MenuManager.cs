using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject canvas;

    void Start()
    {
        int levelNum = 1;
        foreach (Transform button in canvas.transform.Find("Levels")) {
            if (levelNum == 1) {
                button.GetComponent<Button>().onClick.AddListener(() => LoadLevel(1));
            } else if (levelNum == 2) {
                button.GetComponent<Button>().onClick.AddListener(() => LoadLevel(2));
            } else if (levelNum == 3) {
                button.GetComponent<Button>().onClick.AddListener(() => LoadLevel(3));
            } else if (levelNum == 4) {
                button.GetComponent<Button>().onClick.AddListener(() => LoadLevel(4));
            } else if (levelNum == 5) {
                button.GetComponent<Button>().onClick.AddListener(() => LoadLevel(5));
            } else if (levelNum == 6) {
                button.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene("CreativeModeScene"));
            }
            
            levelNum++;
        }
    }

    void LoadLevel(int level) {
        string sceneName = "Level" + level.ToString() + "Scene";
        SceneManager.LoadScene(sceneName);
    }
}