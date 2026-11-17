using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Left click  = quick stab
// Right click = heavy backstab (flips to reverse grip, slams down)
// F           = inspect (front flip)
//
// Poses are in "hand space": position x = right, y = up, z = forward.
public class Knife : Item
{
    [Header("Hit Area")]
    public float reach = 1.6f;
    public float radius = 0.55f;
    [Range(0f, 1f)] public float hitPoint = 0.6f;   // how far through the strike the damage lands

    [Header("Stab (left click)")]
    public float stabDamage = 25f;
    public float stabCooldown = 0.35f;
    public Vector3 stabPullback = new Vector3(0.05f, -0.03f, -0.18f);  // where the hand pulls back to
    public Vector3 stabThrust = new Vector3(0f, 0.02f, 0.45f);         // where the hand thrusts to
     // hand position at the end of the slam

    [Header("Inspect (F)")]
    public KeyCode inspectKey = KeyCode.F;
    // Axis the knife spins around, in hand space.
    // (1,0,0) = end over end, (0,0,1) = like a propeller, (0,1,0) = spinning on its own length.
    public Vector3 flipAxis = Vector3.right;
    public bool reverseFlip = false;     // tick this if the inspect flip spins the wrong way
    public int flipCount = 1;            // 2 = double flip
    public float flipHeight = 0.28f;     // how high it's tossed
    public float flipTime = 0.55f;       // how long it's in the air
    public float flipWindback = 25f;     // little wind-up spin the opposite way before the flip

    bool busy;
    bool inspecting;
    float nextAttackTime;
    Coroutine current;

    Vector3 basePos;
    Quaternion baseRot;

    public override void OnEquipped(Transform hand)
    {
        base.OnEquipped(hand);
        // reset in case it was put away mid-animation
        busy = false;
        inspecting = false;
    }

    // ---------- Input ----------

    void Update()
    {
        // Only runs while the knife is in hand (it's hidden otherwise)
        if (Input.GetKeyDown(inspectKey) && !busy)
        {
            current = StartCoroutine(Inspect());
        }


    }

    // Called by PlayerHand on left click
    public override void Use()
    {
        CancelInspect();

        if (busy || Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + stabCooldown;

        current = StartCoroutine(Stab());
    }

  

    void CancelInspect()
    {
        if (!inspecting) return;

        if (current != null) StopCoroutine(current);
        ResetPose();
        busy = false;
        inspecting = false;
    }

    // ---------- Stab (left click) ----------

    IEnumerator Stab()
    {
        busy = true;
        CacheBase();

        // 1. Pull back
        yield return Tween(0.08f, k =>
        {
            float e = EaseOut(k);
            Pose(Vector3.Lerp(Vector3.zero, stabPullback, e), Quaternion.Euler(-5f * e, 0f, 0f));
        });

        // 2. Thrust forward fast
        bool hit = false;
        yield return Tween(0.06f, k =>
        {
            float e = k * k * k;
            Pose(Vector3.Lerp(stabPullback, stabThrust, e), Quaternion.Euler(Mathf.Lerp(-5f, -8f, e), 0f, 0f));

            if (!hit && k >= hitPoint)
            {
                hit = true;
                DealDamage(stabDamage);
            }
        });
        if (!hit) DealDamage(stabDamage);

        // 3. Brief pause for impact
        yield return new WaitForSeconds(0.05f);

        // 4. Recover
        yield return Tween(0.18f, k =>
        {
            float e = EaseSmooth(k);
            Pose(Vector3.Lerp(stabThrust, Vector3.zero, e), Quaternion.Euler(Mathf.Lerp(-8f, 0f, e), 0f, 0f));
        });

        ResetPose();
        busy = false;
    }

    // ---------- Heavy backstab (right click) ----------

    

    // ---------- Inspect (front flip) ----------

    IEnumerator Inspect()
    {
        busy = true;
        inspecting = true;
        CacheBase();

        float dir = reverseFlip ? -1f : 1f;
        Vector3 axis = flipAxis.sqrMagnitude < 0.001f ? Vector3.right : flipAxis.normalized;

        float startAngle = -dir * flipWindback;
        float endAngle = dir * 360f * Mathf.Max(1, flipCount);
        Vector3 posReady = new Vector3(0f, -0.06f, -0.02f);
        Quaternion rotReady = Quaternion.AngleAxis(startAngle, axis);

        // 1. Drop the hand and wind back, getting ready to flip
        yield return Tween(0.15f, k =>
        {
            float e = EaseSmooth(k);
            Pose(Vector3.Lerp(Vector3.zero, posReady, e), Quaternion.Slerp(Quaternion.identity, rotReady, e));
        });

        // 2. FLIP: toss it up in an arc while it spins
        yield return Tween(flipTime, k =>
        {
            float angle = Mathf.Lerp(startAngle, endAngle, k);
            float arc = 4f * flipHeight * k * (1f - k);   // parabola: up, then back down
            Vector3 pos = new Vector3(
                0f,
                Mathf.Lerp(posReady.y, 0f, k) + arc,
                Mathf.Lerp(posReady.z, 0.03f, k));
            Pose(pos, Quaternion.AngleAxis(angle, axis));
        });

        // 3. Catch: a small dip as it lands in the hand, then settle
        Vector3 caught = new Vector3(0f, 0f, 0.03f);
        Vector3 dipped = new Vector3(0f, -0.05f, 0f);
        yield return Tween(0.07f, k =>
        {
            Pose(Vector3.Lerp(caught, dipped, EaseOut(k)), Quaternion.identity);
        });
        yield return Tween(0.22f, k =>
        {
            Pose(Vector3.Lerp(dipped, Vector3.zero, EaseSmooth(k)), Quaternion.identity);
        });

        ResetPose();
        busy = false;
        inspecting = false;
    }

    // ---------- Helpers ----------

    // Runs step(0..1) over the given time
    IEnumerator Tween(float duration, System.Action<float> step)
    {
        if (duration <= 0f)
        {
            step(1f);
            yield break;
        }

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            step(t / duration);
            yield return null;
        }
        step(1f);
    }

    static float EaseSmooth(float t) { return t * t * (3f - 2f * t); }
    static float EaseOut(float t) { return 1f - (1f - t) * (1f - t); }

    void CacheBase()
    {
        basePos = heldPosition;
        baseRot = Quaternion.Euler(heldRotation);
    }

    void Pose(Vector3 pos, Quaternion rot)
    {
        transform.localPosition = basePos + pos;
        transform.localRotation = rot * baseRot;
    }

    void ResetPose()
    {
        CacheBase();
        transform.localPosition = basePos;
        transform.localRotation = baseRot;
    }

    void DealDamage(float amount)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Knife: no camera tagged MainCamera found, so attacks can't hit anything.");
            return;
        }

        Vector3 center = cam.transform.position + cam.transform.forward * reach;

        // One hit per target, even if it has several colliders
        var alreadyHit = new HashSet<IDamageable>();

        foreach (Collider c in Physics.OverlapSphere(center, radius))
        {
            var target = c.GetComponentInParent<IDamageable>();
            if (target == null || !alreadyHit.Add(target)) continue;

            target.TakeDamage(amount);
        }
    }


   
}