using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    [SerializeField] GameObject[] houses;
    GameObject activeDeliveryPoint;
    int lastIndex = -1; // son seçilen ev indexini hatýrla

    void Start()
    {
        foreach (GameObject house in houses)
        {
            Transform dp = house.transform.Find("DeliveryPoint");
            if (dp != null)
            {
                dp.gameObject.SetActive(false);
                dp.tag = "Untagged";
            }
        }
    }

    public void SelectNewDelivery()
    {
        if (activeDeliveryPoint != null)
        {
            activeDeliveryPoint.SetActive(false);
            activeDeliveryPoint.tag = "Untagged";
            activeDeliveryPoint = null;
        }

        // son seçilenden farklý bir index seç
        int index;
        do
        {
            index = Random.Range(0, houses.Length);
        } while (index == lastIndex && houses.Length > 1);

        lastIndex = index;

        Transform deliveryPoint = houses[index].transform.Find("DeliveryPoint");
        if (deliveryPoint != null)
        {
            activeDeliveryPoint = deliveryPoint.gameObject;
            activeDeliveryPoint.SetActive(true);
            activeDeliveryPoint.tag = "Customer";
        }
    }

    public void HideDelivery()
    {
        if (activeDeliveryPoint != null)
        {
            activeDeliveryPoint.SetActive(false);
            activeDeliveryPoint.tag = "Untagged";
            activeDeliveryPoint = null;
        }
    }
}