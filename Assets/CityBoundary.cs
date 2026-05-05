using UnityEngine;

public class CityBoundary : MonoBehaviour
{
    [SerializeField] float minX = -25f;
    [SerializeField] float maxX = 25f;
    [SerializeField] float minY = -25f;
    [SerializeField] float maxY = 25f;

    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }
}