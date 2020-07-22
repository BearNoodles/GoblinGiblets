using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script was designed to display a countdown style clock on scenes with a limited time to play
// It did not make it in to the finished product
public class clock : MonoBehaviour {
    // red clock is a child object with the same clock face sprite but tinted red
    GameObject redClock;

    float fillAmount = 0.0f;
    float fillTime = 3.0f;


	// Use this for initialization
	void Start () {
        redClock = transform.Find("redClock").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        // increment the fill
        fillAmount += Time.deltaTime / fillTime;

        // loop back to the start of the fill (for demo purposes)
        if (fillAmount > 1.0f)
        {
            fillAmount = 0.0f;
        }

        // apply the new fill amount
        redClock.GetComponent<Image>().fillAmount = fillAmount;
	}
}
