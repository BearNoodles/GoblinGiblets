using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script is used to fill the stars on the results screen
// It originally filled the stars over a set amount of time, it was later 
// adapted to fill them in time with the count on the results screen
public class FiveStar : MonoBehaviour {
    // the time to fill all the stars
    public float timeToFill = 3.0f;
    // the starts only start filling when this is set to true
    public bool startFill = false;
    
    // the star images that will be filled up
    Image star1;
    Image star2;
    Image star3;
    Image star4;
    Image star5;

    // scores start at 0
    float minScore = 0;
    // max score is what it will take to fill all the stars, it is possible the player will score more than this
    public float maxScore = 500;
    // section is the amount of points required to fill one of the 5 stars
    float section;
    // score was used for testing in unity withour needing to play through a level
    public int score = 450;
    // elapsed time is used when filling based on time
    float elapsedTime = 0.0f;

    // stars change with score instead of time
    public bool increaseWithScore = true;
    // counting score is the current score on the results screen
    public int countingScore = 0;
    // totalScore is the final score the player will have, used when filling over time and when skipping the fill
    public int totalScore;
    // setting to true will skip the fill by count or time and move to max fill
    public bool skipToFinish = false;

    // star sounds
    public AK.Wwise.Event starSound1 = new AK.Wwise.Event();
    public AK.Wwise.Event starSound2 = new AK.Wwise.Event();
    public AK.Wwise.Event starSound3 = new AK.Wwise.Event();
    public AK.Wwise.Event starSound4 = new AK.Wwise.Event();
    public AK.Wwise.Event starSound5 = new AK.Wwise.Event();
    // bools tracking whether the star sound has played, each should only play once
    private bool star1Played = false;
    private bool star2Played = false;
    private bool star3Played = false;
    private bool star4Played = false;
    private bool star5Played = false;




    // Use this for initialization
    void Start () {
        // get the image component of the stars that are child of a child of a child of the object with the script
        star1 = gameObject.transform.Find("Backdrop").Find("EmptyStar1").Find("FullStar").GetComponent<Image>();
        star2 = gameObject.transform.Find("Backdrop").Find("EmptyStar2").Find("FullStar").GetComponent<Image>();
        star3 = gameObject.transform.Find("Backdrop").Find("EmptyStar3").Find("FullStar").GetComponent<Image>();
        star4 = gameObject.transform.Find("Backdrop").Find("EmptyStar4").Find("FullStar").GetComponent<Image>();
        star5 = gameObject.transform.Find("Backdrop").Find("EmptyStar5").Find("FullStar").GetComponent<Image>();
        // define a section size
        section = maxScore / 5.0f;
    }
	
	// Update is called once per frame
	void Update () {
        // if the fill has not started dont do anything
        if (!startFill)
        {
            return;
        }
        // if skip to finish is true instantly fill the stars
        if (skipToFinish)
        {
            countingScore = totalScore;
            FillByScore();
            return;
        }
        // if the stars are being filled by score call the appropriate function
        if (increaseWithScore)
        {
            FillByScore();
        } else // if not by score then it must be by time
        {
            FillOverTime();
        }                     
    }

    // 
    void FillByScore()
    {
        // if the player score or target score are 0 or less
        if(totalScore <= 0 || maxScore <= 0)
        {
            // somethings gone wrong or 0 points scored, no need to do anything more here
            return;
        }

        // calculate current score as a % of the target score
        float progress = (float)countingScore / (float)totalScore; // 0 - 1
        // amount to fill when finished as a %;
        float fullFillAmount = (float)totalScore / maxScore; // 0 - 1
        // amount to fill this frame as a %
        float currentFillAmount = fullFillAmount * progress; // 0 - 1
        // amount to fill of the 500% required for all 5 stars
        currentFillAmount *= 5.0f; // 0 - 5

        // fill stars one after another, reducing the amount taken to fill each one before moving on to the next
        star1.fillAmount = Mathf.Clamp(currentFillAmount, 0.0f, 1.0f);
        currentFillAmount -= 1.0f;

        star2.fillAmount = Mathf.Clamp(currentFillAmount, 0.0f, 1.0f);
        currentFillAmount -= 1.0f;

        star3.fillAmount = Mathf.Clamp(currentFillAmount, 0.0f, 1.0f);
        currentFillAmount -= 1.0f;

        star4.fillAmount = Mathf.Clamp(currentFillAmount, 0.0f, 1.0f);
        currentFillAmount -= 1.0f;

        star5.fillAmount = Mathf.Clamp(currentFillAmount, 0.0f, 1.0f);
        currentFillAmount -= 1.0f;

        // play star sound is called every frame, whether on not a sound actualy plays is handled in there
        PlayStarSound();
    }

