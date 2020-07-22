using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowScript : MonoBehaviour {

    SpriteRenderer glowSprite;
    Color glowColor;
    float time;

    bool isGlowing;

	// Use this for initialization
	void Awake ()
    {
        glowSprite = GetComponent<SpriteRenderer>();
        time = 0;
        glowColor = glowSprite.color;
        glowColor.a = 0;
        glowSprite.color = glowColor;
        isGlowing = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(!isGlowing)
        {
            return;
        }
        time += Time.deltaTime * 3;
        glowColor = glowSprite.color;
        glowColor.a = (Mathf.Sin(time) + 3.0f) / 4.0f;
        glowSprite.color = glowColor;
	}

    public void SetGlowing(bool glow)
    {
        if (glow == false)
        {
            glowColor.a = 0;
            glowSprite.color = glowColor;
        }

        isGlowing = glow;
    }
}
