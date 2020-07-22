using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneSwitcher {

	// Use this for loading scenes based on index number - can be changed to be done by name
	public static void SceneLoader(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    
    //Loads a minigame scene additively, on top of the kitchen
    public static void LoadSceneAdd(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }

    // After 3 seconds unload the scene
    public static IEnumerator exitScene()
    {
        yield return new WaitForSeconds(3);
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
    }

    // End the game
    public static void EndGame()
    {
        Debug.Log("Goodbye!");
        Application.Quit();
    }
}
