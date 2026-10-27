
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Transform playerCamera;
    public float moveSpeed = 5f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    private CharacterController controller;
    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Keyboard.current == null || playerCamera == null)
            return;

        float x = 0f;
        float z = 0f;

        // A/D ja vasen/oikea nuoli
        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
            x -= 1f;

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
            x += 1f;

        // W/S ja ylös/alas nuoli
        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed)
            z += 1f;

        if (Keyboard.current.sKey.isPressed ||
            Keyboard.current.downArrowKey.isPressed)
            z -= 1f;

        // Liikkuminen kameran vaakasuuntaisen suunnan mukaan
        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = (forward * z + right * x).normalized;

        // Painovoima
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        // Hyppy välilyönnillä
        if (controller.isGrounded &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        Vector3 velocity = move * moveSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}
