using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    // YENÝ: Çarpan hesaplamalarýnda kullanýlacak orjinal (bozulmamýþ) deðerler
    float baseSteeringVelocity = 300.0f;
    float baseRideVelocity = 20.0f;
    float baseBoostSpeed = 30.0f;
    float baseSlowSpeed = 15.0f;

    // Anlýk olarak oyunda kullanýlan deðiþkenler
    float steeringVelocity;
    float rideVelocity;
    float boostSpeed;
    float slowSpeed;

    void Start()
    {
        // Oyun baþladýðýnda çarpaný 1 olarak kabul et ve hýzlarý ayarla
        SetSpeedMultiplier(1f);
    }

    void Update()
    {
        float steeringControl = Input.GetAxis("Horizontal") * steeringVelocity * Time.deltaTime;
        float rideControl = Input.GetAxis("Vertical") * rideVelocity * Time.deltaTime;

        transform.Rotate(0, 0, -steeringControl);
        transform.Translate(0, rideControl, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Boot")
        {
            rideVelocity = boostSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        rideVelocity = slowSpeed;
    }

    // YENÝ: GameManager tarafýndan vardiya atlandýðýnda çaðrýlacak fonksiyon
    public void SetSpeedMultiplier(float multiplier)
    {
        steeringVelocity = baseSteeringVelocity * multiplier;
        rideVelocity = baseRideVelocity * multiplier;
        boostSpeed = baseBoostSpeed * multiplier;
        slowSpeed = baseSlowSpeed * multiplier;
    }
}