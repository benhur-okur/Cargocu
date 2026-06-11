using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    float baseSteeringVelocity = 300.0f;
    float baseRideVelocity = 20.0f;
    float baseBoostSpeed = 30.0f;
    float baseSlowSpeed = 15.0f;

    float steeringVelocity;
    float rideVelocity;
    float boostSpeed;
    float slowSpeed;

    // YENÝ: Kargo aðýrlýðýndan gelen yavaþlama çarpaný (Varsayýlan 1 = Normal hýz)
    float currentCargoPenalty = 1f;

    void Start()
    {
        SetSpeedMultiplier(1f);
    }

    void Update()
    {
        // YENÝ: currentCargoPenalty çarpanýný hýza ve dönüþe ekledik (Aðýr kargoyla dönmek de zorlaþýr)
        float steeringControl = Input.GetAxis("Horizontal") * steeringVelocity * currentCargoPenalty * Time.deltaTime;
        float rideControl = Input.GetAxis("Vertical") * rideVelocity * currentCargoPenalty * Time.deltaTime;

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

    public void SetSpeedMultiplier(float multiplier)
    {
        steeringVelocity = baseSteeringVelocity * multiplier;
        rideVelocity = baseRideVelocity * multiplier;
        boostSpeed = baseBoostSpeed * multiplier;
        slowSpeed = baseSlowSpeed * multiplier;
    }

    // YENÝ: Kargo alýndýðýnda Collision scripti burayý çaðýracak
    public void ApplyCargoWeight(float penalty)
    {
        currentCargoPenalty = penalty;
    }

    // YENÝ: Kargo teslim edildiðinde Collision scripti burayý çaðýracak (Hýz normale döner)
    public void RemoveCargoWeight()
    {
        currentCargoPenalty = 1f;
    }
}