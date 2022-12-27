using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeReceiver : MonoBehaviour
{
    private PlayerScript playerScript;

    void Start()
    {
        GestureManager.Instance.onSwipe += OnSwipe;

        playerScript = GameManager.Instance.GetPlayerScript();
    }

    void OnDisable()
    {
        GestureManager.Instance.onSwipe -= OnSwipe;
    }

    private void OnSwipe(object send, SwipeEventArgs args)
    {
        if (args.SwipeDirection == SwipeEventArgs.SwipeDirections.LEFT)
        {
            playerScript.SwitchWeapons(-1);
            GameUIManager.Instance.UpdateWeaponColorUI();
        }
        else if (args.SwipeDirection == SwipeEventArgs.SwipeDirections.RIGHT)
        {
            playerScript.SwitchWeapons(1);
            GameUIManager.Instance.UpdateWeaponColorUI();
        }
    }
}
