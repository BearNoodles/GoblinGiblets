using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script for making random mess sprites around a point
public class messScript : MonoBehaviour {

    // mess prefabs
    public GameObject mess1;
    public GameObject mess2;
    public GameObject mess3;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // call to spawn 3 peices of mess at the given location, repeated the amount of times of count variable
    public void MakeMess(Vector3 pos, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            // create random rotation
            Quaternion randRot = Random.rotation;
            // only use z axis of rotation
            randRot.x = 0;
            randRot.y = 0;
            // spawn mess at random position within 1 unit with rotation
            Instantiate(mess1, new Vector3(Random.Range(-1.0f, 1.0f) + pos.x, Random.Range(-1.0f, 1.0f) + pos.y, 0.0f), randRot, gameObject.transform);
            // new rotation
            randRot = Random.rotation;
            // only use z axis of rotation
            randRot.x = 0;
            randRot.y = 0;
            // spawn mess at random position within 1 unit with rotation
            Instantiate(mess2, new Vector3(Random.Range(-1.0f, 1.0f) + pos.x, Random.Range(-1.0f, 1.0f) + pos.y, 0.0f), randRot, gameObject.transform);
            // new rotation
            randRot = Random.rotation;
            // only use z axis of rotation
            randRot.x = 0;
            randRot.y = 0;
            // spawn mess at random position within 1 unit with rotation
            Instantiate(mess3, new Vector3(Random.Range(-1.0f, 1.0f) + pos.x, Random.Range(-1.0f, 1.0f) + pos.y, 0.0f), randRot, gameObject.transform);
        }
    }
}
