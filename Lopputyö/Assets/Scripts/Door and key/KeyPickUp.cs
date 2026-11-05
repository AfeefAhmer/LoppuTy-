
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public KeyCode pickupKey = KeyCode.E;

    private PlayerHand hand;

    void Start()
    {
        hand = GetComponent<PlayerHand>();

        if (hand == null)
            hand = GetComponentInParent<PlayerHand>();
    }

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            TryPickupKey();
        }
    }

    void TryPickupKey()
    {
        if (hand == null) return;

        KeyItem[] keys = FindObjectsByType<KeyItem>(
            FindObjectsSortMode.None
        );

        foreach (KeyItem key in keys)
        {
            if (key.gameObject.activeInHierarchy &&
                key.CanPickup(transform.position))
            {
                hand.AddItem(key);
                Debug.Log("Avain poimittu!");
                return;
            }
        }
    }
}
