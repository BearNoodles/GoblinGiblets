using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectStars : MonoBehaviour {

    // easy = 1, medium = 2, hard = 3;
    public int level = 1;

    public GameObject levelManager;

    public Sprite emptyStar;
    public Sprite fullStar;

    private GameObject star1;
    private GameObject star2;
    private GameObject star3;
    private GameObject star4;
    private GameObject star5;

    // Use this for initialization
    void Start () {
        star1 = gameObject.transform.Find("TopPanel").Find("Star1").gameObject;
        star2 = gameObject.transform.Find("TopPanel").Find("Star2").gameObject;
        star3 = gameObject.transform.Find("BottomPanel").Find("Star3").gameObject;
        star4 = gameObject.transform.Find("BottomPanel").Find("Star4").gameObject;
        star5 = gameObject.transform.Find("BottomPanel").Find("Star5").gameObject;

        LevelSelect levelSelectScript = levelManager.GetComponent<LevelSelect>();

        int targetScore = 0;
        int highScore = 0;

        if (level == 1)
        {
            targetScore = levelSelectScript.easyScore * levelSelectScript.easyCusts;
            highScore = SaveLoad.getEasyHiScore();
        }
        if (level == 2)
        {
            targetScore = levelSelectScript.normalScore * levelSelectScript.normalCusts;
            highScore = SaveLoad.getMediumHiScore();
        }
        if (level == 3)
        {
            targetScore = levelSelectScript.hardScore * levelSelectScript.hardCusts;
            highScore = SaveLoad.getHardHiScore();
        }


        float starsToFill = ((float)highScore / (float)targetScore) * 5.0f;

        if (starsToFill >= 1.0f)
        {
            star1.GetComponent<Image>().sprite = fullStar;
            starsToFill -= 1.0f;
        }
        if (starsToFill >= 1.0f)
        {
            star2.GetComponent<Image>().sprite = fullStar;
            starsToFill -= 1.0f;
        }
        if (starsToFill >= 1.0f)
        {
            star3.GetComponent<Image>().sprite = fullStar;
            starsToFill -= 1.0f;
        }
        if (starsToFill >= 1.0f)
        {
            star4.GetComponent<Image>().sprite = fullStar;
            starsToFill -= 1.0f;
        }
        if (starsToFill >= 1.0f)
        {
            star5.GetComponent<Image>().sprite = fullStar;
            starsToFill -= 1.0f;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
