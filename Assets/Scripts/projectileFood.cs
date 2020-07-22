using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script on the food fired in serving minigame designed to play a sound when hitting or missing table
public class projectileFood : MonoBehaviour {

    // sounds
    public AK.Wwise.Event hitSound = new AK.Wwise.Event();
    public AK.Wwise.Event missSound = new AK.Wwise.Event();

    private bool soundPlayed;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // only play one sound
        if (!soundPlayed)
        {
            // if the table is hit
            if (collision.gameObject.tag == "Table")
            {
                //Debug.Log("hit a table");
                hitSound.Post(gameObject);
                soundPlayed = true;
            }
            // if something other than a table is hit
            if (collision.gameObject.tag != "Table")
            {
                //Debug.Log("hit not a table");
                missSound.Post(gameObject);
                soundPlayed = true;
            }
        }
    }
}
