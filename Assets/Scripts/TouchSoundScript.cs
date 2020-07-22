using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchSoundScript : MonoBehaviour {


    public AK.Wwise.Event footstepSound = new AK.Wwise.Event();

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("sound check");
            footstepSound.Post(gameObject);
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            footstepSound.Post(this.gameObject);
        }
#endif
    }
}
