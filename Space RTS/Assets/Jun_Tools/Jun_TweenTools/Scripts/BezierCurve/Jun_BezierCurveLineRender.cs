using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class Jun_BezierCurveLineRender : MonoBehaviour {

    public Jun_BezierCurve bezierCurve;

    [Range(0.01f,2)]
    public float pointDistance = 0.1f;
    LineRenderer lineRender;

    Vector3 pos;

	// Use this for initialization
	void Awake () 
    {
        lineRender = GetComponent<LineRenderer>();

        pos = transform.position;
	}

	private void OnEnable()
	{
        DrawLine();
	}

	void Update ()
    {
        if (transform.position != pos)
        {
            DrawLine();
            pos = transform.position;
        }
	}

    void DrawLine ()
    {
		if (bezierCurve == null)
			return;

        if(lineRender == null)
            lineRender = GetComponent<LineRenderer>();

        if (lineRender == null)
            return;

        int count = (int)(bezierCurve.curveLenght / pointDistance);

		if (count > 0)
		{
			lineRender.positionCount = count + 1;
			for (int i = 0; i < count + 1; i++)
			{
				float v = i * 1.0f / count;
				v = v > 1 ? 1 : v;
                Vector3 pos = bezierCurve.GetPointInCurve(v);
				lineRender.SetPosition(i, pos);
			}
		}
    }

	private void OnValidate()
	{
        DrawLine();
	}
}
