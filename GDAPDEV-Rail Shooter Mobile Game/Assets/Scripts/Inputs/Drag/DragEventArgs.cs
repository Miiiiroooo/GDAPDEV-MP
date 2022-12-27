using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DragEventArgs : EventArgs
{
    private Touch _joystickFinger;
    private GameObject _hitObject;


    public DragEventArgs(Touch joystickFinger, GameObject obj = null)
    {
        _joystickFinger = joystickFinger;
        _hitObject = obj;
    }

    public GameObject HitObject
    {
        get
        {
            return _hitObject;
        }
    }

    public Touch JoystickFinger
    {
        get
        {
            return _joystickFinger;
        }
    }
}
