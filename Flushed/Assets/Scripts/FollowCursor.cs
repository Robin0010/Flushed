using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    public WeaponData currentWeapon;

    [SerializeField] GameObject launchPoint;

    bool isFacingRight;

    private void Update()
    {
        if (currentWeapon != null)
        {
            LookAtCursor();
        }
    }

    public void UpdateWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;

        Quaternion newWeaponRotation = transform.rotation;
        newWeaponRotation.eulerAngles = new Vector3(0,0,0);
        transform.rotation = newWeaponRotation;
        launchPoint.transform.rotation = newWeaponRotation;
    }

    public void FlipWeapon(bool facingRight)
    {
        if (facingRight)
        {
            isFacingRight = true;
        }
        else
        {
            isFacingRight = false;
        }
    }

    private void LookAtCursor() 
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 direction = mousePos - transform.position;

        direction = direction.normalized;

        float rot_z = 0;

        float rot_y = 0;

        float rot_x = 0;

        if (!isFacingRight)
        {
            rot_y = 180;
            rot_z = Mathf.Atan2(direction.y, -direction.x) * Mathf.Rad2Deg;
        }
        else
        {
            rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        if (currentWeapon.followCursor)
        {
            transform.rotation = Quaternion.Euler(rot_x, rot_y, rot_z);
        }

        launchPoint.transform.rotation = Quaternion.Euler(rot_x, rot_y, rot_z);
    }
}
