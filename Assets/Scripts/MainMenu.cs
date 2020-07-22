using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    // Audio event
    public AK.Wwise.Event gameStart;

   
    public void StartGame()
    {
        // Start audio object
        gameStart.Post(gameObject);
        // Load level select scene
        SceneSwitcher.SceneLoader("Level Selector");
    }

    public void EndGame()
    {
        // End game
        SceneSwitcher.EndGame();
    }
}
