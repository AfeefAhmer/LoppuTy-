using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public KeyCode pickupKey = KeyCode.E;
    public float pickupRadius = 2f;

    PlayerHand hand;

    void Awake()
    {
        hand = GetComponent<PlayerHand>();
        if (hand == null) hand = GetComponentInParent<PlayerHand>();
        if (hand == null) hand = GetComponentInChildren<PlayerHand>();
        if (hand == null)
            Debug.LogError("PlayerPickup: no PlayerHand found. Add PlayerHand to the player.", this);
    }

    void Update()
    {
        if (Input.GetKeyDown(pickupKey)) TryPickup();
    }

    void TryPickup()
    {
        if (hand == null) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRadius, ~0, QueryTriggerInteraction.Collide);

        Item closest = null;
        float closestDist = Mathf.Infinity;

        foreach (Collider hit in hits)
        {
            Item item = hit.GetComponentInParent<Item>();
            if (item == null) continue;

            float dist = Vector3.Distance(transform.position, item.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = item;
            }
        }

        if (closest == null)
        {
            Debug.Log("PlayerPickup: E pressed, " + hits.Length + " colliders nearby but none have an Item script.");
            return;
        }

        hand.AddItem(closest);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}