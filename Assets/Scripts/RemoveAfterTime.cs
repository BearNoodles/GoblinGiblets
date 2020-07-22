using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script placed on an object will destroy it after a second or the set number
// used on the particle effects spawned when touching the screen
public class RemoveAfterTime : MonoBehaviour {
    // time can be set in editor
    public float timeBeforeDestruction = 1.0f;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, timeBeforeDestruction);
    }
	
	// Update is called once per frame
	void Update () {
        
	}
}
