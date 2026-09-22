using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayWeapon : MonoBehaviour
{
    public GameObject weapon;

    [System.Serializable]
    public class SwaySetting
    {
        public bool weaponSway;
        public float swayAmount = 0.02f;
        public float maxSwayAmount = 0.06f;
        public float swaySmoothValue = 4.0f;
        public Vector3 initialSwayPosition;
    }
    [SerializeField]
    public SwaySetting swaySetting;

    private float movementX;
    private float movementY;
    private Vector3 finalSwayPosition;

    private void Update()
    {
        Sway();
    }

    private void Sway()
    {
        movementX = Input.GetAxis("Mouse X");
        movementY = Input.GetAxis("Mouse Y");

        if (swaySetting.weaponSway == true)
        {
            movementX *= -swaySetting.swayAmount;
            movementY *= -swaySetting.swayAmount;

            movementX = Mathf.Clamp
                (movementX, -swaySetting.maxSwayAmount, swaySetting.maxSwayAmount);
            movementY = Mathf.Clamp
                (movementY, -swaySetting.maxSwayAmount, swaySetting.maxSwayAmount);

            finalSwayPosition = new Vector3
               (movementX, movementY, 0);
            weapon.transform.localPosition = Vector3.Lerp
                (weapon.transform.localPosition, finalSwayPosition +
                    swaySetting.initialSwayPosition, Time.deltaTime * swaySetting.swaySmoothValue);
        }
    }
}
