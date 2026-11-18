using System.Collections;
using UnityEngine;

// Left click = shoot (hold if Automatic is ticked), R = reload.
// Plays your shoot / reload animations and damages anything with IDamageable.
public class Gun : Item
{
    [Header("Shooting")]
    public float damage = 20f;
    public float range = 100f;
    public float fireRate = 0.15f;      // seconds between shots
    public bool automatic = false;      // hold click to keep firing

    [Header("Ammo")]
    public int magazineSize = 12;
    public int reserveAmmo = 48;
    public bool infiniteReserve = false;
    public bool autoReloadWhenEmpty = true;
    public float reloadTime = 0f;       // 0 = use the length of your reload animation clip

    [Header("Animations")]
    public Animator animator;           // leave empty to find it on the gun automatically
    public bool useTriggers = true;     // true = Animator triggers, false = play states by name
    public string shootAnim = "Shoot";  // trigger name (or state name)
    public string reloadAnim = "Reload";

    [Header("Effects (optional)")]
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;

    [Header("Debug")]
    public bool debugHits = true;       // prints what each shot hit in the Console

    int ammo;
    bool reloading;
    bool held;
    float nextShotTime;

    public int Ammo { get { return ammo; } }

    protected override void Awake()
    {
        base.Awake();
        ammo = magazineSize;

        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public override void OnEquipped(Transform hand)
    {
        base.OnEquipped(hand);
        held = true;
        reloading = false;
    }

    public override void OnUnequipped()
    {
        held = false;
        base.OnUnequipped();
    }

    public override void OnDropped(Vector3 position, Vector3 force)
    {
        held = false;
        reloading = false;
        base.OnDropped(position, force);
    }

    // PlayerHand calls this on left click. The gun reads its own input
    // (so it can support holding the button), so this does nothing.
    public override void Use() { }

    void Update()
    {
        if (!held || reloading) return;

        // Empty: reload by itself
        if (ammo <= 0 && autoReloadWhenEmpty && Time.time >= nextShotTime)
        {
            TryReload();
            return;
        }

        bool fire = automatic ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
        if (fire) TryShoot();

        if (Input.GetKeyDown(KeyCode.R)) TryReload();
    }

    // ---------- Shooting ----------

    void TryShoot()
    {
        if (Time.time < nextShotTime) return;

        if (ammo <= 0)
        {
            if (Input.GetMouseButtonDown(0)) PlaySound(emptySound);
            return;
        }

        nextShotTime = Time.time + fireRate;
        ammo--;

        PlayAnim(shootAnim);
        if (muzzleFlash != null) muzzleFlash.Play();
        PlaySound(shootSound);

        FireRay();
    }

    void FireRay()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Gun: no camera tagged MainCamera found.");
            return;
        }

        // Shoots from the middle of the screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (!Physics.Raycast(ray, out RaycastHit hit, range, ~0, QueryTriggerInteraction.Ignore))
        {
            if (debugHits) Debug.Log("Gun: shot hit nothing.");
            return;
        }

        Debug.DrawLine(ray.origin, hit.point, Color.red, 0.3f);

        IDamageable target = hit.collider.GetComponentInParent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);
            if (debugHits) Debug.Log("Gun: hit " + hit.collider.name + " for " + damage + " damage.");
        }
        else if (debugHits)
        {
            Debug.Log("Gun: hit " + hit.collider.name + " but it has no IDamageable script (add one to your enemy).");
        }
    }

    // ---------- Reloading ----------

    void TryReload()
    {
        if (reloading) return;
        if (ammo >= magazineSize) return;
        if (!infiniteReserve && reserveAmmo <= 0) return;

        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        reloading = true;
        PlayAnim(reloadAnim);
        PlaySound(reloadSound);

        float time = reloadTime > 0f ? reloadTime : ClipLength(reloadAnim);
        if (time <= 0f) time = 1.5f;
        yield return new WaitForSeconds(time);

        int needed = magazineSize - ammo;
        int taken = infiniteReserve ? needed : Mathf.Min(needed, reserveAmmo);
        ammo += taken;
        if (!infiniteReserve) reserveAmmo -= taken;

        reloading = false;
    }

    // ---------- Animation / sound helpers ----------

    void PlayAnim(string name)
    {
        if (animator == null || string.IsNullOrEmpty(name)) return;

        if (useTriggers)
        {
            if (!HasTrigger(name))
            {
                Debug.LogWarning("Gun: the Animator has no trigger parameter called '" + name + "'. Add one, or untick Use Triggers.");
                return;
            }
            animator.ResetTrigger(name);
            animator.SetTrigger(name);
        }
        else
        {
            animator.Play(name, 0, 0f);
        }
    }

    bool HasTrigger(string name)
    {
        foreach (AnimatorControllerParameter p in animator.parameters)
        {
            if (p.name == name && p.type == AnimatorControllerParameterType.Trigger) return true;
        }
        return false;
    }

    float ClipLength(string name)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return 0f;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.ToLower().Contains(name.ToLower())) return clip.length;
        }
        return 0f;
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
    }
}