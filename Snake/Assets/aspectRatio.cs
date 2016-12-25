using UnityEngine;
using System.Collections;

public class aspectRatio : MonoBehaviour {

	// Use this for initialization
	void Start () {

        // ratio we need
        float aspectRatio = 16.0f / 9.0f;

        // ratio we have
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / aspectRatio;

        // obtaining camera component to modify its viewport
        Camera camera = GetComponent<Camera>();


        Rect rect = camera.rect;
        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        // add pillarbox
        else
        {
            float scalewidth = 1.0f / scaleheight;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
