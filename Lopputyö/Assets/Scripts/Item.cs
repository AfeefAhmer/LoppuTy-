    using UnityEngine;


public class Item : MonoBehaviour
{
    public string itemName = "Item";

    public Vector3 heldPosition = Vector3.zero;
    public Vector3 heldRotation = Vector3.zero;

    Rigidbody rb;
    Collider[] colliders;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public virtual void OnPickedUp()
    {
        SetPhysics(false);
        gameObject.SetActive(false); 
    }

    public virtual void OnEquipped(Transform hand)
    {
        gameObject.SetActive(true);
        transform.SetParent(hand);
        transform.localPosition = heldPosition;
        transform.localRotation = Quaternion.Euler(heldRotation);
    }

    public virtual void OnUnequipped()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnDropped(Vector3 position, Vector3 force)
    {
        transform.SetParent(null);
        transform.position = position;
        gameObject.SetActive(true);
        SetPhysics(true);
        if (rb != null) rb.AddForce(force, ForceMode.Impulse);
    }

    public virtual void Use() { }

    void SetPhysics(bool on)
    {
        if (rb != null) rb.isKinematic = !on;
        if (colliders == null) return;
        foreach (var c in colliders) c.enabled = on;
    }
}
