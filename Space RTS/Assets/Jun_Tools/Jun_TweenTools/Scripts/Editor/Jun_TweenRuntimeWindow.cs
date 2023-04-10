using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Jun_TweenRuntimeWindow : EditorWindow
{
	public static void Open(Jun_TweenRuntime tweenRuntime)
	{
		if (tweenRuntime == null)
			return;

		Jun_TweenRuntimeWindow window =
			EditorWindow.GetWindow<Jun_TweenRuntimeWindow>(tweenRuntime.gameObject.name + "(Tween)",true);
		window.Show();
		window.minSize = new Vector2(400, 200);
		window.ChangeEditorObject(tweenRuntime);
	}

	Jun_TweenType addTweenType = Jun_TweenType.PositionTween;
	private Jun_TweenRuntime runtimeObj;
	private SerializedObject sobj;
	private int curTrackIndex;
	private bool isPlay = false;
	private int playDir = 1;
	private float playTime;

	private static Texture2D _tweenPlayIcon;
	private static Texture2D _tweenPuaseIcon;
	private static Texture2D _boxTexture;
	
	Texture2D positionIcon;
	Texture2D rotateIcon;
	Texture2D scaleIcon;
	Texture2D colorIcon;
	Texture2D alphaIcon;
	Texture2D fieldOfViewIcon;
	Texture2D pathIcon;
	Texture2D[] icons;
	
	private static GUISkin _tweenSkin;

	public static Texture2D tweenPlayIcon
	{
		get
		{
			if (_tweenPlayIcon == null)
			{
				_tweenPlayIcon = Resources.Load("TweenPlay", typeof(Texture2D)) as Texture2D;
			}

			return _tweenPlayIcon;
		}
	}

	public static Texture2D tweenPuaseIcon
	{
		get
		{
			if (_tweenPuaseIcon == null)
			{
				_tweenPuaseIcon = Resources.Load("TweenPuase", typeof(Texture2D)) as Texture2D;
			}

			return _tweenPuaseIcon;
		}
	}

	public static Texture2D boxTexture
	{
		get
		{
			if (_boxTexture == null)
			{
				_boxTexture = Resources.Load("BoxTexture",typeof(Texture2D)) as Texture2D;
			}

			return _boxTexture;
		}
	}

	public static GUISkin tweenSkin
	{
		get
		{
			if (_tweenSkin == null)
			{
				_tweenSkin = Resources.Load("TweenSkin",typeof(GUISkin)) as GUISkin;
			}
			
			return _tweenSkin;
		}
	}

	public static GUIStyle trackStyle
	{
		get
		{
			if (tweenSkin != null)
				return tweenSkin.customStyles[0];
			return null;
		}
	}

	void OnSelectionChange()
	{
		if(Selection.activeGameObject == null)
			return;
		
		if (runtimeObj == null || Selection.activeGameObject != runtimeObj.gameObject)
		{
			Jun_TweenRuntime tr = Selection.activeGameObject.GetComponent<Jun_TweenRuntime>();
			if (tr != null)
			{
				ChangeEditorObject(tr);
			}
		}
	}

	private void OnFocus()
	{
		OnSelectionChange();
	}

	void ChangeEditorObject(Jun_TweenRuntime obj)
	{
		runtimeObj = obj;
		sobj = new SerializedObject(obj);
		sobj.Update();
		SerializedProperty tweens = sobj.FindProperty("tweens");
		float animationTime = sobj.FindProperty("animationTime").floatValue;
		for (int i = 0; i < tweens.arraySize; i++)
		{
			SerializedProperty thisTween = tweens.GetArrayElementAtIndex(i);
			SerializedProperty thisTween_MinAnimationTime = thisTween.FindPropertyRelative("_minAnimationTime");
			SerializedProperty thisTween_MaxAnimationTime = thisTween.FindPropertyRelative("_maxAnimationTime");
			SerializedProperty minTime = thisTween.FindPropertyRelative("_minTime");
			SerializedProperty maxTime = thisTween.FindPropertyRelative("_maxTime");
			
			float curMinTime = thisTween_MinAnimationTime.floatValue * animationTime;
			float curMaxTime = thisTween_MaxAnimationTime.floatValue * animationTime;
			if (minTime.floatValue != curMinTime)
			{
				minTime.floatValue = curMinTime;
			}
			if (maxTime.floatValue != curMaxTime)
			{
				maxTime.floatValue = curMaxTime;
			}
		}
		sobj.ApplyModifiedProperties();
		Repaint();
	}

	bool isDelete = false;
	int deleteIndex = 0;
	private void OnGUI()
	{
		if(positionIcon == null)
		    positionIcon = Resources.Load("Position", typeof(Texture2D)) as Texture2D;
		if(rotateIcon == null)
		    rotateIcon = Resources.Load("Rotate", typeof(Texture2D)) as Texture2D;
		if(scaleIcon == null)
		    scaleIcon = Resources.Load("Scale", typeof(Texture2D)) as Texture2D;
		if(colorIcon == null)
		    colorIcon = Resources.Load("Color", typeof(Texture2D)) as Texture2D;
		if(alphaIcon == null)
		    alphaIcon = Resources.Load("Alpha", typeof(Texture2D)) as Texture2D;
		if(fieldOfViewIcon == null)
		    fieldOfViewIcon = Resources.Load("FieldOfView", typeof(Texture2D)) as Texture2D;
		if(pathIcon == null)
		    pathIcon = Resources.Load("Path", typeof(Texture2D)) as Texture2D;
		icons = new Texture2D[7] { positionIcon, rotateIcon, scaleIcon, colorIcon, alphaIcon, fieldOfViewIcon, pathIcon };
		
		if (runtimeObj != null && sobj == null)
		{
			sobj = new SerializedObject(runtimeObj);
		}
		if (sobj != null && runtimeObj != null)
		{
			EventController();
			sobj.Update();

			SerializedProperty m_onFinish = sobj.FindProperty("m_onFinish");

			SerializedProperty tweens = sobj.FindProperty("tweens");

			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			TrackMenu();
			TrackSetting(tweens);
			GUILayout.EndVertical();

			curTrackIndex = curTrackIndex >= tweens.arraySize ? (tweens.arraySize - 1) : curTrackIndex;
			curTrackIndex = curTrackIndex < 0 ? 0 : curTrackIndex;
			
			GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(240), GUILayout.Height(position.height - 5));
			if (clickTimelineEventIndex >= 0)
			{
				DrawTimelineEventSetting();
			}
			else
			{
				if (curTrackIndex >= 0 && curTrackIndex < tweens.arraySize)
				{
					TweenSetting(tweens.GetArrayElementAtIndex(curTrackIndex));
				}
				else
				{
					TweenSetting(null);
				}
			}
			GUILayout.EndVertical();

			GUILayout.EndHorizontal();
			if (isDelete)
			{
				isDelete = false;
				// Debug.Log("Delete track index:" + deleteIndex);
				tweens.DeleteArrayElementAtIndex(deleteIndex);
			}
			
			sobj.ApplyModifiedProperties();
		}
	}

	Event inputEvent;
	float saveTime;

	private Vector3 startVertex;
	private Vector2 mouseDownPosition, mousePosition,mouseRightButtonDownPos;
	private bool isMouseButtonDown,isMouseButtonDrag;
	void EventController()
	{
		#region Keyboard event
		inputEvent = Event.current;

		if (inputEvent != null)
		{
			if (inputEvent.isMouse)
			{
				mousePosition = inputEvent.mousePosition;

				if (inputEvent.button == 0)
				{
					if (inputEvent.type == EventType.MouseDown)
					{
						isMouseButtonDown = true;
						mouseDownPosition = inputEvent.mousePosition;
						isDragTrack = false;
						isClickTrack = false;
					}

					if (inputEvent.type == EventType.MouseDrag)
					{
						isMouseButtonDrag = true;
					}

					if (inputEvent.type == EventType.MouseUp)
					{
						isMouseButtonDrag = false;
						isMouseButtonDown = false;
					}
				}

				if (inputEvent.button == 1)
				{
					if (inputEvent.type == EventType.MouseDown)
					{
						mouseRightButtonDownPos = inputEvent.mousePosition;
					}
				}
			}
		}
		#endregion
	}

	void TrackMenu()
	{
		SerializedProperty animationTime = sobj.FindProperty("animationTime");
		SerializedProperty enablePlay = sobj.FindProperty("enablePlay");
		SerializedProperty awakePlay = sobj.FindProperty("awakePlay");
		SerializedProperty lgnoreTimeScale = sobj.FindProperty("lgnoreTimeScale");
		SerializedProperty randomStartValue = sobj.FindProperty("randomStartValue");
		SerializedProperty startValue = sobj.FindProperty("startValue");

		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		float textLenght = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 80;
		GUILayout.BeginVertical();
		GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(enablePlay);
		EditorGUILayout.PropertyField(awakePlay);
		EditorGUIUtility.labelWidth = 130;
		EditorGUILayout.PropertyField(lgnoreTimeScale);
		EditorGUIUtility.labelWidth = 80;
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(randomStartValue,new GUIContent("RandomStart"),GUILayout.Width(100));
		EditorGUILayout.PropertyField(startValue);
		GUILayout.EndHorizontal();

		EditorGUIUtility.labelWidth = textLenght;
		GUILayout.EndVertical();
	}

	private bool isDragTimeline = false;
	private float trackStartX = 0, trackStartY = 0,dragStartTrackX = 0, dragStartTrackY = 0;
	private float trackScale = 1;
	private float trackMouseDownX = 0 , trackMouseDownWidht = 0;
	private float timelineStartTime = 0;
	private float previewTime = 0;
	private bool isClickTrack = false, isDragTrack = false;
	private int dragTrackType = 0;//0 drag track  1 move minValue  2 move maxValue
	private bool isClickFrameHandle = false, isDragFrameHandle = false;

	void Update()
	{
		if (isPlay)
		{
			float t = (Time.realtimeSinceStartup - playTime) / runtimeObj.animationTime;
			t = t > 1 ? 1 : t;
			
			float curV = (playDir == 1) ? t : (1 - t);
			runtimeObj.previewValue = curV;
			
			if(t >= 1)
			{
				playTime = Time.realtimeSinceStartup;
				switch (runtimeObj.playType)
				{
					case Jun_TweenRuntime.PlayType.Deful:
					case Jun_TweenRuntime.PlayType.Once:
						isPlay = false;
						t = 0;
						playDir = 1;
						break;
					
					case Jun_TweenRuntime.PlayType.Loop:
						t = 0;
						playDir = 1;
						break;
					
					case Jun_TweenRuntime.PlayType.PingPong:
						playDir = playDir == 1 ? -1 : 1;
						break;
				}
			}
			Repaint();
		}
	}

	private float minTrackScale = 0.1f;
	private float maxTrackScale = 3f;
	float timelineMaxTime = 0;
	float timeRangeLeft = 0;
	float timeRangeRight = 0;
	float timelineLenght = 0;
	void TrackSetting(SerializedProperty tweens)
	{
		SerializedProperty playType = sobj.FindProperty("playType");
		SerializedProperty animationTime = sobj.FindProperty("animationTime");
		SerializedProperty animationStartTime = sobj.FindProperty("animationStartTime");

		float height = position.height - 97;
		GUILayout.BeginVertical(EditorStyles.helpBox,GUILayout.Height(height));
		
		GUILayout.BeginHorizontal();
		GUI.color = Color.green;
		Texture2D icon = tweenPlayIcon;
		if (isPlay)
		{
			icon = tweenPuaseIcon;
			GUI.color = Color.yellow;
		}
		if (GUILayout.Button(icon,GUILayout.Height(40), GUILayout.Width(40)))
		{
			isPlay = !isPlay;
			if (isPlay)
			{
				runtimeObj.previewValue = 0;
				playDir = 1;
			}
			playTime = Time.realtimeSinceStartup;
		}

		GUI.color = Color.white;
		
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		GUILayout.Label("Track editor");
		GUILayout.Label("-",GUILayout.Width(10));
		trackScale = GUILayout.HorizontalSlider(trackScale, minTrackScale, maxTrackScale);
		GUILayout.Label("+",GUILayout.Width(20));
		GUILayout.EndHorizontal();
		
		GUILayout.Box("",EditorStyles.helpBox,GUILayout.Height(1));

		GUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(playType,new GUIContent(""),GUILayout.Width(100));
		GUILayout.Label("");
		addTweenType = (Jun_TweenType)EditorGUILayout.EnumPopup (addTweenType,GUILayout.Width(130));

		if(GUILayout.Button ("Add Tween",EditorStyles.miniButton,GUILayout.Width(80)))
		{
			AddTween (tweens);
			Repaint();
		}
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		
		GUILayout.Space(2);
		GUILayout.Box("",EditorStyles.helpBox,GUILayout.Height(2));

		float trackLenght = position.width - 260;
		Rect rect = GUILayoutUtility.GetRect(trackLenght,position.height - 99);

		GUI.BeginScrollView(rect, Vector2.zero, rect);
		float trackX = rect.x + trackStartX;
		float trackMaxX = rect.width * trackScale - rect.width;
		float trackY = rect.y + trackStartY;
		float trackH = 30;
		//DrawTimeline
		Rect timelineRect = new Rect(trackX, trackY, rect.width, rect.height);
		DrawTimeline(timelineRect,trackLenght,ref timelineMaxTime,ref timeRangeLeft,ref timeRangeRight,ref timelineLenght);
		
		trackY += 40;
		GUI.Box(new Rect(trackX,trackY,timelineLenght,2),"",EditorStyles.helpBox);
		trackY += 5;

		#region DragTimeline
		if (rect.Contains(mousePosition))
		{
			if (inputEvent.type == EventType.MouseDown && inputEvent.button == 1)
			{
				dragStartTrackX = trackStartX;
				dragStartTrackY = trackStartY;
				isDragTimeline = true;
			}

			if (inputEvent.isScrollWheel)
			{
				if (inputEvent.delta.y > 0)
				{
					trackScale += 0.1f;
					trackScale = trackScale > maxTrackScale ? maxTrackScale : trackScale;
				}
				else
				{
					trackScale -= 0.1f;
					trackScale = trackScale < minTrackScale ? minTrackScale : trackScale;
				}
				Repaint();
			}
		}
		if (isDragTimeline && inputEvent.type == EventType.MouseDrag && inputEvent.button == 1)
		{
			float moveX = mousePosition.x - mouseRightButtonDownPos.x;
			float moveY = mousePosition.y - mouseRightButtonDownPos.y;
			float curTrackX = dragStartTrackX + moveX;
			float curTrackY = dragStartTrackY + moveY;
			float minTrackX = trackLenght - timelineLenght;
			float minTrackY = rect.height - (tweens.arraySize + 1) * (trackH + 6) - 5;
			curTrackX = curTrackX < minTrackX ? minTrackX : curTrackX;
			curTrackY = curTrackY < minTrackY ? minTrackY : curTrackY;
			curTrackX = curTrackX > 0 ? 0 : curTrackX;
			curTrackY = curTrackY > 0 ? 0 : curTrackY;
			trackStartX = curTrackX;
			trackStartY = curTrackY;
			Repaint();
		}
		if (inputEvent.type == EventType.MouseUp)
			isDragTimeline = false;
		#endregion

		float trackMaxTime = 0;
		for (int i = 0; i < tweens.arraySize; i++)
		{
			SerializedProperty thisTween = tweens.GetArrayElementAtIndex(i);

			SerializedProperty tweenName = thisTween.FindPropertyRelative("_tweenName");
			SerializedProperty tweenType = thisTween.FindPropertyRelative("_tweenType");
			SerializedProperty thisTween_MinAnimationTime = thisTween.FindPropertyRelative("_minAnimationTime");
			SerializedProperty thisTween_MaxAnimationTime = thisTween.FindPropertyRelative("_maxAnimationTime");
			SerializedProperty minTime = thisTween.FindPropertyRelative("_minTime");
			SerializedProperty maxTime = thisTween.FindPropertyRelative("_maxTime");

			thisTween_MinAnimationTime.floatValue = minTime.floatValue / animationTime.floatValue;
			thisTween_MaxAnimationTime.floatValue = maxTime.floatValue / animationTime.floatValue;

			if (maxTime.floatValue >= trackMaxTime)
				trackMaxTime = maxTime.floatValue;

			float minValue = thisTween_MinAnimationTime.floatValue;
			float maxValue = thisTween_MaxAnimationTime.floatValue;

			float thisLenght = (maxTime.floatValue - minTime.floatValue) * trackLenght / timelineMaxTime;
			float thisX = (minTime.floatValue * trackLenght / timelineMaxTime) + trackX;
			
			Rect trackRect = new Rect(thisX,trackY,thisLenght,trackH);
			Rect leftScaleRect = new Rect(trackRect.x ,trackRect.y,8,trackRect.height);
			Rect rightScaleRect = new Rect(trackRect.x + trackRect.width - 8, trackRect.y, 8, trackRect.height);
			Rect dragTrackRect = new Rect(trackRect.x + 8, trackRect.y, trackRect.width - 16, trackRect.height);
			
			if (trackRect.Contains(mousePosition) && rect.Contains(mousePosition))
			{
				switch (inputEvent.button)
				{
					case 0:
						if (inputEvent.type == EventType.MouseDown)
						{
							clickTimelineEventIndex = -1;
							curTrackIndex = i;
							trackMouseDownX = trackRect.x;
							trackMouseDownWidht = trackRect.width;
							timelineStartTime = animationStartTime.floatValue;
							previewTime = runtimeObj.previewValue * animationTime.floatValue;
							isClickTrack = true;
							if (dragTrackRect.Contains(mousePosition))
							{
								dragTrackType = 0;
							}
							if (leftScaleRect.Contains(mousePosition))
							{
								dragTrackType = 1;
							}
							if (rightScaleRect.Contains(mousePosition))
							{
								dragTrackType = 2;
							}
						}
						break;
					
					case 1:
						if(inputEvent.type == EventType.MouseDown)
						{
							
						}
						break;
				}
			}
			
			if (inputEvent.type == EventType.MouseDrag && i == curTrackIndex && isClickTrack && inputEvent.button == 0)
			{
				float moveV = mousePosition.x - mouseDownPosition.x;
				trackRect.x = trackMouseDownX + moveV;
				trackRect.x = trackRect.x < trackX ? trackX : trackRect.x;

				float minInsV = 0.02f;
				switch (dragTrackType)
				{
					case 0:
						minTime.floatValue = (trackRect.x - trackX) * timelineMaxTime / trackLenght;
						maxTime.floatValue = (trackRect.x + trackRect.width - trackX) * timelineMaxTime / trackLenght;
						break;
					
					case 1:
						float minTimeV = (trackRect.x - trackX) * timelineMaxTime / trackLenght;
						if (minTimeV + minInsV < maxTime.floatValue)
						{
							minTime.floatValue = minTimeV;
						}
						else
						{
							minTime.floatValue = maxTime.floatValue - minInsV;
						}
						break;
					
					case 2:
						float maxTimeV = (trackMouseDownX + trackMouseDownWidht + moveV - trackX) * timelineMaxTime / trackLenght;
						if (maxTimeV - minInsV > minTime.floatValue)
						{
							maxTime.floatValue = maxTimeV;
						}
						else
						{
							maxTime.floatValue = minTime.floatValue + minInsV;
						}
						break;
				}
				isDragTrack = true;
			}

			string tweenNameStr = tweenName.stringValue;
			if (string.IsNullOrEmpty(tweenNameStr))
				tweenNameStr = tweenType.enumDisplayNames[tweenType.enumValueIndex];

			if (i == curTrackIndex)
			{
				GUI.color = new Color(0.5f,1f,1f,1);
			}
			else
			{
				GUI.color = Color.white;
			}
			
			DrawTrack(trackRect,i,tweenNameStr, ref minValue,ref maxValue,1,1);
			GUI.color = Color.white;
			GUI.TextArea(leftScaleRect,"");
			GUI.TextArea(rightScaleRect,"");
			trackY += 2 + trackH;
			
			GUI.Box(new Rect(trackX,trackY,timelineLenght,2),"");
			trackY += 4;
			
			thisTween_MinAnimationTime.floatValue = minValue;
			thisTween_MaxAnimationTime.floatValue = maxValue;
		}
		
		if (isDragTrack)
		{
			animationTime.floatValue = trackMaxTime;
			if (timelineStartTime > trackMaxTime)
			{
				timelineStartTime = trackMaxTime;
			}
			animationStartTime.floatValue = timelineStartTime;
			float v = previewTime / trackMaxTime;
			runtimeObj.previewValue = v;
		}
		
		//Draw frame handle
		float fHandleX = timeRangeLeft + (timeRangeRight - timeRangeLeft) * runtimeObj.previewValue;
		GUI.color = new Color(0, 1, 1, 1);
		GUI.DrawTexture(new Rect(fHandleX - 1,rect.y + 20 + trackStartY,2,rect.height - 20),boxTexture);

		Rect frameHandleRect = new Rect(fHandleX - 10, rect.y + trackStartY, 20, 20);
		if (frameHandleRect.Contains(mousePosition))
		{
			switch (inputEvent.button)
			{
				case 0:
					if (inputEvent.type == EventType.MouseDown)
					{
						isDragFrameHandle = true;
					}
					break;
			}
		}
		if (isDragFrameHandle && inputEvent.type == EventType.MouseDrag)
		{
			float moveV = inputEvent.delta.x;
			float curPreviewV = (mousePosition.x - timeRangeLeft) / (timeRangeRight - timeRangeLeft);
			curPreviewV = curPreviewV < 0 ? 0 : curPreviewV;
			runtimeObj.previewValue = curPreviewV;
			Repaint();
		}
		GUI.DrawTexture(frameHandleRect, boxTexture);
		GUI.color = Color.white;
		
		GUI.EndScrollView();
		GUILayout.EndVertical();

		if (inputEvent.type == EventType.MouseUp)
		{
			isClickTrack = false;
			isDragTrack = false;
			isDragFrameHandle = false;
		}
	}

	private bool isClickTimeline = false;
	private int clickTimelineEventIndex = -1;
	private bool clickTimelineEvent = false;
	private float clickTimelineEventTime = 0;
	
	void DrawTimeline(Rect rect,float trackLenght,ref float trackMaxTime,ref float timeRangeLeft,ref float timeRangeRight,ref float timelineTotalLenght)
	{
		SerializedProperty animationTime = sobj.FindProperty("animationTime");
		SerializedProperty animationStartTime = sobj.FindProperty("animationStartTime");
		SerializedProperty timelineEvents = sobj.FindProperty("_timelineEvents");
		
		float secondInterval = trackScale / 1.2f;
		float drawX = rect.x;
		float secondLenght = secondInterval * trackLenght;
		float frameInterval = secondLenght / 10.0f;
		float maxTime = (1.0f / trackScale) * 1.2f;
		float totalLenght = animationTime.floatValue * 1.2f * secondLenght;
		totalLenght = totalLenght < trackLenght ? trackLenght : totalLenght;
		timelineTotalLenght = totalLenght;
		trackMaxTime = maxTime;

		int frameIndex = 0;
		while (true)
		{
			float frameHeight = 3;
			float frameWidht = 0.3f;

			if (frameIndex % 5 == 0)
			{
				frameHeight = 8;
				frameWidht = 0.4f;
			}
			
			if (frameIndex % 10 == 0)
			{
				frameHeight = 16;
				frameWidht = 0.5f;
				int count = frameIndex / 10;
				GUI.Label(new Rect(drawX,rect.y,30,15),count.ToString("00")+ ":00",EditorStyles.miniLabel);
			}
			
			GUI.DrawTexture(new Rect(drawX,rect.y + 20 - frameHeight,frameWidht,frameHeight),boxTexture);
			drawX += frameInterval;

			frameIndex++;
			if ((drawX - rect.x) > totalLenght)
			{
				break;
			}
		}
		
		GUI.DrawTexture(new Rect(rect.x,rect.y + 20,totalLenght,0.5f),boxTexture);
		
		//Draw timeline evnt
		for (int i = 0; i < timelineEvents.arraySize; i++)
		{
			SerializedProperty thisEvent = timelineEvents.GetArrayElementAtIndex(i);
			SerializedProperty thisEventTime = thisEvent.FindPropertyRelative("m_eventTime");
			float eventX = thisEventTime.floatValue * trackLenght / maxTime;
			Rect eventRect = new Rect(rect.x + eventX - 3,rect.y + 21,6,19);
			if (i == clickTimelineEventIndex)
			{
				GUI.color = new Color(0.4f,0.8f,0.8f,1);
			}
			else
			{
				GUI.color = new Color(0.7f,0.7f,0.7f,1);
			}
			
			GUI.DrawTexture(eventRect,boxTexture);
			GUI.color = Color.white;

			if (inputEvent.isMouse && inputEvent.button == 0 && eventRect.Contains(mousePosition))
			{
				if (inputEvent.type == EventType.MouseDown)
				{
					clickTimelineEventIndex = i;
					clickTimelineEvent = true;
					clickTimelineEventTime = thisEventTime.floatValue;
					Repaint();
				}
			}
			
			if (inputEvent.isMouse && inputEvent.button == 0 && clickTimelineEvent == true &&
			    inputEvent.type == EventType.MouseDrag && i == clickTimelineEventIndex)
			{
				float moveV = mousePosition.x - mouseDownPosition.x;
				thisEventTime.floatValue = clickTimelineEventTime + (moveV * maxTime / trackLenght);
				Repaint();
			}
		}

		if (inputEvent.type == EventType.MouseUp)
		{
			clickTimelineEvent = false;
		}

		//Draw animation time range
		float timeRangeStart = animationStartTime.floatValue * trackLenght / maxTime + rect.x;
		float timeRangeEnd = animationTime.floatValue * trackLenght / maxTime + rect.x;
		float timeRangeWidth = timeRangeEnd - timeRangeStart;
		timeRangeLeft = timeRangeStart;
		timeRangeRight = timeRangeEnd;
		GUI.color = Color.green;
		GUI.DrawTexture(new Rect( timeRangeStart,rect.y + 21,timeRangeWidth,3),boxTexture);
		GUI.color = Color.white;

		Rect timeLineEventRect = new Rect(rect.x, rect.y, totalLenght, 40);
		if (inputEvent.isMouse && timeLineEventRect.Contains(mousePosition))
		{
			if (inputEvent.button == 1)
			{
				if (inputEvent.type == EventType.MouseDown)
				{
					isClickTimeline = true;
				}

				if (inputEvent.type == EventType.MouseDrag)
				{
					isClickTimeline = false;
				}
				if (inputEvent.type == EventType.MouseUp)
				{
					if (isClickTimeline)
					{
						GenericMenu menu = new GenericMenu();
						GUIContent itemContent = new GUIContent("AddEvent");
						float addTime = (mousePosition.x - rect.x) * maxTime / trackLenght;
						menu.AddItem(itemContent, false, TimelineAddEvent,addTime);
						menu.ShowAsContext();
					}
					isClickTimeline = false;
				}
			}
		}
	}

	void TimelineAddEvent(object v)
	{
		float addTime = (float)v;
		sobj.Update();
		SerializedProperty timelineEvents = sobj.FindProperty("_timelineEvents");
		timelineEvents.arraySize++;
		SerializedProperty lastEvent = timelineEvents.GetArrayElementAtIndex(timelineEvents.arraySize - 1);
		if (lastEvent != null)
		{
			SerializedProperty eventTime = lastEvent.FindPropertyRelative("m_eventTime");
			eventTime.floatValue = addTime;
		}
		sobj.ApplyModifiedProperties();
	}

	void DrawTrack(Rect rect,int index,string name, ref float minValue,ref float maxValue,float totalLenght,float scale)
	{
		name = string.IsNullOrEmpty(name) ? "Unnamed" : name;
		if (GUI.Button(rect, name, trackStyle))
		{
			if (inputEvent.button == 1)
			{
				GenericMenu menu = new GenericMenu();
				GUIContent itemContent = new GUIContent("Delete");
				menu.AddItem(itemContent, false, (object v) =>
				{
					isDelete = true;
					deleteIndex = (int)v;
				},index);
				menu.ShowAsContext();
			}
		}
	}

	void TweenSetting(SerializedProperty thisTween)
	{
		if (thisTween != null)
		{
			EditorGUIUtility.labelWidth = 80;
			
			SerializedProperty tweenName = thisTween.FindPropertyRelative("_tweenName");
			SerializedProperty thisTween_TweenType = thisTween.FindPropertyRelative("_tweenType");
			string thisTweenName = "Tween: " + thisTween_TweenType.enumNames[thisTween_TweenType.enumValueIndex];
			if (!string.IsNullOrEmpty(tweenName.stringValue))
				thisTweenName = tweenName.stringValue;
			
			Texture2D tweenIcon = positionIcon;
			if (thisTween_TweenType.enumValueIndex < icons.Length)
				tweenIcon = icons[thisTween_TweenType.enumValueIndex];
			
			GUILayout.BeginHorizontal();
			GUILayout.Button(tweenIcon,EditorStyles.label,GUILayout.Height(20),GUILayout.Width(20));
			GUILayout.Label(thisTweenName);
			GUILayout.EndHorizontal();
			
			EditorGUILayout.PropertyField(tweenName);
			GUILayout.BeginHorizontal();
			GUI.backgroundColor = Color.red;
			GUI.backgroundColor = Color.white;
			GUILayout.EndHorizontal();

			GUILayout.BeginVertical();
			SerializedProperty thisTween_IsSelf = thisTween.FindPropertyRelative("_isSelf");
			SerializedProperty thisTween_TweenObject = thisTween.FindPropertyRelative("_tweenObject");
			SerializedProperty thisTween_Curve = thisTween.FindPropertyRelative("_curve");
			SerializedProperty thisTween_MinAnimationTime = thisTween.FindPropertyRelative("_minAnimationTime");
			SerializedProperty thisTween_MaxAnimationTime = thisTween.FindPropertyRelative("_maxAnimationTime");
			SerializedProperty thisTween_IsLocal = thisTween.FindPropertyRelative("_isLocal");

			EditorGUILayout.PropertyField (thisTween_TweenType);
			
			EditorGUILayout.PropertyField(thisTween_IsSelf);
			if (!thisTween_IsSelf.boolValue)
			{ 
				EditorGUILayout.PropertyField(thisTween_TweenObject);
			}
			else
			{
				thisTween_TweenObject.objectReferenceValue = runtimeObj.gameObject;
			}

			GUILayout.TextArea("", GUILayout.Height(2));
			EditorGUILayout.PropertyField(thisTween_IsLocal);
			EditorGUILayout.PropertyField(thisTween_Curve);

			if (thisTween_TweenType != null)
			{
				int tweenTypeID = thisTween_TweenType.enumValueIndex;
				switch (tweenTypeID)
				{
					case 0:
					case 1:
					case 2:
						SerializedProperty thisFromVector = thisTween.FindPropertyRelative("_fromVector");
						SerializedProperty thisToVector = thisTween.FindPropertyRelative("_toVector");

						GUILayout.BeginHorizontal();
						if (GUILayout.Button("SetFromValue", EditorStyles.miniButtonLeft))
						{
							SetCurrentValueToTween(thisTween, true);
						}

						if (GUILayout.Button("SetToValue", EditorStyles.miniButtonRight))
						{
							SetCurrentValueToTween(thisTween, false);
						}

						GUILayout.EndHorizontal();

						Vector3 fromVector =
							EditorGUILayout.Vector3Field("From Vector", thisFromVector.vector3Value);
						Vector3 toVecor = EditorGUILayout.Vector3Field("To Vector", thisToVector.vector3Value);

						thisFromVector.vector3Value = fromVector;
						thisToVector.vector3Value = toVecor;
						break;

					case 3:
						SerializedProperty fromColor = thisTween.FindPropertyRelative("_fromColor");
						SerializedProperty toColor = thisTween.FindPropertyRelative("_toColor");
						EditorGUILayout.PropertyField(fromColor);
						EditorGUILayout.PropertyField(toColor);
						break;

					case 4:
					case 5:
						SerializedProperty fromAlpha = thisTween.FindPropertyRelative("_fromFloat");
						SerializedProperty toAlpha = thisTween.FindPropertyRelative("_toFloat");
						EditorGUILayout.PropertyField(fromAlpha);
						EditorGUILayout.PropertyField(toAlpha);
						break;

					case 6:
						SerializedProperty bezierCurve = thisTween.FindPropertyRelative("_bezierCurve");
						EditorGUILayout.PropertyField(bezierCurve, new GUIContent("Curve:"));
						break;
				}
			}

			GUILayout.EndVertical();
		}
	}

	void DrawTimelineEventSetting()
	{
		if(sobj == null)return;
		SerializedProperty timelineEvents = sobj.FindProperty("_timelineEvents");
		if (clickTimelineEventIndex >= 0 && clickTimelineEventIndex < timelineEvents.arraySize)
		{
			SerializedProperty thisEvent = timelineEvents.GetArrayElementAtIndex(clickTimelineEventIndex);
			SerializedProperty eventTime = thisEvent.FindPropertyRelative("m_eventTime");
			SerializedProperty events = thisEvent.FindPropertyRelative("m_events");

			EditorGUILayout.PropertyField(eventTime);
			EditorGUILayout.PropertyField(events);

			if (eventTime.floatValue < 0)
				eventTime.floatValue = 0;

			if (GUILayout.Button("Delete Event",GUILayout.Width(100)))
			{
				timelineEvents.DeleteArrayElementAtIndex(clickTimelineEventIndex);
				clickTimelineEventIndex = -1;
			}
		}
	}
	
	void AddTween (SerializedProperty tweens)
	{
		tweens.arraySize += 1;
		SerializedProperty thisTween = tweens.GetArrayElementAtIndex (tweens.arraySize - 1);

		SerializedProperty thisTweenName = thisTween.FindPropertyRelative("_tweenName");
		SerializedProperty tweenObject = thisTween.FindPropertyRelative ("_tweenObject");

		SerializedProperty thisTween_IsSelf = thisTween.FindPropertyRelative ("_isSelf");
		SerializedProperty thisTween_IsLocal = thisTween.FindPropertyRelative ("_isLocal");

		SerializedProperty thisTweenType = thisTween.FindPropertyRelative ("_tweenType");
		SerializedProperty thisTween_Curve = thisTween.FindPropertyRelative ("_curve");

		SerializedProperty thisTween_MinAnimationTime = thisTween.FindPropertyRelative ("_minAnimationTime");
		SerializedProperty thisTween_MaxAnimationTime = thisTween.FindPropertyRelative ("_maxAnimationTime");
		SerializedProperty minTime = thisTween.FindPropertyRelative("_minTime");
		SerializedProperty maxTime = thisTween.FindPropertyRelative("_maxTime");

		thisTweenName.stringValue = "";
		thisTweenType.enumValueIndex = (int)addTweenType;
		thisTween_IsSelf.boolValue = true;
		thisTween_IsLocal.boolValue = true;

		thisTween_Curve.animationCurveValue = new AnimationCurve(new Keyframe(0,0),new Keyframe(1,1));

		thisTween_MinAnimationTime.floatValue = 0f;
		thisTween_MaxAnimationTime.floatValue = 1f;
		minTime.floatValue = 0;
		maxTime.floatValue = 1;

		if(thisTween_IsSelf.boolValue)
			tweenObject.objectReferenceValue = runtimeObj.transform;

		SetCurrentValueToTween (thisTween,false);
		SetCurrentValueToTween (thisTween,true);
	}

	void SetCurrentValueToTween(SerializedProperty thisTween, bool isFromValue)
	{
		SerializedProperty tweenObject = thisTween.FindPropertyRelative("_tweenObject");
		SerializedProperty isLocal = thisTween.FindPropertyRelative("_isLocal");
		SerializedProperty type = thisTween.FindPropertyRelative("_tweenType");

		SerializedProperty fromVector = thisTween.FindPropertyRelative("_fromVector");
		SerializedProperty toVector = thisTween.FindPropertyRelative("_toVector");

		SerializedProperty fromColor = thisTween.FindPropertyRelative("_fromColor");
		SerializedProperty toColor = thisTween.FindPropertyRelative("_toColor");

		SerializedProperty fromFloat = thisTween.FindPropertyRelative("_fromFloat");
		SerializedProperty toFloat = thisTween.FindPropertyRelative("_toFloat");

		Vector3 vectorValue = Vector3.zero;
		if (tweenObject.objectReferenceValue != null)
		{
			Transform tweenObj = (Transform)(tweenObject.objectReferenceValue);
			switch (type.enumValueIndex)
			{
				case 0:
					vectorValue = isLocal.boolValue ? tweenObj.localPosition : tweenObj.position;
					break;

				case 1:
					vectorValue = isLocal.boolValue ? tweenObj.localEulerAngles : tweenObj.eulerAngles;
					break;

				case 2:
					vectorValue = Vector3.one;
					vectorValue = isLocal.boolValue ? tweenObj.localScale : tweenObj.lossyScale;
					break;

				case 4:
					fromFloat.floatValue = 0;
					toFloat.floatValue = 1;
					break;

				case 5:
					Transform cameraTran = (Transform)tweenObject.objectReferenceValue;
					Camera camera = cameraTran.GetComponent<Camera>();
					if (camera != null)
					{
						fromFloat.floatValue = camera.fieldOfView;
						toFloat.floatValue = camera.fieldOfView;
					}
					break;

				case 6:
					break;
			}
		}
		
		if(isFromValue)
			fromVector.vector3Value = vectorValue;
		else
			toVector.vector3Value = vectorValue;

		fromColor.colorValue = Color.white;
		toColor.colorValue = Color.white;
	}
}
