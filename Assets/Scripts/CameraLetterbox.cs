using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script adds letter or pillar boxing to a scene when the screens 
// aspect ratio does not match the games target resolution.
// It does not scale the UI components on the canvas.
// It works by using a second camera displaying plain black then drawing 
// the game scene in a window on top of that.
public class CameraLetterbox : MonoBehaviour {

    // code from http://gamedesigntheory.blogspot.com/2010/09/controlling-aspect-ratio-in-unity.html

    // Use this for initialization
    void Start()
    {
        // set the desired aspect ratio/resoultion
        float targetaspect = 2220.0f / 1080.0f;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain sceneCam component so we can modify its viewport
        Camera camera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
}
