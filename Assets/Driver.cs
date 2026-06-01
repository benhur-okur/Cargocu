using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{
    // speed ve velocity için base çarpan degerleri.
    float baseSteeringVelocity = 300.0f;
    float baseRideVelocity = 20.0f;
    float baseBoostSpeed = 30.0f;
    float baseSlowSpeed = 15.0f;

    // anlýk degerler
    float steeringVelocity;
    float rideVelocity;
    float boostSpeed;
    float slowSpeed;

    // çarpaný base deðerle çarparak anlýk deðeri hesaplýyoruz. burda AI'dan fikir ve yardým aldýk ve bu þekilde yaparak vardiya atlandýðýnda hýzlarýn artmasýný saðlýyoruz.

    void Start()
    {
        
        SetSpeedMultiplier(1f); // oyun ilk baþladýgýnda çarpan 1 olarak ayrlýyoruz
    }

    void Update() // her framede güncellenen fonksiyon bunun sayesinde kullanýcýnýninputlarýný çok rhaat þekilde kontrol edioz
    {
        // kullanýcýnýn arabayý kontrol etmesi için iputlarý alýp timedelta ile uyguladýmýz sistem
        float steeringControl = Input.GetAxis("Horizontal") * steeringVelocity * Time.deltaTime; 
        float rideControl = Input.GetAxis("Vertical") * rideVelocity * Time.deltaTime;

        transform.Rotate(0, 0, -steeringControl); // z ekseninde döndüroz çünkü 2d oyunumuz
        transform.Translate(0, rideControl, 0); // arabayý rideControle göre y eksieninde baktýgý yone dogru gitmesi için Translate iþle bu þekilde yaptýk
    }

    void OnTriggerEnter2D(Collider2D other) // bost ekledik ama daha boost tagli objeleri eklemdik -> future work TODOya eklemeiz lazým bunu
    {
        if (other.tag == "Boot")
        {
            rideVelocity = boostSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D other) // çarpýþma olunca (eve falan) hýzýn düþmesi için koydum bunu
    {
        rideVelocity = slowSpeed;
    }

    // yeni bölümler içinmhýzlarýn artmasý sistemini burda base degerler ile çarpanlarý çarparak yeni degerleri elde ediyoruz, gameManager burdaki fonksiyonu kullanýyor vardiya deðiþimleirndeki araba hýz deðiþimleri için.
    public void SetSpeedMultiplier(float multiplier)
    {
        steeringVelocity = baseSteeringVelocity * multiplier;
        rideVelocity = baseRideVelocity * multiplier;
        boostSpeed = baseBoostSpeed * multiplier;
        slowSpeed = baseSlowSpeed * multiplier;
    }
}