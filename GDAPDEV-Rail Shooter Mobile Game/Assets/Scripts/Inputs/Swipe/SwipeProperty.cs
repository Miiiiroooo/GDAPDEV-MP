using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SwipeProperty
{
    [Tooltip("Minimum Distance covered to be considered a Swipe")]
    public float minSwipeDistance = 1.4f;
    [Tooltip("Max gesture time until it is not a swipe anymore")]
    public float swipeTime = 0.6f;
}
