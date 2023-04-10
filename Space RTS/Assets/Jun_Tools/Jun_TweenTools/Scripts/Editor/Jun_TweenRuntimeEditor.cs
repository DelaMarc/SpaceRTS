using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor (typeof (Jun_TweenRuntime))]
public class Jun_TweenRuntimeEditor : Editor 
{
	Vector2 tweenListPos;
	Jun_TweenType addTweenType = Jun_TweenType.PositionTween;

	private bool isPlaying = false;
	private float playTime;

    Texture2D positionIcon;
    Texture2D rotateIcon;
    Texture2D scaleIcon;
    Texture2D colorIcon;
    Texture2D alphaIcon;
    Texture2D fieldOfViewIcon;
    Texture2D pathIcon;

    Texture2D[] icons;

	public override void OnInspectorGUI ()
	{
        positionIcon = Resources.Load("Position", typeof(Texture2D)) as Texture2D;
        rotateIcon = Resources.Load("Rotate", typeof(Texture2D)) as Texture2D;
        scaleIcon = Resources.Load("Scale", typeof(Texture2D)) as Texture2D;
        colorIcon = Resources.Load("Color", typeof(Texture2D)) as Texture2D;
        alphaIcon = Resources.Load("Alpha", typeof(Texture2D)) as Texture2D;
        fieldOfViewIcon = Resources.Load("FieldOfView", typeof(Texture2D)) as Texture2D;
        pathIcon = Resources.Load("Path", typeof(Texture2D)) as Texture2D;

        icons = new Texture2D[7] { positionIcon, rotateIcon, scaleIcon, colorIcon, alphaIcon, fieldOfViewIcon, pathIcon };
        SerializedProperty m_onFinish = serializedObject.FindProperty ("m_onFinish");

		Jun_TweenRuntime tar = (Jun_TweenRuntime)target;
		if (GUILayout.Button("Editor"))
		{
			Jun_TweenRuntimeWindow.Open(tar);
		}
		
		EditorGUILayout.PropertyField (m_onFinish);
		serializedObject.ApplyModifiedProperties();
		return;
		
		//Old version
		serializedObject.Update ();

		SerializedProperty playType = serializedObject.FindProperty ("playType");
		SerializedProperty animationTime = serializedObject.FindProperty ("animationTime");
		SerializedProperty enablePlay = serializedObject.FindProperty ("enablePlay");
		SerializedProperty awakePlay = serializedObject.FindProperty ("awakePlay");
		SerializedProperty lgnoreTimeScale = serializedObject.FindProperty ("lgnoreTimeScale");
        SerializedProperty randomStartValue = serializedObject.FindProperty("randomStartValue");
        SerializedProperty startValue = serializedObject.FindProperty("startValue");

        SerializedProperty tweens = serializedObject.FindProperty ("tweens");

		GUILayout.BeginVertical (GUILayout.MinHeight (100));
		EditorGUILayout.PropertyField (playType);
		EditorGUILayout.PropertyField (animationTime);
		EditorGUILayout.PropertyField (enablePlay);
		EditorGUILayout.PropertyField (awakePlay);
		EditorGUILayout.PropertyField (lgnoreTimeScale); 
        EditorGUILayout.PropertyField(randomStartValue);

        //if (!randomStartValue.boolValue)
        EditorGUILayout.PropertyField(startValue);

		GUILayout.BeginVertical (EditorStyles.helpBox);

		if(Application.isPlaying)
			GUI.enabled = false;
		GUILayout.Label ("Preview");

		GUILayout.BeginHorizontal ();
		bool play = GUILayout.Toggle(isPlaying, "▶",EditorStyles.miniButton, GUILayout.Width(20),GUILayout.Height(20));
		if(play != isPlaying)
		{
			playTime = Time.realtimeSinceStartup;
			isPlaying = play;
		}

		if(isPlaying)
		{
			float t = Time.realtimeSinceStartup - playTime;
            if(t > 1)
			{
				playTime = Time.realtimeSinceStartup;
				t = 0;
			}

			tar.previewValue = t;
			EditorGUILayout.Slider(tar.previewValue, 0f, 1f);
		}
		else
		{
			tar.previewValue = EditorGUILayout.Slider(tar.previewValue, 0f, 1f);
		}
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();
		GUI.enabled = true;

		GUILayout.BeginVertical (EditorStyles.helpBox);
		GUILayout.Label ("Tween List");

		GUILayout.BeginHorizontal ();
		addTweenType = (Jun_TweenType)EditorGUILayout.EnumPopup (addTweenType);

		if(GUILayout.Button ("Add Tween",EditorStyles.miniButton))
		{
			AddTween (tweens);
		}
		GUILayout.EndHorizontal ();

		for (int i = 0; i < tweens.arraySize; i++)
		{
            GUILayout.BeginVertical (EditorStyles.helpBox);
			SerializedProperty thisTween = tweens.GetArrayElementAtIndex (i);
			SerializedProperty thisTween_TweenType = thisTween.FindPropertyRelative ("_tweenType");

			bool deleteElement = false;

			string thisTweenName = "Tween" + (i + 1) + ":" + thisTween_TweenType.enumNames[thisTween_TweenType.enumValueIndex];
            Texture2D tweenIcon = positionIcon;
            if (thisTween_TweenType.enumValueIndex < icons.Length)
                tweenIcon = icons[thisTween_TweenType.enumValueIndex];

			GUILayout.BeginHorizontal ();

            GUILayout.BeginHorizontal();
            GUILayout.Button(tweenIcon,EditorStyles.label,GUILayout.Height(20),GUILayout.Width(20));
            GUILayout.Label(thisTweenName);
            GUILayout.EndHorizontal();

			GUI.backgroundColor = Color.red;
			if(GUILayout.Button ("X",EditorStyles.miniButton,GUILayout.Width (20)))
			{
				if(EditorUtility.DisplayDialog ("Delete Tween?","Delete this Tween?","Yes","No"))
				{
					tweens.DeleteArrayElementAtIndex (i);
					deleteElement = true;
				}
			}
			GUI.backgroundColor = Color.white;
			GUILayout.EndHorizontal ();

			if(!deleteElement)
			{
				SerializedProperty thisTween_IsSelf = thisTween.FindPropertyRelative("_isSelf");
				SerializedProperty thisTween_TweenObject = thisTween.FindPropertyRelative("_tweenObject");
				SerializedProperty thisTween_Curve = thisTween.FindPropertyRelative("_curve");
				SerializedProperty thisTween_MinAnimationTime = thisTween.FindPropertyRelative("_minAnimationTime");
				SerializedProperty thisTween_MaxAnimationTime = thisTween.FindPropertyRelative("_maxAnimationTime");
				SerializedProperty thisTween_IsLocal = thisTween.FindPropertyRelative("_isLocal");

				//EditorGUILayout.PropertyField(thisTween_IsSelf);
				if (!thisTween_IsSelf.boolValue)
				{
					EditorGUILayout.PropertyField(thisTween_TweenObject);
				}
				else
				{
					thisTween_TweenObject.objectReferenceValue = tar.gameObject;
				}
				GUILayout.TextArea("", GUILayout.Height(2));
				//EditorGUILayout.PropertyField (thisTween_TweenType);
				EditorGUILayout.PropertyField(thisTween_IsLocal);
				EditorGUILayout.PropertyField(thisTween_Curve);

				#region<Time Range>
				GUILayout.Space(10);
				GUILayout.TextArea("", GUILayout.Height(2));
				GUILayout.Label("Time Range");
				GUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(thisTween_MinAnimationTime, new GUIContent(""), GUILayout.Width(30));
				GUILayout.Label("Min");
				GUILayout.Label("Max", GUILayout.Width(25));
				EditorGUILayout.PropertyField(thisTween_MaxAnimationTime, new GUIContent(""), GUILayout.Width(30));
				GUILayout.EndHorizontal();

				float thisMinAnimaValue = thisTween_MinAnimationTime.floatValue;
				float thisMaxAnimaValue = thisTween_MaxAnimationTime.floatValue;
				EditorGUILayout.MinMaxSlider(ref thisMinAnimaValue, ref thisMaxAnimaValue, 0f, 1f);
				thisTween_MinAnimationTime.floatValue = thisMinAnimaValue;
				thisTween_MaxAnimationTime.floatValue = thisMaxAnimaValue;

				GUILayout.TextArea("", GUILayout.Height(2));
				GUILayout.Space(10);
				#endregion

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

							Vector3 fromVector = EditorGUILayout.Vector3Field("From Vector", thisFromVector.vector3Value);
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

                            GUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(bezierCurve, new GUIContent("Curve:"));

                            if(bezierCurve.objectReferenceValue == null)
                            {
                                if(GUILayout.Button("Create",EditorStyles.miniButton,GUILayout.Width(60)))
                                {
                                    GameObject newObj = new GameObject();
                                    newObj.name = "Path";
                                    newObj.transform.position = tar.transform.position;
                                    bezierCurve.objectReferenceValue = newObj.AddComponent<Jun_BezierCurve>();
                                    Selection.activeGameObject = newObj;
                                }
                            }
                            GUILayout.EndHorizontal();
                            break;
					}
				}
			}
			GUILayout.EndVertical ();
		}
		GUILayout.EndVertical ();

		EditorGUILayout.PropertyField (m_onFinish);
		GUILayout.EndVertical ();

		serializedObject.ApplyModifiedProperties ();
	}

	void AddTween (SerializedProperty tweens)
	{
		Jun_TweenRuntime tar = (Jun_TweenRuntime)target;

		tweens.arraySize += 1;
		SerializedProperty thisTween = tweens.GetArrayElementAtIndex (tweens.arraySize - 1);

		SerializedProperty tweenObject = thisTween.FindPropertyRelative ("_tweenObject");

		SerializedProperty thisTween_IsSelf = thisTween.FindPropertyRelative ("_isSelf");
		SerializedProperty thisTween_IsLocal = thisTween.FindPropertyRelative ("_isLocal");

		SerializedProperty thisTweenType = thisTween.FindPropertyRelative ("_tweenType");
		SerializedProperty thisTween_Curve = thisTween.FindPropertyRelative ("_curve");

		SerializedProperty thisTween_MinAnimationTime = thisTween.FindPropertyRelative ("_minAnimationTime");
		SerializedProperty thisTween_MaxAnimationTime = thisTween.FindPropertyRelative ("_maxAnimationTime");

		thisTweenType.enumValueIndex = (int)addTweenType;
		thisTween_IsSelf.boolValue = true;
		thisTween_IsLocal.boolValue = true;

		thisTween_Curve.animationCurveValue = new AnimationCurve(new Keyframe(0,0),new Keyframe(1,1));

		thisTween_MinAnimationTime.floatValue = 0f;
		thisTween_MaxAnimationTime.floatValue = 1f;

		if(thisTween_IsSelf.boolValue)
			tweenObject.objectReferenceValue = tar.transform;

		SetCurrentValueToTween (thisTween,false);
		SetCurrentValueToTween (thisTween,true);
	}

	void SetCurrentValueToTween (SerializedProperty thisTween,bool isFromValue)
	{
		SerializedProperty tweenObject = thisTween.FindPropertyRelative ("_tweenObject");
		SerializedProperty isLocal = thisTween.FindPropertyRelative ("_isLocal");
		SerializedProperty type = thisTween.FindPropertyRelative ("_tweenType");

		SerializedProperty fromVector = thisTween.FindPropertyRelative ("_fromVector");
		SerializedProperty toVector = thisTween.FindPropertyRelative ("_toVector");

		SerializedProperty fromColor = thisTween.FindPropertyRelative ("_fromColor");
		SerializedProperty toColor = thisTween.FindPropertyRelative ("_toColor");

		SerializedProperty fromFloat = thisTween.FindPropertyRelative ("_fromFloat");
		SerializedProperty toFloat = thisTween.FindPropertyRelative ("_toFloat");

		Vector3 vectorValue = Vector3.zero;
		if(tweenObject.objectReferenceValue != null)
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
                    if(camera != null)
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
