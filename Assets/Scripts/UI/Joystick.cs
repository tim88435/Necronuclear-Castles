using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    #region SerializeField

    [SerializeField] private float _handlerRange = 1;
    [SerializeField] private float _deadZone = 0;
    [SerializeField] private AxisOptions _axisOptions = AxisOptions.Both;
    [SerializeField] private bool _snapX = false;
    [SerializeField] private bool _snapY = false;
    [SerializeField] protected RectTransform background = null;
    [SerializeField] private RectTransform _handle = null;

    #endregion

    #region References

    private RectTransform _baseRect = null;
    private Canvas _canvas;
    private Camera _cam;
    public Vector2 input = Vector2.zero;

    #endregion

    #region Properties

    public bool SnapX
    {
        get { return _snapX; }
        set { _snapX = value; }
    }

    public bool SnapY
    {
        get { return _snapY; }
        set { _snapY = value; }
    }

    public float HandleRange
    {
        get { return _handlerRange; }
        set { _handlerRange = Mathf.Abs(value); }
    }

    public float DeadZone
    {
        get { return _deadZone; }
        set { _deadZone = Mathf.Abs(value); }
    }

    public AxisOptions AxisOption
    {
        get { return _axisOptions; }
        set { _axisOptions = value; }
    }

    public float Horizontal
    {
        get { return (SnapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; }
    }

    public float Vertical
    {
        get { return (SnapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; }
    }

    public Vector2 Direction
    {
        get { return new Vector2(Horizontal, Vertical); }
    }

    #endregion

    #region Interface

    protected virtual void Start()
    {
        //not sure
        HandleRange = _handlerRange;
        DeadZone = _deadZone;
        //

        //this script will be attached to the background image
        _baseRect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError(
                "The Joystick is not placed inside the canvas! or the script is not on the background image! der Scrub!!");
        }

        Vector2 _center = new Vector2(0.5f, 0.5f);
        background.pivot = _center;
        _handle.anchorMin = _center;
        _handle.anchorMax = _center;
        _handle.pivot = _center;
        _handle.anchoredPosition = Vector2.zero;
    }

    private float SnapFloat(float value, AxisOptions snapAxis)
    {
        if (value == 0)
        {
            return value;
        }

        if (AxisOption == AxisOptions.Both)
        {
            float _angle = Vector2.Angle(input, Vector2.up);
            if (snapAxis == AxisOptions.Horizontal)
            {
                if (_angle < 22.5f || _angle > 157.5f)
                {
                    return 0;
                }
                else
                {
                    return (value > 0) ? 1 : -1;
                }
            }

            if (snapAxis == AxisOptions.Vertical)
            {
                if (_angle < 67.5f || _angle > 112.5f)
                {
                    return 0;
                }
                else
                {
                    return (value > 0) ? 1 : -1;
                }
            }

            //this should never run
            return value;
        }
        else
        {
            if (value > 0)
            {
                return 1;
            }

            if (value < 0)
            {
                return -1;
            }
        }

        //this should never run
        return 0;
    }

    private void FormatInput()
    {
        if (AxisOption == AxisOptions.Horizontal)
        {
            input = new Vector2(input.x, 0f);
        }
        else if (AxisOption == AxisOptions.Vertical)
        {
            input = new Vector2(0f, input.y);
        }
    }

    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera camera)
    {
        if (magnitude > DeadZone)
        {
            if (magnitude > 1)
            {
                input = normalised;
            }
        }
        else
        {
            input = Vector2.zero;
        }
    }

    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        Vector2 _localPoint = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_baseRect, screenPosition, _cam, out _localPoint))
        {
            return _localPoint - (background.anchorMax * _baseRect.sizeDelta);
        }

        return Vector2.zero;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (_cam == null)
        {
            if (_canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                _cam = _canvas.worldCamera;
            }
        }
        Vector2 _position = RectTransformUtility.WorldToScreenPoint(_cam, background.position);
        Vector2 _radius = background.sizeDelta / 2;
        input = (eventData.position - _position) / (_radius * _canvas.scaleFactor);
        FormatInput();
        HandleInput(input.magnitude, input.normalized, _radius, _cam);
        _handle.anchoredPosition = input * _radius * HandleRange;
        //if this plays up we will be changing HandleRange to _handlerRange
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        _handle.anchoredPosition = Vector2.zero;
    }
    #endregion

    public enum AxisOptions
    {
        Both,
        Horizontal,
        Vertical,
    }
}