using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyMode : Joystick
{
    [SerializeField] private float _moveThreshold = 1;
    [SerializeField] private JoystickType _joystickType = JoystickType.Fixed;
    public float MoveThreshold
    {
        get { return _moveThreshold; }
        set { _moveThreshold = Mathf.Abs(value); }
    }
    private Vector2 _fixedPosition = Vector2.zero;

    public void SetMode(JoystickType joystickType)
    {
        _joystickType = joystickType;
        if (joystickType == JoystickType.Fixed)
        {
            background.anchoredPosition = _fixedPosition;
            background.gameObject.SetActive(true);
        }
        else
        {
            background.gameObject.SetActive(false);
        }
    }

    protected override void Start()
    {
        base.Start();
        _fixedPosition = background.anchoredPosition;
        SetMode(_joystickType);
    }

    public void Update()
    {
        if (_joystickType == JoystickType.Fixed && background.gameObject.activeSelf == false)
        {
            SetMode(JoystickType.Fixed);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (_joystickType != JoystickType.Fixed)
        {
            background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            background.transform.position = eventData.position;
            background.gameObject.SetActive(true);
        }
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (_joystickType != JoystickType.Fixed)
        {
            background.gameObject.SetActive(false);
        }
        base.OnPointerUp(eventData);
    }

    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera camera)
    {
        if (_joystickType == JoystickType.Dynamic && magnitude > MoveThreshold)
        {
            Vector2 difference = normalised * (magnitude - MoveThreshold) * radius;
            background.anchoredPosition += difference;
        }
        base.HandleInput(magnitude, normalised, radius, camera);
    }
}

public enum JoystickType
{
    Fixed,
    Floating,
    Dynamic,
}