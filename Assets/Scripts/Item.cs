using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    private bool isHeld;

    private int direction = 1;

    private float moveCurrent;
    private float moveMax;
    private float speed;

    GameObject item;

    GameObject shadow;

    MenuScript.IngredientType type;
    public MenuScript.IngredientType Type { get { return type; } set { type = value; } }

    //TODO SET PUBLIC ENUM FROM INGSPAWNER FOR TYPE OF INGREDIENT
    //SO STATIONS CAN DETECT INGREDIENT TYPE

	// Use this for initialization
	void Start ()
    {
        moveMax = 1;
        isHeld = false;
        speed = 0.007f;
        item = transform.GetChild(0).gameObject;
        shadow = transform.GetChild(1).gameObject;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (!isHeld)
        {
            Float();   

        }
	}

    private void Float()
    {
        moveCurrent += Time.deltaTime;
        //if (moveCurrent > moveMax)
        //{
            //moveCurrent = 0;
            //direction *= -1;
        //}
        float moveAmount = speed * Mathf.Sin(moveCurrent * 2);
        item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y + moveAmount, item.transform.position.z);
    }


    public void SetHeld(bool value)
    {
        isHeld = value;

        shadow.SetActive(!value);
    }

    public bool GetHeld()
    {
        return isHeld;
    }

}
