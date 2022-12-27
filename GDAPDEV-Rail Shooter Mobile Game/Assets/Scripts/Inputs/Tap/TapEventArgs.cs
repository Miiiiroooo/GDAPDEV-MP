using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TapEventArgs : EventArgs
{
    private Vector2 _tapPosition;
    private GameObject _tapObject;


    public TapEventArgs(Vector2 pos, GameObject obj = null)
    {
        _tapPosition = pos;
        _tapObject = obj;
    }

    public Vector2 TapPosition
    {
        get
        {
            return _tapPosition;
        }
    }

    public GameObject TapObject
    {
        get
        {
            return _tapObject;
        }
    }
}
