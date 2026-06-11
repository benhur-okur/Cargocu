using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    
    // kamera pozisyonunun arabayla entegre şekilde değişmsei için thingsToFollow değişkeni var bunu inspecterda arabayya atadık.
    [SerializeField] GameObject thingsToFollow;
    void LateUpdate() 
    {
        // kamera takip sistemi içi thingstoFollowun pozisyonunu alıp z ekseninde -10 yaptk 2d sistemde kamera arabaya görmesi için böle yaptık değiştirilebilir buraya tekrar kontrol et
        transform.position = thingsToFollow.transform.position + new Vector3 (0, 0, -10);
    }
}
