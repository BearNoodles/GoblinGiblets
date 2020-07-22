using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script simply bounces the game object it is attatched to up and down
// it is used on the arrow pointing to the target table in the serving minigame
public class ArrowBounce : MonoBehaviour {

    // these can be changed in unity to adjust the appearance of the bounce
    public float amplitude = 1.0f;
    public float frequency = 2.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector3(0.0f, (Mathf.Sin(Time.time * frequency) / 200.0f) * amplitude, 0.0f));
	}
}
