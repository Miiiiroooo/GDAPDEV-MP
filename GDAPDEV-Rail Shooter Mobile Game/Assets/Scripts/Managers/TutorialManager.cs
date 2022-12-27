using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject continueToNextLevelMenu;

    [Header("Tutorial Texts")]
    public string[] tutorialTexts;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private int index = 0;

    [Header("Tutorial Bools")]
    public bool tapDone;
    public bool swipeDone;
    public bool pinchDone;
    public bool spreadDone;
    public bool shakeDone;
    public bool joystickDone;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        GestureManager.Instance.onTap += onTap;
        GestureManager.Instance.onSwipe += OnSwipe;
        GestureManager.Instance.onSpread += OnSpread;
        continueToNextLevelMenu.SetActive(false);

        StartCoroutine(ShowTutorialWindow());
    }

    private void onTap(object send, TapEventArgs args)
    {
        if (index == 0 && !tapDone)
        {
            Debug.Log("Tap Done");
            tapDone = true;
            index++;
            animator.SetTrigger("Exit");
            StartCoroutine(ShowTutorialWindow());
        }
    }

    private void OnSwipe(object send, SwipeEventArgs args)
    {
        if ((args.SwipeDirection == SwipeEventArgs.SwipeDirections.LEFT || args.SwipeDirection == SwipeEventArgs.SwipeDirections.RIGHT) && !swipeDone && index == 1)
        {
            swipeDone = true;
            index++;
            animator.SetTrigger("Exit");
            StartCoroutine(ShowTutorialWindow());
            StartCoroutine(ShakeDetected());
        }
        
    }

    IEnumerator ShakeDetected()
    {
        yield return new WaitForSeconds(5);

        if (index == 2 && !shakeDone)
        {
            shakeDone = true;
            index++;
            animator.SetTrigger("Exit");
            StartCoroutine(ShowTutorialWindow());
        }
    }

    private void OnSpread(object send, SpreadEventArgs args)
    {
        if (args.DistanceDelta > 0) //show
        {
            if (index == 4 && !spreadDone)
            {
                spreadDone = true;
                index++;
                animator.SetTrigger("Exit");
                StartCoroutine(ShowTutorialWindow());
                StartCoroutine(JoystickDetected());
            }
        }
        else if (args.DistanceDelta < 0) // hide
        {
            if (index == 3 && !pinchDone)
            {
                pinchDone = true;
                index++;
                animator.SetTrigger("Exit");
                StartCoroutine(ShowTutorialWindow());
            }
        }
    }

    IEnumerator JoystickDetected()
    {
        yield return new WaitForSeconds(5);

        if (index == 5 && !joystickDone)
        {
            joystickDone = true;
            animator.SetTrigger("Exit");
            continueToNextLevelMenu.SetActive(true);

        }
    }

    IEnumerator ShowTutorialWindow()
    {
        yield return new WaitForSeconds(2);
        tutorialText.text = tutorialTexts[index];
        animator.SetTrigger("Show");
    }
}
