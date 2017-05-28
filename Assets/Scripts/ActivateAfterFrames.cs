using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAfterFrames : MonoBehaviour {

    public MonoBehaviour target;
    public int activateAfterFrames = 1;
    private int frames = 0;

	// Use this for initialization
	void Start ()
    {
        target.enabled = false;
        frames = 0;
    }
	
	// Update is called once per frame
	void Update () {
        frames++;
        if(frames >= activateAfterFrames)
        {
            target.enabled = true;
            Destroy(this);
        }
	}
}
