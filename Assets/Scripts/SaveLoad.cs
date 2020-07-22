using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script enables saving and loading from the phone (or other platforms memory)
// allowing the storing of hi scores between boots of the game
// The reason for making the script is to prevent typos in the strings causing a mistake by putting it all in one place
public static class SaveLoad
{
    // creates the values to use, wont do anything if they already exist
    public static void initialiseValues()
    {
        // if there isnt a value saved then create them and set to default
        if (!PlayerPrefs.HasKey("ServingHiScore") || !PlayerPrefs.HasKey("EasyHiScore") || !PlayerPrefs.HasKey("MediumHiScore") || !PlayerPrefs.HasKey("HardHiScore"))
        {
            //Give the PlayerPrefs some values to send over
            PlayerPrefs.SetInt("ServingHiScore", 0);
            PlayerPrefs.SetInt("EasyHiScore", 0);
            PlayerPrefs.SetInt("MediumHiScore", 0);
            PlayerPrefs.SetInt("HardHiScore", 0);
        }
    }

    // the serving hi score was the test used for the initial use of this script, no longer used
    public static void setServingHiScore(int score)
    {
        initialiseValues();
        if (score > PlayerPrefs.GetInt("ServingHiScore"))
        {
            PlayerPrefs.SetInt("ServingHiScore", score);
        }
    }
    // the serving hi score was the test used for the initial use of this script, no longer used
    public static int getServingHiScore()
    {
        return PlayerPrefs.GetInt("ServingHiScore", 0);
    }

    // scores for easy medium and hard levels
    public static void setEasyHiScore(int score)
    {
        initialiseValues();
        // if score given is higher than the stored one update the saved score
        if (score > PlayerPrefs.GetInt("EasyHiScore"))
        {
            PlayerPrefs.SetInt("EasyHiScore", score);
        }
    }
 
    public static int getEasyHiScore()
    {
        // returns a score if there is one stored, if not then 0 is returned
        return PlayerPrefs.GetInt("EasyHiScore", 0);
    }

    public static void setMediumHiScore(int score)
    {
        initialiseValues();
        if (score > PlayerPrefs.GetInt("MediumHiScore"))
        {
            PlayerPrefs.SetInt("MediumHiScore", score);
        }
    }

    public static int getMediumHiScore()
    {
        return PlayerPrefs.GetInt("MediumHiScore", 0);
    }

    public static void setHardHiScore(int score)
    {
        initialiseValues();
        if (score > PlayerPrefs.GetInt("HardHiScore"))
        {
            PlayerPrefs.SetInt("HardHiScore", score);
        }
    }

    public static int getHardHiScore()
    {
        return PlayerPrefs.GetInt("HardHiScore", 0);
    }

    // saves changes to phone memory, can take a moment so dont call during gameplay
    // unsure if this is actually needed
    public static void saveToMemory()
    {
        PlayerPrefs.Save();
    }
}