    // fill over time is the original method for filling the stars and no longer used in game
    void FillOverTime()
    {
        // increment elapsed time
        elapsedTime += Time.deltaTime;

        // once time is up this function doesnt need to do anything more
        if (elapsedTime > timeToFill)
        {
            return;
        }

        // calculate current score as a % of the target score
        float progress = elapsedTime / timeToFill; // 0 - 1
        // amount to fill when finished as a %; 
        float fullFillAmount =  Mathf.Clamp((float)score, minScore, maxScore) / maxScore; // 0 - 1
        // amount to fill this frame as a %
        float currentFillAmount = fullFillAmount * progress; // 0 - 1
        // amount to fill of the 500% required for all 5 stars
        currentFillAmount *= 5.0f; // 0 - 5

        // fill stars one after another, reducing the amount taken to fill each one before moving on to the next
        star1.fillAmount = Mathf.Clamp(currentFillAmount, 0.0f, 1.0f);
        currentFillAmount -= 1.0f;

        star2.fillAmount = Mathf.Clamp(currentFillAmount, 0.0f, 1.0f);
        currentFillAmount -= 1.0f;

        star3.fillAmount = Mathf.Clamp(currentFillAmount, 0.0f, 1.0f);
        currentFillAmount -= 1.0f;

        star4.fillAmount = Mathf.Clamp(currentFillAmount, 0.0f, 1.0f);
        currentFillAmount -= 1.0f;

        star5.fillAmount = Mathf.Clamp(currentFillAmount, 0.0f, 1.0f);
        currentFillAmount -= 1.0f;

        // play star sound is called every frame, whether on not a sound actualy plays is handled in there
        PlayStarSound();
    }

    // function responsible for playing star sounds, only works with counting fill method in current incarnation
    private void PlayStarSound()
    {
        section = maxScore / 5.0f;

        // if the final star sound hasnt been played yet, and the score is high enough
        if (!star5Played && countingScore >= maxScore)
        {
            // play the sound
            starSound5.Post(gameObject);
            // no more sounds should play once the final star one has
            star5Played = true;
            star4Played = true;
            star3Played = true;
            star2Played = true;
            star1Played = true;
        }
        // if the 4th star sound hasnt been played yet, and the score is high enough
        else if (!star4Played && countingScore >= (section * 4))
        {
            // play the sound
            starSound4.Post(gameObject);
            // this or earlier star sounds should not play again
            star4Played = true;
            star3Played = true;
            star2Played = true;
            star1Played = true;
        }
        // if the 3rd star sound hasnt been played yet, and the score is high enough
        else if (!star3Played && countingScore >= (section * 3))
        {
            // play the sound
            starSound3.Post(gameObject);
            // this or earlier star sounds should not play again
            star3Played = true;
            star2Played = true;
            star1Played = true;
        }
        // if the 2nd star sound hasnt been played yet, and the score is high enough
        else if (!star2Played && countingScore >= (section * 2))
        {
            // play the sound
            starSound2.Post(gameObject);
            // this or earlier star sounds should not play again
            star2Played = true;
            star1Played = true;
        }
        // if the 1st star sound hasnt been played yet, and the score is high enough
        else if (!star1Played && countingScore >= (section * 1))
        {
            // play the sound
            starSound1.Post(gameObject);
            // this should not play again
            star1Played = true;
        }
    }
}
