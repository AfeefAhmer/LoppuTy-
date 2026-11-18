
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float detectionRange = 10f;
    public float attackRange = 2f;

    [Header("Attack")]
    public int damage = 10;
    public float attackPreparationTime = 3f;
    public float attackCooldown = 5f;

    private Rigidbody rb;

    private bool preparingAttack;

    private float preparationTimer;
    private float cooldownTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance > detectionRange)
        {
            StopMovement();
            preparingAttack = false;
            return;
        }

        FacePlayer();

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.fixedDeltaTime;
            StopMovement();
            return;
        }

        if (distance <= attackRange)
        {
            StopMovement();

            if (!preparingAttack)
            {
                preparingAttack = true;
                preparationTimer = attackPreparationTime;
            }

            preparationTimer -= Time.fixedDeltaTime;

            if (preparationTimer <= 0)
            {
                Attack();
            }
        }
        else
        {
            preparingAttack = false;
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        direction.Normalize();

        Vector3 movement = direction * moveSpeed;

        rb.linearVelocity = new Vector3(
            movement.x,
            rb.linearVelocity.y,
            movement.z
        );
    }

    private void StopMovement()
    {
        rb.linearVelocity = new Vector3(
            0,
            rb.linearVelocity.y,
            0
        );
    }

    private void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void Attack()
    {
        PlayerHealth playerHealth =
            player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        preparingAttack = false;
        cooldownTimer = attackCooldown;
    }
}
