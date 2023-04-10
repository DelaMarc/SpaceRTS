using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Jun_BezierCurve))]
public class Jun_BezierCurveEditor : Editor 
{

	public override void OnInspectorGUI()
	{
        Jun_BezierCurve tar = (Jun_BezierCurve)target;
        GUILayout.BeginVertical();

		tar.isAuto = EditorGUILayout.Toggle("Auto", tar.isAuto);
        tar.isClose = EditorGUILayout.Toggle("Close:",tar.isClose);
        if(GUILayout.Button("AddPoint",EditorStyles.miniButton))
        {
            tar.AddPoint();
        }

        for (int i = 0; i < tar.pointCount; i++)
        {
            Jun_BezierPoint thisPoint = tar.GetPoint(i);
            if(thisPoint == null)
            {
                break;
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Point:" + i, GUILayout.Width(40));
            thisPoint.transform.position = EditorGUILayout.Vector3Field("",thisPoint.transform.position);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

	private void OnSceneGUI()
	{
        Jun_BezierCurve tar = (Jun_BezierCurve)target;
        DrawCurve(tar);
	}

    public static void DrawCurve (Jun_BezierCurve tar)
    {
        for (int i = 0; i < tar.pointCount; i++)
        {
            Jun_BezierPoint thisPoint = tar.GetPoint(i);
            if (thisPoint == null)
            {
                tar.RemovePointAt(i);
                break;
            }

			float handleSize = HandleUtility.GetHandleSize(thisPoint.transform.position) * 0.1f;

            Vector3 pointPos = Handles.FreeMoveHandle(thisPoint.transform.position, Quaternion.identity, handleSize, Vector3.zero, Handles.SphereHandleCap);
            if (thisPoint.transform.position != pointPos)
            {
                thisPoint.transform.position = pointPos;
                tar.IsChange();
            }

			if(!tar.isAuto)
			{            
				if (thisPoint.type != Jun_BezierPoint.PointType.None)
				{
					Vector3 handle01Pos = Handles.FreeMoveHandle(thisPoint.worldHandles01, Quaternion.identity, handleSize, Vector3.zero, Handles.CubeHandleCap);

					if (thisPoint.worldHandles01 != handle01Pos)
					{
						thisPoint.worldHandles01 = handle01Pos;
						if (thisPoint.type == Jun_BezierPoint.PointType.Smooth) thisPoint.worldHandles02 = -(handle01Pos - pointPos) + pointPos;
					}

					Vector3 handle02Pos = Handles.FreeMoveHandle(thisPoint.worldHandles02, Quaternion.identity, handleSize, Vector3.zero, Handles.CubeHandleCap);
					if (thisPoint.worldHandles02 != handle02Pos)
					{
						thisPoint.worldHandles02 = handle02Pos;
						if (thisPoint.type == Jun_BezierPoint.PointType.Smooth) thisPoint.worldHandles01 = -(handle02Pos - pointPos) + pointPos;
					}

					Handles.DrawLine(pointPos, handle01Pos);
					Handles.DrawLine(pointPos, handle02Pos);
				}
			}
			else
			{
				Jun_BezierPoint currentPoint = tar.GetPoint(i);
				Jun_BezierPoint prePoint = tar.GetPrePoint(i);
				Jun_BezierPoint nextPoint = tar.GetNextPoint(i);

				if(prePoint == null || nextPoint == null || currentPoint == null)
				{
					currentPoint.type = Jun_BezierPoint.PointType.None;
				}
				else
				{
					//Vector3 pos01 = prePoint.transform.position - currentPoint.transform.position;
					//Vector3 pos02 = nextPoint.transform.position - currentPoint.transform.position;

					//Vector3 nPos = Vector3.Cross(pos01, pos02);

					//float angle = (180 - Vector3.Angle(pos01, pos02))*0.5f * Mathf.Deg2Rad;
					//Vector3 pos;
					//Vector3 a = prePoint.transform.position - pos;
					//Vector3 b = pos - currentPoint.transform.position;

					//Vector3 curPPos = currentPoint.transform.position;

					//(pos.x - curPPos.x)*(pos.x - curPPos.x)

					//Mathf.Cos(angle) * pos01.magnitude;
				}
			}
        }
    }
}
