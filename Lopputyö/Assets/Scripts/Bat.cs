using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Item
{
    [Header("Damage")]
    public float damage = 35f;
    public float reach = 2f;          // how far in front of the camera the hit lands
    public float radius = 0.8f;       // size of the hit area
    public float swingCooldown = 0.6f;

    [Header("Swing Animation")]
    public float swingDuration = 0.3f;
    // Extra rotation added at the peak of the swing. Change the numbers if the bat swings the wrong way.
    public Vector3 swingRotation = new Vector3(70f, 0f, -45f);

    bool swinging;
    float nextSwingTime;

    public override void OnEquipped(Transform hand)
    {
        base.OnEquipped(hand);
        swinging = false; // in case it was put away mid-swing
    }

    // Called by PlayerHand on left click
    public override void Use()
    {
        if (swinging || Time.time < nextSwingTime) return;
        nextSwingTime = Time.time + swingCooldown;
        StartCoroutine(Swing());
    }

    IEnumerator Swing()
    {
        swinging = true;

        Quaternion start = Quaternion.Euler(heldRotation);
        Quaternion end = Quaternion.Euler(heldRotation + swingRotation);

        // Fast swing forward
        float forwardTime = swingDuration * 0.4f;
        for (float t = 0f; t < forwardTime; t += Time.deltaTime)
        {
            transform.localRotation = Quaternion.Slerp(start, end, t / forwardTime);
            yield return null;
        }
        transform.localRotation = end;

        DealDamage();

        // Slower return
        float backTime = swingDuration * 0.6f;
        for (float t = 0f; t < backTime; t += Time.deltaTime)
        {
            transform.localRotation = Quaternion.Slerp(end, start, t / backTime);
            yield return null;
        }
        transform.localRotation = start;

        swinging = false;
    }

    void DealDamage()
    {
        Camera cam = Camera.main;
        Vector3 center = cam.transform.position + cam.transform.forward * reach;

        // One hit per target, even if it has several colliders
        var alreadyHit = new HashSet<IDamageable>();

        foreach (Collider c in Physics.OverlapSphere(center, radius))
        {
            var target = c.GetComponentInParent<IDamageable>();
            if (target == null || !alreadyHit.Add(target)) continue;

            target.TakeDamage(damage);
        }
    }

    // Shows the hit area in the Scene view while the bat is selected
    void OnDrawGizmosSelected()
    {
        if (Camera.main == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Camera.main.transform.position + Camera.main.transform.forward * reach, radius);
    }
}