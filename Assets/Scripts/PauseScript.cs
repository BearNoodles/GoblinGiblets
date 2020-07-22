using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour {

    // Game objects and script for pause UI
    GameManager gm;
    GameObject pauseCanvas;
    GameObject pauseKitchen;

    // Toggle pause status
    bool pauseToggle;

    // Public audio objects
    public AK.Wwise.Event quitSound = new AK.Wwise.Event();
    public AK.Wwise.Event levelSelectSound = new AK.Wwise.Event();

    // Use this for initialization
    private void Start()
    {
        // Initialize objects
        pauseCanvas = GameObject.FindGameObjectWithTag("PauseCanvas");
        pauseKitchen = GameObject.FindGameObjectWithTag("PauseButton");

        // Set 
        pauseToggle = true;
        PauseToggle();
    }

    // Quit the game
    public void QuitButton()
    {
        quitSound.Post(gameObject);
        SceneSwitcher.EndGame();
    }

    // Set pause status
    public void PauseToggle()
    {
        // set pause toggle to inverse of itself
        pauseToggle = !pauseToggle;
        // Set UI objects to active
        pauseKitchen.SetActive(!pauseToggle);
        pauseCanvas.SetActive(pauseToggle);
    }

    // Return to level select screen
    public void LevelSelectButton()
    {
        levelSelectSound.Post(gameObject);
        SceneSwitcher.SceneLoader("Level Selector");
    }
}
