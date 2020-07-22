using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelManager : MonoBehaviour {
    
    // Public objects
    [Header("Public Objects - Coders Only")]
    public GameObject wheel;
    public GameObject goblin;
    public GameObject blender;
    public GameObject progBar;
    public GameObject gameOver;
    public GameObject progArrow;
    public GameObject blurPanel;
    public GameObject controlPanel;
    public GameObject particlePrefab;

    // Public text objects
    public Text overText;
    public Text timeText;
    public Text speedText;
    public Text scoreText;

    // Private objects
    Animator wheelAnimator;
    Animator goblinAnimator;
    Animator blenderAnimator;

    // Private variables
    [Range (0, 100)]
    int roundScore;
    float arrowInc;
    int targetScore;
    Vector3 arrowPos;
    float blenderRot;
    float score = 0.0f;
    bool ended = false;
    bool started = false;
    bool timerOn = false;
    float rotSpeed = 0.0f;
    Vector3 particlePosition;
    float progBarTotalHeight;
    Quaternion particleRotation;

    [Header("Public Variables - Designer Friendly")]
    // Public variables for designers to tweak
    public float minSpeed = 400;
    public float gameTime = 5.0f;
    public float scoreInc = 10.0f;
    public float speedInc = 100.0f;
    public float maxSpeed = 300.0f;
    public float rotDecrease = 50.0f;
    public float scoreTarget = 400.0f;

    [Header("Audio Objects - John Friendly")]
    // Public audio objects and events
    public AK.Wwise.RTPC blendingScoreRTPC;
    public AK.Wwise.Event blendMinigameEnd;
    public AK.Wwise.Event blendMinigameStart;
    public AK.Wwise.Event blendMinigame3Sec = new AK.Wwise.Event();

    private void Start()
    {
        // Set UI panels to be hidden or visible at the start
        gameOver.SetActive(false);
        blurPanel.SetActive(true);
        controlPanel.SetActive(true);

        // Reset rounded score for audio event
        roundScore = 0;
        blendingScoreRTPC.SetGlobalValue(roundScore);

        // Acquire the progress arrow's position from startup
        arrowPos = progArrow.transform.position;

        // Set target score to a quarter of the total score for each round (1/4 so every minigame adds up to total)
        targetScore = MinigameScores.ScoreTarget / 4;                                         

        // Set the initial position and rotation of the particle emitter to make sure particles shoot towards camera
        particlePosition = new Vector3(blender.transform.position.x - 3, blender.transform.position.y + 3, 1.0f);
        particleRotation = new Quaternion(Quaternion.identity.x + 180, Quaternion.identity.y, Quaternion.identity.z, Quaternion.identity.w);

        // Get the animator for the blender and wheel
        wheelAnimator = wheel.GetComponent<Animator>();
        goblinAnimator = goblin.GetComponent<Animator>();
        blenderAnimator = blender.GetComponent<Animator>();

        // Get the height of the progress bar
        progBarTotalHeight = progBar.GetComponent<Renderer>().bounds.size.y;

        // Play audio
        blendMinigameStart.Post(gameObject);
    }

    private void Update()
    {
        // Start the blender and wheel animation parameters
        blenderAnimator.SetBool("Started", started);
        wheelAnimator.SetBool("Started", started);
        blenderAnimator.SetFloat("Speed", rotSpeed);
        wheelAnimator.SetFloat("Speed", rotSpeed);

        // Wait for user input to start the game
        if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space))
        {
            if (started == false)
            {
                blurPanel.SetActive(true);
                controlPanel.SetActive(true);
                // Run game
                Debug.Log("Game Started");
                started = true;
            }
        }

        if (started == true)
        {
            // Start gameplay
            blurPanel.SetActive(false);
            controlPanel.SetActive(false);
            AltGameplay();
            // Timer
            gameTime -= Time.deltaTime;
        }
    }

    // onClick function defies the coding standard because it is a standard Unity function
    public void onClick()
    {
        if(started == true)
        {
            // Increase the speed of the wheel rotation based on increment value
            rotSpeed += speedInc;
        }
    }

    void AltGameplay()
    {
        // Simulate air resistance on the wheel
        if (rotSpeed > 0)
        {
            rotSpeed -= (rotDecrease * Time.deltaTime);
        }
        rotSpeed = Mathf.Clamp(rotSpeed, 0, maxSpeed);

        // Rotate the blender and running goblin using quaternions
        blenderRot += Time.deltaTime * rotSpeed;

        // Wait for the game to be over
        if (gameTime < 0)
        {
            GameOver();
        }
        else
        {
            // Play the timer tick down effect
            if (gameTime <= 3.0f && timerOn == false)
            {
                blendMinigame3Sec.Post(blender);
                timerOn = true;
            }

            // Move the arrow based on the wheel speed
            arrowInc = (rotSpeed / 40);
            arrowPos.y = Mathf.Clamp(arrowPos.y, -4.32f, 4.5f);
            progArrow.transform.position = new Vector3(arrowPos.x, arrowPos.y + arrowInc, arrowPos.z);
            if (arrowInc > 2.0f && arrowInc < 6.0f)
            {
                score += MinigameScores.DifficultyId;
                score = Mathf.Clamp(score, 0, MinigameScores.ScoreTarget / 4);
                Instantiate(particlePrefab, particlePosition, particleRotation);
                goblinAnimator.speed = 1.0f;
            }
            else if (arrowInc < 2.0f)
            {
                goblinAnimator.speed = 0.5f;
            }
            else if (arrowInc > 6.0f)
            {
                goblinAnimator.speed = 1.5f;
            }
        }

        // Set text strings
        timeText.text = "Time: " + gameTime.ToString("0.0");
        speedText.text = "Speed: " + rotSpeed.ToString("0");

        //Round the score to an int
        Mathf.Round(score);
        roundScore = (int)score;
        scoreText.text = "Score: " + roundScore.ToString();
        blendingScoreRTPC.SetGlobalValue(rotSpeed);
    }

    // Run when the minigame has been completed
    void GameOver()
    {
        // Set game over text dependent on the position of the progress bar arrow - should be 2/3 of the way up i.e. when the arrow is in the green section of the bar
        gameTime = 0;
        blurPanel.SetActive(true);
        gameOver.SetActive(true);
        overText.text = roundScore.ToString();


        if(ended == false)
        {
            // Update the global score for the minigame
            MinigameScores.BlendingScore = roundScore;
            ended = true;
        }

        // Return to the kitchen
        blendMinigameEnd.Post(gameObject);
        StartCoroutine(SceneSwitcher.exitScene());
    }
}
