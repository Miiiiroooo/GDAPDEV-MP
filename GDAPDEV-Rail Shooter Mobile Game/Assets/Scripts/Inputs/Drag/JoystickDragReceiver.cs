using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickDragReceiver : MonoBehaviour
{
    [SerializeField] private GameObject joystickPanel;
    [SerializeField] private GameObject insideCircleJoystick;
    private Vector2 originalInnerCircleLocalPos;
    private float insideCircleThreshhold = 75f;

    [SerializeField] Camera playerCamera;
    private Vector3 originalCameraRotation = Vector3.zero;            // saves the original rotation of the camera without the joystick inputs
    private Vector3 deltaRotationFromOriginal = Vector3.zero;         // keeps track of the change in rotation of the camera relative to the original
    private float maxChangeRotationY = 30;                            // maximum rotation to the left and the right of the camera (relative to the y axis of rotation)
    private float maxChangeRotationX = 15;                            // maximum rotation to the top and bottom of the camera (relative to the x axis of rotation)
    private float rotationSpeed = 50f;


    void Start()
    {
        GestureManager.Instance.onDrag += OnDrag;

        originalInnerCircleLocalPos = insideCircleJoystick.GetComponent<RectTransform>().localPosition;    
        originalCameraRotation = playerCamera.gameObject.transform.rotation.eulerAngles;
    }

    void OnDisable()
    {
        GestureManager.Instance.onDrag -= OnDrag;
    }

    private void OnDrag(object send, DragEventArgs args)
    {
        RectTransform joystickTransform = joystickPanel.GetComponent<RectTransform>();
        RectTransform insideCircleTransform = insideCircleJoystick.GetComponent<RectTransform>();

        if (args.JoystickFinger.phase == TouchPhase.Moved || args.JoystickFinger.phase == TouchPhase.Stationary)
        {
            // convert screen position of the touch input into a canvas position
            Vector2 fingerPosInCanvas = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickTransform, args.JoystickFinger.position, null, out fingerPosInCanvas);  

            // get distance between the finger and the middle of joystick
            // then get the direction from the middle to where the finger is located
            float distance = Vector2.Distance(fingerPosInCanvas, originalInnerCircleLocalPos);
            Vector2 direction = fingerPosInCanvas - (Vector2)originalInnerCircleLocalPos;
            direction.Normalize();

            if (distance <= insideCircleThreshhold)
            {
                // when finger is within bounds of joystick, snap the position of the inside circle directly to where the finger is
                insideCircleTransform.position = args.JoystickFinger.position;
            }
            else
            {
                // when finger is beyond the bounds of joystick, change the position of inside circle towards the direction of the finger multiplied by its limit/threshold
                insideCircleTransform.anchoredPosition = direction * insideCircleThreshhold;
            }

            RotateCamera(distance, direction);
        }
        else if (args.JoystickFinger.phase == TouchPhase.Ended)
        {
            // reset to middle
            insideCircleTransform.localPosition = originalInnerCircleLocalPos; 
            //insideCircleTransform.anchoredPosition = Vector2.zero; 
        }        
    }

    private void RotateCamera(float magnitude, Vector2 direction)
    {
        // init the multiplier based on the magnitude between the finger and the joystick
        float multiplier = magnitude / 75f;

        if (multiplier < 0.3)
            multiplier = 0.3f;
        else if (multiplier > 1f)
            multiplier = 1f;


        // init the rotation in the x and y axis of rotations
        float deltaRotationY = multiplier * direction.x * rotationSpeed * Time.deltaTime;
        float deltaRotationX = (multiplier * direction.y * rotationSpeed * Time.deltaTime) * -1;   // multiply by -1, up is negative degrees and down is positive


        // init the rotation of the camera with the previous calculations
        deltaRotationFromOriginal.y += deltaRotationY;

        if (deltaRotationFromOriginal.y > maxChangeRotationY)
            deltaRotationFromOriginal.y = maxChangeRotationY;
        else if (deltaRotationFromOriginal.y < -maxChangeRotationY)
            deltaRotationFromOriginal.y = -maxChangeRotationY;

        deltaRotationFromOriginal.x += deltaRotationX;

        if (deltaRotationFromOriginal.x > maxChangeRotationX)
            deltaRotationFromOriginal.x = maxChangeRotationX;
        else if (deltaRotationFromOriginal.x < -maxChangeRotationX)
            deltaRotationFromOriginal.x = -maxChangeRotationX;

        Vector3 nextRotationValues = new Vector3(originalCameraRotation.x + deltaRotationFromOriginal.x, originalCameraRotation.y + deltaRotationFromOriginal.y, 0);
        playerCamera.gameObject.transform.eulerAngles = nextRotationValues;
    }
}
