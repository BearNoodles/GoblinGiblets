using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [Header("Public Variables - Designer Friendly")]
    // Target scores for one round of each difficulty
    public int easyScore = 2000;
    public int normalScore = 4000;
    public int hardScore = 6000;

    // Number of rounds for each difficulty
    public int easyCusts = 1, normalCusts = 2, hardCusts = 3;
    
    // Public button objects
    public Button easyButt, normalButt, hardButt;

    [Header("Audio Objects - John Friendly")]
    // Public audio event
    public AK.Wwise.Event buttonPress;

    // Start game at easy difficulty
    public void StartEasyGame()
    {
        buttonPress.Post(gameObject);
        Debug.Log("Easy level");
        MinigameScores.ScoreTarget = easyScore;
        MinigameScores.DishTarget = easyCusts;
        MinigameScores.DifficultyId = 1;
        SceneSwitcher.SceneLoader("KitchenScene");
    }

    // Start game at normal difficulty
    public void StartNormalGame()
    {
        buttonPress.Post(gameObject);
        Debug.Log("Normal level");
        MinigameScores.ScoreTarget = normalScore;
        MinigameScores.DishTarget = normalCusts;
        MinigameScores.DifficultyId = 2;
        SceneSwitcher.SceneLoader("KitchenScene");
    }

    // Start game at hard difficulty
    public void StartHardGame()
    {
        buttonPress.Post(gameObject);
        Debug.Log("Hard level");
        MinigameScores.ScoreTarget = hardScore;
        MinigameScores.DishTarget = hardCusts;
        MinigameScores.DifficultyId = 3;
        SceneSwitcher.SceneLoader("KitchenScene");
    }
}