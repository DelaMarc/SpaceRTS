using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class Jun_Tween
{
	[HideInInspector] [SerializeField] private string _tweenName = "";
	[HideInInspector][SerializeField]Jun_TweenType _tweenType = Jun_TweenType.PositionTween;
	[HideInInspector][SerializeField]bool _isSelf = false;
	[HideInInspector][SerializeField]Transform _tweenObject;

	public bool isSelf { get { return _isSelf; }}

	#region<Tween Setting>
	[HideInInspector][SerializeField]AnimationCurve _curve = new AnimationCurve(new Keyframe(0,0),new Keyframe(1,1));

	[HideInInspector][SerializeField]float _minAnimationTime = 0f;
	[HideInInspector][SerializeField]float _maxAnimationTime = 1f;
	[HideInInspector] [SerializeField]float _minTime = 0;
	[HideInInspector] [SerializeField]float _maxTime = 1;

	public float minAnimationTime{get{return _minAnimationTime;}}
	public float maxAnimationTime{get{return _maxAnimationTime;}}

	[Space(10)]
	[HideInInspector][SerializeField]bool _isLocal = true;
       
	private bool _isPlaying = false;
	public bool isPlaying {get{return _isPlaying;}}
	#endregion

	#region<Vectors>
	[HideInInspector][SerializeField]Vector3 _fromVector = Vector3.one;
	[HideInInspector][SerializeField]Vector3 _toVector = Vector3.one;

	public Vector3 fromVector{get{return _fromVector;}set {_fromVector = value;}}
	public Vector3 toVector {get{return _toVector;}set {_toVector = value;}}
	#endregion

	#region<Color>
	[HideInInspector][SerializeField]Color _fromColor = Color.white;
	[HideInInspector][SerializeField]Color _toColor = Color.white;

	public Color fromColor {get{return _fromColor;}set {_fromColor = value;}}
	public Color toColor {get{return _toColor;}set {_toColor = value;}}
	#endregion

	#region<Float>
	[HideInInspector][SerializeField]float _fromFloat = 0;
	[HideInInspector][SerializeField]float _toFloat = 1;

	public float fromValue {get{return _fromFloat;}set {_fromFloat = value;}}
	public float toValue {get{return _toFloat;}set{_toFloat = value;}}
    #endregion

    #region<GUI Alpha>
    private CanvasGroup canvasGroup;
	#endregion
    
	#region<Color Component>
	private MaskableGraphic maskableGraphic;
	private Renderer render;
	private SpriteRenderer spriteRender;
	private ParticleSystem particleSystem;
    #endregion

    //Camera
    private Camera tweenCamera;

    //BezierCurve
    [HideInInspector] [SerializeField] Jun_BezierCurve _bezierCurve;

    public void Init (Transform setObj)
	{
        _tweenObject = setObj;
		if(_tweenObject == null)
			return;
		
        switch (_tweenType)
        {
            case Jun_TweenType.PositionTween:
                break;

            case Jun_TweenType.RotateTween:
                break;

            case Jun_TweenType.ScaleTween:
                break;

            case Jun_TweenType.ColorTween:
            case Jun_TweenType.AlphaTween:
                if (_tweenObject != null)
                {
                    maskableGraphic = _tweenObject.GetComponent<MaskableGraphic>();
                    render = _tweenObject.GetComponent<Renderer>();
                    spriteRender = _tweenObject.GetComponent<SpriteRenderer>();
                    particleSystem = _tweenObject.GetComponent<ParticleSystem>();
                    canvasGroup = _tweenObject.GetComponent<CanvasGroup>();
                }
                break;

            case Jun_TweenType.FieldOfViewTween:
                tweenCamera = _tweenObject.GetComponent<Camera>();
                break;

            case Jun_TweenType.BezierCurve:
                break;
        }
	}

	public void PlayAtTime (float curveValue)
	{
		if(_tweenObject == null)
			return;

		curveValue = _curve.Evaluate (curveValue);
		
		switch (_tweenType)
		{
			case Jun_TweenType.PositionTween:
				if (_isLocal)
					_tweenObject.localPosition = (toVector - fromVector) * curveValue + fromVector;
				else
					_tweenObject.position = (toVector - fromVector) * curveValue + fromVector;
				break;

			case Jun_TweenType.RotateTween:
				if (_isLocal)
					_tweenObject.localEulerAngles = (toVector - fromVector) * curveValue + fromVector;
				else
					_tweenObject.eulerAngles = (toVector - fromVector) * curveValue + fromVector;
				break;

			case Jun_TweenType.ScaleTween:
				_tweenObject.localScale = (toVector - fromVector) * curveValue + fromVector;
				break;

			case Jun_TweenType.ColorTween:
				Color curColor = (toColor - fromColor) * curveValue + fromColor;

				if (maskableGraphic != null)
					maskableGraphic.color = curColor;

				if (spriteRender != null)
					spriteRender.color = curColor;
				else if (render != null)
				{
					if (render.material != null)
						render.material.color = curColor;
				}

				if (particleSystem != null)
				{
					var particleMain = particleSystem.main;
					particleMain.startColor = curColor;
				}
				break;

			case Jun_TweenType.AlphaTween:
				float curAlpha = (toValue - fromValue) * curveValue + fromValue;
				curAlpha = curAlpha < 0 ? 0 : curAlpha;
				curAlpha = curAlpha > 1 ? 1 : curAlpha;

				if (maskableGraphic != null)
					maskableGraphic.color = new Color(maskableGraphic.color.r, maskableGraphic.color.g, maskableGraphic.color.b, curAlpha);

				if (spriteRender != null)
				{
					spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, curAlpha);
				}
				else if (render != null)
				{
                    if (render.material != null)
                    {
                        Color currentColor = render.material.GetColor("_TintColor");
                        Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, curAlpha);
                        render.material.SetColor("_TintColor",newColor);
                    }
				}

				if (particleSystem != null)
				{
					var particleMain = particleSystem.main;
					particleMain.startColor = new Color(particleMain.startColor.color.r, particleMain.startColor.color.g, particleMain.startColor.color.b, curAlpha);
				}

				if(canvasGroup != null)
                {
                    canvasGroup.alpha = curAlpha;
                }
				break;

			case Jun_TweenType.FieldOfViewTween:
                float curField = (toValue - fromValue) * curveValue + fromValue;
                if (tweenCamera != null)
                    tweenCamera.fieldOfView = curField;
				break;

            case Jun_TweenType.BezierCurve:
	            if (_bezierCurve != null)
	            {
		            Vector3 curPosition = _bezierCurve.GetPointInCurve(curveValue);
		            if (_isLocal)
			            _tweenObject.localPosition = curPosition;
		            else
			            _tweenObject.position = curPosition;
	            }
	            break;
		}
	}

	//Play
	public void Play ()
	{
		_isPlaying = true;
	}

	public void SetValue (Vector3 setFromVector,Vector3 setToVector)
	{
		fromVector = setFromVector;
		toVector = setToVector;
	}

	public void SetValue (Color setFromColor,Color setToColor)
	{
		fromColor = setFromColor;
		toColor = setToColor;
	}

	public void SetValue (float setFromFloat,float setToFloat)
	{
		fromValue = setFromFloat;
		toValue = setToFloat;
	}
}
