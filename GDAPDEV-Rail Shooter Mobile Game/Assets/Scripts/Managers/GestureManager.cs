using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class GestureManager : MonoBehaviour
{
    #region FIELDS/ATTRIBUTES
    // singleton 
    public static GestureManager Instance;

    // events 
    public EventHandler<TapEventArgs> onTap;
    public EventHandler<SwipeEventArgs> onSwipe;
    public EventHandler<DragEventArgs> onDrag;
    public EventHandler<SpreadEventArgs> onSpread;

    // inputs
    public TapProperty _tapProperty;
    public SwipeProperty _swipeProperty;
    public SpreadProperty _spreadProperty;

    private Touch trackedFinger1;
    private Touch trackedFinger2;
    private Touch joyStickFinger;
    private int joystickTouchID = -1;
    private int totalCountWhenJoystickWasDetected = -1;

    private Vector2 startPoint = new Vector2(-1,-1);
    private Vector2 endPoint = new Vector2(-1, -1);
    private float gestureTime = 0.0f;

    // extra fields
    [SerializeField] private GameObject joystickPanel;
    private bool isTakingInputs = false;
    #endregion


    // Unity Methods
    void Awake()
    {
        // singleton properties
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        // event-broadcasting
        EventBroadcaster.Instance.AddObserver(EventNames.ON_CHANGE_GAME_STATE, OnChangeGameState);
    }

    void OnDestroy()
    {
        EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.ON_CHANGE_GAME_STATE, OnChangeGameState);
    }

    void Update()
    {
        if (Input.touchCount > 0 && isTakingInputs)
        {
            CheckJoystickInputs();
            CheckOtherFingerInputs();
        }
    }


    // Methods for Checking Inputs
    private void CheckJoystickInputs()
    {
        RectTransform joystickTransform = joystickPanel.GetComponent<RectTransform>();

        if (joystickTouchID == -1)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began && RectTransformUtility.RectangleContainsScreenPoint(joystickTransform, Input.GetTouch(i).position))
                {
                    joystickTouchID = i;
                    joyStickFinger = Input.GetTouch(joystickTouchID);
                    totalCountWhenJoystickWasDetected = Input.touchCount;
                }
            }
        }
        else
        {
            int id = Input.touchCount < totalCountWhenJoystickWasDetected && joystickTouchID != 0 ? 
                     joystickTouchID - (totalCountWhenJoystickWasDetected - Input.touchCount) : 
                     joystickTouchID;

            joyStickFinger = Input.GetTouch(id);

            if (joyStickFinger.phase == TouchPhase.Ended)
            {
                joystickTouchID = -1;
            }

            FireDragEvent();
        }
    }

    private void CheckOtherFingerInputs()
    {
        List<int> touchIDList = new List<int>();

        if (joystickTouchID != -1 && Input.touchCount == 1)
            return;
        else
            for (int i = 0; i < Input.touchCount; i++)
                if (i != joystickTouchID)
                    touchIDList.Add(i);

        if (touchIDList.Count == 1)
        {
            trackedFinger1 = Input.GetTouch(touchIDList[0]);
            CheckSingleFingerGestures();                    
        }
        else if (touchIDList.Count > 1)
        {
            trackedFinger1 = Input.GetTouch(touchIDList[0]);
            trackedFinger2 = Input.GetTouch(touchIDList[1]);

            if ((trackedFinger1.phase == TouchPhase.Moved || trackedFinger2.phase == TouchPhase.Moved))
            {
                Vector2 prevPoint1 = GetPreviousPoint(trackedFinger1);
                Vector2 prevPoint2 = GetPreviousPoint(trackedFinger2);

                float currDistance = Vector2.Distance(trackedFinger1.position, trackedFinger2.position);
                float prevDistance = Vector2.Distance(prevPoint1, prevPoint2);

                if (Math.Abs(currDistance - prevDistance) >= (_spreadProperty.MinDistanceChange * Screen.dpi))
                {
                    FireSpreadEvent(currDistance - prevDistance);
                }
            }
        }
    }

    private void CheckSingleFingerGestures()
    {
        if (trackedFinger1.phase == TouchPhase.Began)
        {
            startPoint = trackedFinger1.position;
            gestureTime = 0.0f;
        }
        else if (trackedFinger1.phase == TouchPhase.Ended && startPoint.x != -1 && startPoint.y != -1)
        {
            endPoint = trackedFinger1.position;

            // TAP
            if (gestureTime <= _tapProperty.tapTime &&
                Vector2.Distance(startPoint, endPoint) < (Screen.dpi * _tapProperty.tapMaxDistance))
            {
                FireTapEvent(trackedFinger1.position);
            }
            // SWIPE
            else if (gestureTime <= _swipeProperty.swipeTime &&
                Vector2.Distance(startPoint, endPoint) < (Screen.dpi * _swipeProperty.minSwipeDistance))
            {
                FireSwipeEvent();
            }

            startPoint = new Vector2(-1, -1);
            endPoint = new Vector2(-1, -1);
        }
        else
        {
            gestureTime += Time.deltaTime;
        }
    }

    private Vector2 GetPreviousPoint(Touch finger)
    {
        return finger.position - finger.deltaPosition;
    }


    // Methods for Input Events
    private void FireTapEvent(Vector2 pos)
    {
        GameObject hitObj = null;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit = new RaycastHit();
        int checkpoint_mask = LayerMask.GetMask("Checkpoint");
        int enemy_layer = LayerMask.NameToLayer("Enemy");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~checkpoint_mask))
        {
            if (hit.collider.gameObject.layer == enemy_layer)
            { 
                hitObj = hit.collider.gameObject;
            }
            //Debug.Log(hit.transform.name);
        }

        TapEventArgs tapArgs = new TapEventArgs(pos, hitObj);
        if (onTap != null)
        {
            onTap(this, tapArgs);
        }

        if (hitObj != null)
        {
            ITappable handler = hitObj.GetComponent<ITappable>();
            if (handler != null)
            {
                handler.OnTap();
            }
        }
    }

    private void FireSwipeEvent()
    {
        // determine swipe direction
        SwipeEventArgs.SwipeDirections swipeDir = SwipeEventArgs.SwipeDirections.RIGHT;
        Vector2 dir = endPoint - startPoint;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0) swipeDir = SwipeEventArgs.SwipeDirections.RIGHT;
            else swipeDir = SwipeEventArgs.SwipeDirections.LEFT;
        }
        else
        {
            if (dir.y > 0) swipeDir = SwipeEventArgs.SwipeDirections.UP;
            else swipeDir = SwipeEventArgs.SwipeDirections.DOWN;
        }

        SwipeEventArgs swipeArgs = new SwipeEventArgs(startPoint, swipeDir, dir, null);
        if (onSwipe != null)
        {
            onSwipe(this, swipeArgs);
        }
    }

    private void FireDragEvent()
    {
        DragEventArgs args = new DragEventArgs(joyStickFinger, null);

        if (onDrag != null)
        {
            onDrag(this, args);
        }
    }

    private void FireSpreadEvent(float spreadDistance)
    {
        if (onSpread != null)
        {
            SpreadEventArgs spreadArgs = new SpreadEventArgs(trackedFinger1, trackedFinger2, spreadDistance, null);
            onSpread(this, spreadArgs);
        }
    }


    // Methods for Delegates
    private void OnChangeGameState()
    {
        GameManager.GameStates currentState = GameManager.Instance.GetCurrentGameState();

        switch (currentState)
        {
            case GameManager.GameStates.Countdown:
                StopTakingInputs();
                break;
            case GameManager.GameStates.Gameplay:
                ResumeTakingInputs();
                break;
            case GameManager.GameStates.GameOver:
                StopTakingInputs();
                break;
            case GameManager.GameStates.BossDefeated:
                StopTakingInputs();
                break;
            case GameManager.GameStates.Shop:
                StopTakingInputs();
                break;
            case GameManager.GameStates.Settings:
                StopTakingInputs();
                break;
            case GameManager.GameStates.Debug:
                StopTakingInputs();
                break;
        }
    }


    // Other User-Defined Methods
    private void StopTakingInputs()
    {
        this.isTakingInputs = false;
    }

    private void ResumeTakingInputs()
    {
        this.isTakingInputs = true;
    }
}
