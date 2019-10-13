using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameSettings;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ringCountDisplay;
    [SerializeField] Button plusButton, minusButton;

    void Awake()
    {
        UpdateHUD();
    }

    public void OnRingCountChange(int amount)
    {
        //update ringCount
        ringCount = Mathf.Clamp(ringCount + amount, minRings, maxRings);
        UpdateHUD();
    }

    void UpdateHUD()
    {
        //update ring count display
        ringCountDisplay.text = "Rings: " + ringCount;

        //if ringCount is equal to either minRings or maxRings, disable appropriate button
        minusButton.interactable = ringCount != minRings;
        plusButton.interactable = ringCount != maxRings;
    }

    public void OnSelectPlay(int sceneIndex)
    {
        //reset game variables
        gameOver = false;
        moveCount = 0;

        //load Game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }
}