using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameSettings;

public class HUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moveCountDisplay;
    [SerializeField] TextMeshProUGUI minMoveCountDisplay;
    [SerializeField] Button backButton;

    void Awake()
    {
        //set minimum move count display
        int minMoveCount = (int)Mathf.Pow(2, ringCount) - 1;
        minMoveCountDisplay.text = "Minimum moves: " + minMoveCount;
    }

    void Update()
    {
        //update move count display
        moveCountDisplay.text = !gameOver ? "Moves: " + moveCount : "Well done!" + '\n' + "Moves: " + moveCount;
    }

    public void OnSelectBack(int sceneIndex)
    {
        //load Main scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }
}