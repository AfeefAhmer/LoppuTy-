using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SwingPose
{
    public Vector3 position;   
    public Vector3 rotation;   
}

[System.Serializable]
public class SwingVariation
{
    public string name;
    public SwingPose windup;
    public SwingPose followThrough;
    public float windupTime = 0.15f;
    public float swingTime = 0.12f;
    public float holdTime = 0.04f;
    public float recoverTime = 0.25f;
    public float damageMultiplier = 1f;
}

public class Bat : Item
{
    [Header("Damage")]
    public float damage = 35f;
    public float reach = 2f;          
    public float radius = 0.8f;       

    [Header("Combo")]
    public float comboWindow = 0.5f;    
    public float comboCooldown = 0.6f;  
    [Range(0f, 1f)] public float hitPoint = 0.5f;  

    public SwingVariation[] combo;

    int comboIndex;
    bool swinging;
    bool queued;           
    float nextSwingTime;
    float lastSwingEnd = -999f;

    Vector3 basePos;
    Quaternion baseRot;

    void Reset()
    {
        combo = DefaultCombo();
    }

    protected override void Awake()
    {
        base.Awake();
        if (combo == null || combo.Length == 0) combo = DefaultCombo();
    }

    static SwingVariation[] DefaultCombo()
    {
        return new SwingVariation[]
        {
            new SwingVariation
            {
                name = "Diagonal slash",
                windup = new SwingPose { position = new Vector3(0.15f, 0.1f, -0.15f), rotation = new Vector3(-25f, 20f, -45f) },
                followThrough = new SwingPose { position = new Vector3(-0.35f, -0.1f, 0.3f), rotation = new Vector3(40f, -45f, 60f) },
                windupTime = 0.15f, swingTime = 0.12f, holdTime = 0.03f, recoverTime = 0.2f,
                damageMultiplier = 1f
            },
            new SwingVariation
            {
                name = "Backhand",
                windup = new SwingPose { position = new Vector3(-0.3f, 0.05f, -0.1f), rotation = new Vector3(-20f, -30f, 50f) },
                followThrough = new SwingPose { position = new Vector3(0.3f, -0.1f, 0.3f), rotation = new Vector3(35f, 40f, -60f) },
                windupTime = 0.12f, swingTime = 0.1f, holdTime = 0.03f, recoverTime = 0.2f,
                damageMultiplier = 1.1f
            },
            new SwingVariation
            {
                name = "Overhead smash",
                windup = new SwingPose { position = new Vector3(0.05f, 0.35f, -0.2f), rotation = new Vector3(-70f, 0f, -10f) },
                followThrough = new SwingPose { position = new Vector3(0f, -0.3f, 0.4f), rotation = new Vector3(60f, 0f, 5f) },
                windupTime = 0.3f, swingTime = 0.12f, holdTime = 0.1f, recoverTime = 0.45f,
                damageMultiplier = 1.75f
            }
        };
    }

    public override void OnEquipped(Transform hand)
    {
        base.OnEquipped(hand);
        swinging = false;
        queued = false;
        comboIndex = 0;
    }

    public override void Use()
    {
        if (swinging)
        {
            queued = true;
            return;
        }

        if (Time.time < nextSwingTime) return;

        if (Time.time - lastSwingEnd > comboWindow) comboIndex = 0;

        StartCoroutine(Swing());
    }

    IEnumerator Swing()
    {
        swinging = true;
        queued = false;

        SwingVariation v = combo[comboIndex];
        basePos = heldPosition;
        baseRot = Quaternion.Euler(heldRotation);
        SwingPose rest = new SwingPose();

        for (float t = 0f; t < v.windupTime; t += Time.deltaTime)
        {
            float k = t / v.windupTime;
            ApplyPose(rest, v.windup, 1f - (1f - k) * (1f - k));
            yield return null;
        }
        ApplyPose(rest, v.windup, 1f);

        bool hasHit = false;
        for (float t = 0f; t < v.swingTime; t += Time.deltaTime)
        {
            float k = t / v.swingTime;
            ApplyPose(v.windup, v.followThrough, k * k);

            if (!hasHit && k >= hitPoint)
            {
                hasHit = true;
                DealDamage(damage * v.damageMultiplier);
            }
            yield return null;
        }
        ApplyPose(v.windup, v.followThrough, 1f);
        if (!hasHit) DealDamage(damage * v.damageMultiplier);

        yield return new WaitForSeconds(v.holdTime);

        for (float t = 0f; t < v.recoverTime; t += Time.deltaTime)
        {
            float k = Mathf.SmoothStep(0f, 1f, t / v.recoverTime);
            ApplyPose(v.followThrough, rest, k);
            yield return null;
        }
        ApplyPose(rest, rest, 1f);

        swinging = false;
        lastSwingEnd = Time.time;
        comboIndex++;

        if (comboIndex >= combo.Length)
        {
            comboIndex = 0;
            nextSwingTime = Time.time + comboCooldown;
            queued = false;
        }
        else if (queued)
        {
            StartCoroutine(Swing());
        }
    }

    void ApplyPose(SwingPose from, SwingPose to, float t)
    {
        Vector3 pos = Vector3.Lerp(from.position, to.position, t);
        Quaternion rot = Quaternion.Slerp(Quaternion.Euler(from.rotation), Quaternion.Euler(to.rotation), t);

        transform.localPosition = basePos + pos;
        transform.localRotation = rot * baseRot;
    }

    void DealDamage(float amount)
    {
        Camera cam = Camera.main;
        Vector3 center = cam.transform.position + cam.transform.forward * reach;

        var alreadyHit = new HashSet<IDamageable>();

        foreach (Collider c in Physics.OverlapSphere(center, radius))
        {
            var target = c.GetComponentInParent<IDamageable>();
            if (target == null || !alreadyHit.Add(target)) continue;

            target.TakeDamage(amount);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (Camera.main == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Camera.main.transform.position + Camera.main.transform.forward * reach, radius);
    }
}