using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeEventArgs : EventArgs
{
    public enum SwipeDirections
    { 
        RIGHT,
        LEFT,
        UP,
        DOWN
    }

    private Vector2 _swipePos;
    private SwipeDirections _swipeDirection;
    private Vector2 _swipeVector;
    private GameObject _hitObj;

    public SwipeEventArgs(Vector2 swipePos, SwipeDirections swipeDirection, Vector2 swipeVector, GameObject hitObj)
    {
        this._swipePos = swipePos;
        this._swipeDirection = swipeDirection;
        this._swipeVector = swipeVector;
        this._hitObj = hitObj;
    }

    public Vector2 SwipePos
    {
        get { return _swipePos; }
    }

    public SwipeDirections SwipeDirection
    {
        get { return _swipeDirection; }
    }

    public Vector2 SwipeVector
    {
        get { return _swipeVector; }
    }

    public GameObject HitObj
    {
        get { return _hitObj; }
    }
}
