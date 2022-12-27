using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeDetector : MonoBehaviour
{
    private float totalShake;
    private float minChange = 2f;
    private bool isPaused = false;
    private PlayerScript playerScript;

    private void Start()
    {
        totalShake = 0.0f;
        playerScript = GameManager.Instance.GetPlayerScript();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused)
            return;

        float x = Input.acceleration.x;
        float y = Input.acceleration.y;
        if (Mathf.Abs(x) > minChange || Mathf.Abs(y) > minChange)
        {
            totalShake += Mathf.Abs(x);
            totalShake += Mathf.Abs(y);
        }
        else
        {
            if (totalShake > 35)
            {
                BasicWeaponTemplate currentWeapon = playerScript.GetCurrentWeapon();

                currentWeapon.Reload();
                playerScript.ReloadAudio.Play();
            }
            
            totalShake = 0;
        }
    }
}
