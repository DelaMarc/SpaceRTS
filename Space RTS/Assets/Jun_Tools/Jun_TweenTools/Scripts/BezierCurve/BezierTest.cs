using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierTest : MonoBehaviour {

    public Jun_BezierCurve curve;
    float t;
    float speed = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
        t += Time.deltaTime*0.1f*speed;
        if (t >= 1)
        {
            speed = -1;
        }

        if(t <= 0)
        {
            speed = 1;
        }
        transform.position = curve.GetPointInCurve(t);
	}
}
