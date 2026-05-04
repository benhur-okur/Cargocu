using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    bool hasPacked; // değer atamamak false olarak başlatmaktır!!
    bool hasDelivered;

    [SerializeField] Color32 hasPackageColor = new Color32(1, 1, 1, 1);
    [SerializeField] Color32 noPackageColor = new Color32(1, 1, 1, 1);

    SpriteRenderer spriteRenderer;
    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void OnCollisionEnter2D(Collision2D other){

       // Debug.Log("Ohh..It Hurtsssss...!!!");
        
    }

    
   void OnTriggerEnter2D(Collider2D other) {
        
        if(other.tag == "Package" && !hasPacked){
            hasPacked = true;
            hasDelivered = false;
            Debug.Log("Package picked up!!!");
            spriteRenderer.color = hasPackageColor;
            Destroy(other.gameObject);
            
            
        }else if(other.tag == "Customer" && hasPacked && !hasDelivered){
            Debug.Log("Customer: Thanksss..");
            hasPacked = false;
            hasDelivered = true;
            spriteRenderer.color = noPackageColor;
        }
       
    }


}
