
using UnityEngine;

public class KeyItem : Item
{
    public string keyID = "HouseKey";

    public float pickupDistance = 0f;

    public bool boxOpened = false;

    public override void OnPickedUp()
    {
        base.OnPickedUp();
    }

    public bool CanPickup(Vector3 playerPosition)
    {
        float distance = Vector3.Distance(
            playerPosition,
            transform.position
        );

        return boxOpened && distance <= pickupDistance;
    }
}
