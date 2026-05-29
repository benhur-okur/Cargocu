using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.5f);
    }
}