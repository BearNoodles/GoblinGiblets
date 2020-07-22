using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// cooking/fire stokeing/oven minigame
public class TapMinigame : MonoBehaviour {

    
    // score variables
    int scoreOnGreen; 
    int scoreOnYellow;
    int scoreOnOrange;
    int targetScore;
    // score
    private int hits = 0;

    // customisable variables
    [Header("Custom Variables")]
    // custom width for bar
    public float barWidth = 8;

    [Header("Dont touch")]
    // Public objects for the gameover UI
    public GameObject gameOverPanel;
    public Text gameOverText;

    // sprites
    public GameObject rat;
    public Sprite rawRat;
    public Sprite cookedRat;

    public GameObject indicator;

    // objects
    public GameObject pointer;
    public GameObject target;
    public Text scoreText;

    public GameObject breathParticles;
    public GameObject fartParticles;

    public GameObject blurPanel;
    public GameObject tutorialPanel;

    // scripts
    private TapPointer pointerScript;
    private TapTarget targetScript;

    private bool gameFailed = false;
    private bool gameWon = false;

    private bool scoreAdded;
    private bool gameStarted;
        

    // sounds
    public AK.Wwise.Event ovenGameStartSound = new AK.Wwise.Event();
    public AK.Wwise.Event ovenGameStopSound = new AK.Wwise.Event();

    public AK.Wwise.Event fartSound = new AK.Wwise.Event();
    public AK.Wwise.Event hitSound = new AK.Wwise.Event();

    /// The Wwise RTPC that we will connect to.
    public AK.Wwise.RTPC Oven_Minigame;


    // Use this for initialization
    void Start () {
        // get pointer and target script reference
        pointerScript = pointer.GetComponent<TapPointer>();
        pointerScript.travelDistance = barWidth;
        targetScript = target.GetComponent<TapTarget>();

        // Hide the game over panel
        gameOverPanel.SetActive(false);

        // starting score text
        scoreText.text = "Score: 0";

        ovenGameStartSound.Post(gameObject);

        // difficulty based scoring
        if (MinigameScores.ScoreTarget > 0)
        {
            scoreOnGreen = 50 * (MinigameScores.DishTarget * 5);
            scoreOnYellow = 25 * (MinigameScores.DishTarget * 5);
            scoreOnOrange = 10 * (MinigameScores.DishTarget * 5);
            targetScore = MinigameScores.ScoreTarget / 4;
        } else
        {
            scoreOnGreen = 50; 
            scoreOnYellow = 25;
            scoreOnOrange = 10;
            targetScore = 100;
        }
        
        scoreAdded = false;
        gameStarted = false;

        // display tutorial objects
        blurPanel.SetActive(true);
        tutorialPanel.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {

        // hide tutorial panel
        if (Input.GetMouseButtonDown(0) && !gameStarted)
        {
            blurPanel.SetActive(false);
            tutorialPanel.SetActive(false);
            gameStarted = true;
            return;
        }

        // if game finished or game not started
        if (scoreAdded || !gameStarted)
        {
            return;
        }

        // colour indicator, highlights old goblin art when target and pointer overlap to tell you when to tap, not visible in game currently
        if (pointer.GetComponent<BoxCollider2D>().IsTouching(target.GetComponent<BoxCollider2D>()) && targetScript.colourState != 0) // tap when the pointer is on the target
        {
            indicator.GetComponent<SpriteRenderer>().color = Color.green;
        } else
        {
            indicator.GetComponent<SpriteRenderer>().color = Color.white;
        }

        // scale score to 0-100 value for oven volume
        float ovenVolume = (hits / targetScore) * 100.0f;
        // oven sound based on score clamped 0-100
        Oven_Minigame.SetGlobalValue(Mathf.Clamp(ovenVolume, 0, 100)); 

        // if the game is not finished
        if (!gameFailed && !gameWon) 
        {
            // if the player has touched the screen
            if (Input.GetMouseButtonDown(0))
            {
                if (pointer.GetComponent<BoxCollider2D>().IsTouching(target.GetComponent<BoxCollider2D>())) // tap when the pointer is on the target
                {
                    // result of tap depends on current colour of target
                    // 1 and 5 orange, 2 and 4 yellow, 3 green, 0 red
                    if (targetScript.colourState == 1 || targetScript.colourState == 5)
                    {
                        // increase score
                        hits += scoreOnOrange;
                        // if over max score clamp it back
                        if (hits > targetScore) { hits = targetScore; }
                        // update score displayed on UI
                        scoreText.text = "Score: " + hits;
                        // spawn breath prefab
                        Instantiate(breathParticles);
                        // play breath sound
                        hitSound.Post(gameObject);
                    }
                    else if (targetScript.colourState == 2 || targetScript.colourState == 4)
                    {
                        hits += scoreOnYellow;
                        if (hits > targetScore) { hits = targetScore; }
                        scoreText.text = "Score: " + hits;
                        Instantiate(breathParticles);
                        hitSound.Post(gameObject);
                    }
                    else if (targetScript.colourState == 3)
                    {
                        hits += scoreOnGreen;
                        if (hits > targetScore) { hits = targetScore; }
                        scoreText.text = "Score: " + hits;
                        Instantiate(breathParticles);
                        hitSound.Post(gameObject);
                    }
                    else if (targetScript.colourState == 0) // if bar is red then hitting it is a failure
                    {
                        gameFailed = true;
                        Instantiate(fartParticles);
                        fartSound.Post(gameObject);
                    }
                }
                else // if the player missed the target bar
                {
                    gameFailed = true;
                    Instantiate(fartParticles);
                    fartSound.Post(gameObject);
                }
            }
        }

        // if the score is equal to or over the target score
        if (hits >= targetScore)
        {
            // you win
            rat.GetComponent<SpriteRenderer>().sprite = cookedRat; // change rat sprite to cooked version, not visible in final build
            gameWon = true; // set game won
            Victory(); 
        }
        if (gameFailed)
        {
            Failure();
        }
    }

    // on victory do these things
    public void Victory()
    {
        // show game over hud
        gameOverPanel.SetActive(true); 

        // update score for outside the scene
        MinigameScores.CookingScore = targetScore;

        // display score on screen
        gameOverText.text = "" + MinigameScores.CookingScore;

        // once score is added the gameplay will not happen any more in update
        scoreAdded = true;

        // stop oven sound
        ovenGameStopSound.Post(gameObject);

        // leave scene
        StartCoroutine(SceneSwitcher.exitScene());
    }

    // on failure do these things, the victory and failure functions used to do different things, they could be merged or changed
    public void Failure()
    {
        // show game over hud
        gameOverPanel.SetActive(true);

        // update score for outside the scene
        MinigameScores.CookingScore = hits;

        // display score on screen
        gameOverText.text = "" + MinigameScores.CookingScore;

        // once score is added the gameplay will not happen any more in update
        scoreAdded = true;

        // stop oven sound
        ovenGameStopSound.Post(gameObject);

        // leave scene
        StartCoroutine(SceneSwitcher.exitScene());
    }


    // function from earlier version of minigame, currently unused
    // reposition target to a random spot on the bar after a hit.
    public void RepositionTarget()
    {
        // generate random X value, target width hard coded here, fix if necessary
        float targetWidth = 1;
        float xValue = Random.Range(0.0f, pointerScript.travelDistance - targetWidth) - ((pointerScript.travelDistance - targetWidth) / 2);

        target.GetComponent<Transform>().position = new Vector3(xValue, target.GetComponent<Transform>().position.y, 0.0f);
    }
}
