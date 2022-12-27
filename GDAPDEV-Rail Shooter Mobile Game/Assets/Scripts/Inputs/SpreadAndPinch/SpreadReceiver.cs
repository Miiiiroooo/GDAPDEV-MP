using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadReceiver : MonoBehaviour
{
    private Transform playerTransform;
    private PlayerScript playerScript;
    private bool isHiding = false;

    void Start()
    {
        playerTransform = GameManager.Instance.GetPlayerTransform();
        playerScript = GameManager.Instance.GetPlayerScript();

        GestureManager.Instance.onSpread += OnSpread;   
    }

    private void OnDisable()
    {
        GestureManager.Instance.onSpread -= OnSpread;
    }

    private void OnSpread(object send, SpreadEventArgs args)
    {
        if (args.DistanceDelta > 0 && isHiding)
        {
            isHiding = !isHiding;

            playerTransform.position += new Vector3(0, 1, 0);
            playerScript.OnPlayerHide(isHiding);
        }
        else if (args.DistanceDelta < 0 && !isHiding)
        {
            isHiding = !isHiding;

            playerTransform.position += new Vector3(0, -1, 0);
            playerScript.OnPlayerHide(isHiding);
        }
    }
}
