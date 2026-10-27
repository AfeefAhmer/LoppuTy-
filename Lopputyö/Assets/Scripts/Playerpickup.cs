using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public KeyCode pickupKey = KeyCode.E;
    public float pickupRadius = 2f;
    public string itemTag = "Item";

    [Header("Inventory")]
    public List<string> inventory = new List<string>();

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            TryPickup();
        }
    }

    void TryPickup()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRadius);

        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag(itemTag)) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hit.gameObject;
            }
        }

        if (closest != null)
        {
            inventory.Add(closest.name);
            Debug.Log("Picked up: " + closest.name);
            Destroy(closest);
        }
    }

    // Shows the pickup range in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}