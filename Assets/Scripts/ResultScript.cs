using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// code to make the score fill over a set time instead of increasing by 1 per frame added by Ben Rogers
public class ResultScript : MonoBehaviour {

    public Text blendCurrentScoreText, ovenCurrentScoreText, chopCurrentScoreText, serveCurrentScoreText, totalScoreText, targetScoreText;
    private int blendScore = 0, ovenScore = 0, chopScore = 0, serveScore = 0, totalScore = 0;
    private bool resultFinished;
    public GameObject winO, loseO;

    private GameObject buttons;
    public GameObject stars;
    private FiveStar starScript;

    // time to fill to target score, may take longer or shorter with more or less score than that
    public float timeToFill = 7.0f;

    private float endTime, endTimer;

    public AK.Wwise.Event buttonPress;

    // Use this for initialization
    void Start ()
    {
        endTimer = 0;
        endTime = 3;
        resultFinished = false;
        winO.SetActive(false);
        loseO.SetActive(false);
        if(MinigameScores.ScoreTarget == 0)
        {
            MinigameScores.ScoreTarget = 500;
        }
        targetScoreText.text = MinigameScores.ScoreTarget.ToString();

        buttons = GameObject.FindGameObjectWithTag("Buttons");
        buttons.SetActive(false);

        starScript = stars.GetComponent<FiveStar>();
        Debug.Log(stars.name);
        starScript.score = MinigameScores.TotalScore;
        starScript.maxScore = MinigameScores.ScoreTarget * MinigameScores.DishTarget;
        starScript.totalScore = MinigameScores.TotalScore;

        // Set the total score for results based on difficulty
        if(MinigameScores.DifficultyId == 1)
        {
            SaveLoad.setEasyHiScore(MinigameScores.TotalScore);
        }
        else if (MinigameScores.DifficultyId == 2)
        {
            SaveLoad.setMediumHiScore(MinigameScores.TotalScore);
        }
        else if (MinigameScores.DifficultyId == 3)
        {
            SaveLoad.setHardHiScore(MinigameScores.TotalScore);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        blendCurrentScoreText.text = blendScore.ToString();
        chopCurrentScoreText.text = chopScore.ToString();
        ovenCurrentScoreText.text = ovenScore.ToString();
        serveCurrentScoreText.text = serveScore.ToString();
        totalScoreText.text = totalScore.ToString();
        
        if(!starScript.startFill)
        {
            starScript.startFill = true;
        }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        if (Input.GetMouseButtonUp(0))
        {
            SkipScoring();
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SkipScoring();
        }
#endif

        // filling over time instead of 1 per frame
        // how many points should be filled in a second
        float pointsPerSec = (MinigameScores.ScoreTarget * MinigameScores.DishTarget) / timeToFill;
        // how many points should be filled this frame
        int pointsToAddThisFrame = (int)(pointsPerSec * Time.deltaTime);
        
        // if the counting isnt finished
        if (totalScore < MinigameScores.TotalScore)
        {
            // add the score for this frame
            totalScore += pointsToAddThisFrame;
            // if the score has gone over its target
            if (totalScore > MinigameScores.TotalScore)
            {
                // cap it
                totalScore = MinigameScores.TotalScore;
            }
            // update the stars
            starScript.countingScore = totalScore;
        }
        // screen does not separate these out anymore
        /*if (blendScore < MinigameScores.BlendingScoreTotal)
        {
            Debug.Log(MinigameScores.BlendingScoreTotal);
            blendScore++;
            totalScore++;
            starScript.countingScore = totalScore;
        }
        else if (chopScore < MinigameScores.ChoppingScoreTotal)
        {
            Debug.Log(MinigameScores.ChoppingScoreTotal);
            chopScore++;
            totalScore++;
            starScript.countingScore = totalScore;
        }
        else if (ovenScore < MinigameScores.CookingScoreTotal)
        {
            Debug.Log(MinigameScores.CookingScoreTotal);
            ovenScore++;
            totalScore++;
            starScript.countingScore = totalScore;
        }
        else if (serveScore < MinigameScores.ServingScoreTotal)
        {
            serveScore++;
            totalScore++;
            starScript.countingScore = totalScore;
        }*/
        else if(!resultFinished)
        {
            ShowResult();
            buttons.SetActive(true);
        }
        else if(endTimer < endTime)
        {
            endTimer += Time.deltaTime;
        }
        else
        {

        }

        
    }

    private void ShowResult()
    {
        resultFinished = true;
        if (totalScore >= MinigameScores.ScoreTarget * MinigameScores.DishTarget)
        {
            winO.SetActive(true);
        }
        else
        {
            //loseO.SetActive(true);
        }
    }

    private void SkipScoring()
    {
        blendScore = MinigameScores.BlendingScoreTotal;
        chopScore = MinigameScores.ChoppingScoreTotal;
        ovenScore = MinigameScores.CookingScoreTotal;
        serveScore = MinigameScores.ServingScoreTotal;

        totalScore = MinigameScores.TotalScore;

        starScript.skipToFinish = true;
    }

    public void ReturnToMenu()
    {
        // Return to the main menu
        buttonPress.Post(gameObject); 
        SceneSwitcher.SceneLoader("Main Menu");
    }
    public void ReturnToLevelSelect()
    {
        // Return to level select
        buttonPress.Post(gameObject);
        SceneSwitcher.SceneLoader("Level Selector");
    }
    public void RetryCurrentLevel()
    {
        // Return to the kitchen
        buttonPress.Post(gameObject);
        SceneSwitcher.SceneLoader("KitchenScene");
    }
}
