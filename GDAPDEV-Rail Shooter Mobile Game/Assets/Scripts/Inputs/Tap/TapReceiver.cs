using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapReceiver : MonoBehaviour
{
    private PlayerScript playerScript;

    // Start is called before the first frame update
    void Start()
    {
        GestureManager.Instance.onTap += onTap;

        playerScript = GameManager.Instance.GetPlayerScript();
    }

    void OnDisable()
    {
        GestureManager.Instance.onTap -= onTap;
    }

    private void onTap(object send, TapEventArgs args)
    {
        BasicWeaponTemplate currentWeapon = playerScript.GetCurrentWeapon();

        if (currentWeapon.GetCurrentAmmoCount() > 0)
        {
            currentWeapon.ReduceCurrentAmmo(1);
            playerScript.PlayerShotsAudio.Play();
        }
        else
        {
            playerScript.NoAmmoAudio.Play();
        }
    }
}
