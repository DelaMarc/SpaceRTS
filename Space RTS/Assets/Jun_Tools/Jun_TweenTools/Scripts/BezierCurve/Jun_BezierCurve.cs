using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jun_BezierCurve : MonoBehaviour 
{

    [HideInInspector][SerializeField] List<Jun_BezierPoint> m_bezierPoints = new List<Jun_BezierPoint>();

    public int pointCount{ get { return m_bezierPoints.Count; }}

    [HideInInspector] [SerializeField] float m_lenght;
    [SerializeField] bool m_isClose = false;
	[SerializeField] bool m_isAuto = false;

    private int sample = 30;
    private bool isChange = true;

    public float curveLenght
    {
        get
        {
            if(isChange)
            {
                ApproximateLenght();
                isChange = false;
            }
            return m_lenght;
        }
    }

    public bool isClose{ get { return m_isClose; }set { m_isClose = value; IsChange(); }}
	public bool isAuto{ get { return m_isAuto; } set { m_isAuto = value; IsChange(); }}

    public void AddPoint ()
    {
        GameObject newObj = new GameObject();
        newObj.transform.parent = transform;
        newObj.name = "Point" + pointCount;
        Jun_BezierPoint newPoint = newObj.AddComponent<Jun_BezierPoint>();

        if (pointCount == 0)
            newPoint.transform.localPosition = Vector3.zero;
        if (pointCount == 1)
        {
            newPoint.transform.localPosition = Vector3.up;
        }

        if(pointCount > 1) 
        {
            Vector3 lastPoint = GetPoint(pointCount - 1).transform.localPosition;
            newPoint.transform.localPosition = lastPoint - GetPoint(pointCount - 2).transform.localPosition + lastPoint;
        }

        m_bezierPoints.Add(newPoint);

        IsChange();
    }

    public void RemovePointAt (int index)
    {
        if (index < pointCount)
            m_bezierPoints.RemoveAt(index);
        IsChange();
    }

    public Jun_BezierPoint GetPoint (int index)
    {
        if (index < pointCount)
            return m_bezierPoints[index];
        return null;
    }

    public Vector3 GetPointInCurve (float timeValue)
    {
		if (isClose)
        {
            if (timeValue > 1)
                timeValue = timeValue - (int)timeValue;

            if (timeValue < 0)
            {
                timeValue = 1 + timeValue - (int)timeValue;
            }
        }

        if (pointCount < 2)
            return transform.position;
        if (timeValue <= 0)
            return GetPoint(0).transform.position;
        if (timeValue >= 1)
            return isClose? GetPoint(0).transform.position: GetPoint(pointCount - 1).transform.position;

        float lenght = curveLenght;
   
        float preLenght = 0;

        int count = pointCount - 1;
        if (isClose)
            count = pointCount;
        for (int i = 0; i < count; i++)
        {
            Jun_BezierPoint thisPoint = GetPoint(i);
            Jun_BezierPoint nextPoint = GetPoint(i + 1);

            if (isClose && i == count - 1)
                nextPoint = GetPoint(0);

            if (thisPoint == null || nextPoint == null)
                break;

            Vector3 prePosition = thisPoint.transform.position;
            float thisLenght = GetCurveLenght(thisPoint, nextPoint);

            float thisV = thisLenght * 1.0f / lenght;

            if (timeValue >= preLenght && timeValue <= preLenght + thisV)
            {
                Vector3 pos = GetPoint(thisPoint, nextPoint, (timeValue - preLenght) / thisV);
                return pos;
            }

            preLenght += thisV;
        }

        return transform.position;
    }

	public Jun_BezierPoint GetPrePoint (int index)
	{
		int i = index - 1;
		if (isClose && i < 0)
		{
			return GetPoint(pointCount - 1);
		}
		if (i >= 0)
			return GetPoint(i);
		return null;
	}

	public Jun_BezierPoint GetNextPoint (int index)
	{
		int i = index + 1;
		if (isClose && i >= pointCount)
			return GetPoint(0);
		if (i < pointCount)
			return GetPoint(i);
		return null;
	}

    Vector3 GetPoint (Jun_BezierPoint point01, Jun_BezierPoint point02,float t)
    {
        if (point01 == null || point02 == null)
            return transform.position;
        
        Vector3 p1 = point01.transform.position;
        Vector3 p2 = point01.worldHandles02;
        Vector3 p3 = point02.worldHandles01;
        Vector3 p4 = point02.transform.position;

        return Jun_BezierCurveTools.GetBezierCurvePoint(p1, p2, p3, p4, t);
    }

    float GetCurveLenght (Jun_BezierPoint point01,Jun_BezierPoint point02)
    {
        if (point01 == null || point02 == null)
            return 0;
        
        Vector3 p1 = point01.transform.position;
        Vector3 p2 = point01.worldHandles02;
        Vector3 p3 = point02.worldHandles01;
        Vector3 p4 = point02.transform.position;

        Vector3 lastPosition = p1;
        float thisLenght = 0;
        for (int j = 0; j <= sample; j++)
        {
            float t = j * 1.0f / sample;
            Vector3 thisPos = Jun_BezierCurveTools.GetBezierCurvePoint(p1, p2, p3, p4, t);
            thisLenght += Vector3.Distance(thisPos, lastPosition);
            lastPosition = thisPos;
        }
        return thisLenght;
    }

    void ApproximateLenght ()
    {
        if (pointCount < 2)
            return;
        
        m_lenght = 0;
        for (int i = 0; i < pointCount - 1; i++)
        {
            Jun_BezierPoint thisPoint = GetPoint(i);
            Jun_BezierPoint nextPoint = GetPoint(i + 1);
            m_lenght += GetCurveLenght(thisPoint, nextPoint);
        }

        if (isClose)
            m_lenght += GetCurveLenght(GetPoint(pointCount - 1),GetPoint(0));
    }

    public void IsChange ()
    {
        isChange = true;
        ApproximateLenght();
    }

	private void OnDrawGizmos()
	{
        int lineCount = 30;

        for (int i = 0; i < lineCount; i++)
        {
            float t = i * 1.0f / lineCount;
            float nextT = (i + 1) * 1.0f / lineCount;

            Vector3 tp1 = GetPointInCurve(t);
            Vector3 tp2 = GetPointInCurve(nextT);

            Gizmos.DrawLine(tp1,tp2);
        }

	}
}
