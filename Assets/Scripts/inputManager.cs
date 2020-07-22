using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour {

    [Header("Public Objects - Coders Only")]
    // Public objects for buttons and a variable to store the current button
    public Button leftButton;
    public Button rightButton;
    
    public GameObject bar;
    public GameObject blade;
    public GameObject arrow;
    public GameObject blurPanel;
    public GameObject ingredient;
    public GameObject controlPanel;
    public GameObject gameOverPanel;
    public GameObject particlePrefab;

    // Public text objects
    public Text timeText;
    public Text scoreText;
    public Text gameOverText;

    [Header("Public Variables - Designer Friendly")]
    // Public variables for design to fine tune
    public int scoreInc;
    public float timeLeft = 5.0f;
    public float arrowInc = 0.0f;

    // Private variables
    int score = 0;
    bool left;
    bool right;
    int winScore;
    float screenWidth;
    float screenHeight;
    float buttonYRange;
    float buttonXRange;
    bool ended = false;
    float arrowStartPos;
    float smooth = 5.0f;
    bool timerOn = false;
    bool started = false;
    float tiltAngle = 60.0f;

    // Private objects
    Animator bladeAnim;
    Button currentButton;

    [Header("Audio Events - John Friendly")]
    // Wwise event object for the slice sound
    public AK.Wwise.Event sliceSound = new AK.Wwise.Event();
    public AK.Wwise.Event slicingGameStart;
    public AK.Wwise.Event slicingGameEnd;
    public AK.Wwise.Event slicingGame3Sec = new AK.Wwise.Event();
    public AK.Wwise.RTPC slicingGameRTPC;

    // Run on scene start
    private void Start()
    {
        // Wait until the user has chosen to start the minigame
        blurPanel.SetActive(true);
        controlPanel.SetActive(true);
        // Hide the game over panel
        gameOverPanel.SetActive(false);

        // Set the left button to be the initial interactable one
        leftButton.interactable = true;
        currentButton = leftButton;
        rightButton.interactable = false;

        // Get the arrow's X position at the start of the game
        arrowStartPos = arrow.transform.position.x;

        // Get the target score for the minigame
        winScore = MinigameScores.ScoreTarget / 4;

        // Get the bounds of the screen
        screenHeight = Screen.height;
        screenWidth = Screen.width;

        // Get the sprite required for the ingredient
        ingredient.GetComponent<SpriteRenderer>().sprite = IngredientTracker.CurrentIngredientSprite;

        // Set the range for random button positions
        buttonYRange = screenHeight / 3;
        buttonXRange = screenWidth / 20;
    }

    // Rotate the blade based on which button is pressed - possibly done with animations in future
    void RotateBlade(Button button)
    {
        // Set the blade rotation position
        if (button == leftButton)
        {
            blade.transform.Rotate(0, 0, -60);
        }
        else if (button == rightButton)
        {
            blade.transform.Rotate(0, 0, 60);
        }
        // Play the slicing sound
        sliceSound.Post(blade);
    }

    // Run on button click
    public void onClick()
    {
        // While there is still time left
        if (timeLeft > 0 && started == true)
        {
            // Depending on the current button, rotate the blade and alternate button - Could be moved to own function to remove duplicate code in future
            if (currentButton == leftButton)
            {
                Instantiate(particlePrefab, Camera.main.ScreenToWorldPoint(currentButton.transform.position), Quaternion.identity);
                ButtonChange(leftButton, rightButton);
            }
            else if (currentButton == rightButton)
            {
                Instantiate(particlePrefab, Camera.main.ScreenToWorldPoint(currentButton.transform.position), Quaternion.identity);
                ButtonChange(rightButton, leftButton);
            }

            // Move arrow for progress bar while the score isn't reached
            if (score <= winScore)
            {
                arrowInc = ((float)score / (float)winScore) * (bar.GetComponent<Renderer>().bounds.size.x);
                Vector3 arrowPos = arrow.transform.position;
                Debug.Log("Arrow inc: " + arrowInc);
                Debug.Log("Arrow Start Pos: " + arrowStartPos);
                arrow.transform.position = new Vector3(arrowInc + arrowStartPos, arrow.transform.position.y, arrow.transform.position.z);
            }
        }
    }

    // Set the current button and handle movement
    void ButtonChange(Button current, Button other)
    {
        SetButtonPos(current);
        RotateBlade(current);
        score += scoreInc * (MinigameScores.DifficultyId * 2);
        score = Mathf.Clamp(score, 0, MinigameScores.ScoreTarget / 4);
        current.interactable = false;
        other.interactable = true;
        currentButton = other;
    }

    // Set the buttons random moving positions
    void SetButtonPos(Button current)
    {
        float rand = Random.Range(-buttonYRange, buttonYRange);
        if (current == leftButton)
        {
            Vector3 randPosL = new Vector3(0 + buttonXRange, (screenHeight / 2) + rand, 0.0f);
            current.GetComponent<RectTransform>().position = randPosL;
        }
        else if (current == rightButton)
        {
            Vector3 randPosR = new Vector3(screenWidth - buttonXRange, (screenHeight / 2) + rand, 0.0f);
            current.GetComponent<RectTransform>().position = randPosR;
        }
    }

    // Runs once every frame
    private void Update()
    {
        // Wait for user input to start the game
        if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space))
        {
            if (started == false)
            {
                blurPanel.SetActive(true);
                controlPanel.SetActive(true);
                // Run game
                started = true;
            }
        }

        if (started == true)
        {
            // Start gameplay
            blurPanel.SetActive(false);
            controlPanel.SetActive(false);
            Gameplay();
            // Decrease the timer
            timeLeft -= Time.deltaTime;
        }
    }
    
    void Gameplay()
    {
        // Minigame End
        if (timeLeft < 0)
        {
            GameOver();
        }

        if(timeLeft <= 3.0f && timerOn == false)
        {
            slicingGame3Sec.Post(blade);
            timerOn = true;
        }

        // Rotate the blade back to the base rotation if it's rotation has changed at all
        float tiltAroundZ = Input.GetAxis("Horizontal") * tiltAngle;
        float tiltAroundX = Input.GetAxis("Vertical") * tiltAngle;

        Quaternion target = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);

        blade.transform.rotation = Quaternion.Slerp(blade.transform.rotation, target, Time.deltaTime * smooth);

        // Output text for score and time
        scoreText.text = "Score: " + score;
        timeText.text = "Time Left: " + timeLeft.ToString("0.0");
    }

    void GameOver()
    {
        // When time runs out, display the game over panel with the correct text
        timeLeft = 0;

        blurPanel.SetActive(true);
        gameOverPanel.SetActive(true);
        gameOverText.text = score.ToString();

        if(ended == false)
        {
            // Set the global score for the slicing game
            MinigameScores.ChoppingScore = score;
            ended = true;
        }

        StartCoroutine(SceneSwitcher.exitScene());
    }
}
