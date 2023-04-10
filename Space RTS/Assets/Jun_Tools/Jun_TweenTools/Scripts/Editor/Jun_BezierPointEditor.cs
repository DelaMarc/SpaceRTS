using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Jun_BezierPoint))]
public class Jun_BezierPointEditor : Editor 
{
	public override void OnInspectorGUI()
	{
        base.OnInspectorGUI();
	}

	private void OnSceneGUI()
	{
        Jun_BezierPoint tar = (Jun_BezierPoint)target;
        Jun_BezierCurveEditor.DrawCurve(tar.curve);
        if (tar.transform.parent != tar.curve)
            tar.transform.parent = tar.curve.transform;
    }
}
