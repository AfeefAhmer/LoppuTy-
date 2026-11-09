
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 0.15f;

    private float verticalRotation = 0f;

    public static bool keypadOpen = false;

    void Start()
    {
        keypadOpen = false;
        UpdateCursor();

        if (player != null)
        {
            Renderer[] renderers = player.GetComponentsInChildren<Renderer>();

            foreach (Renderer rend in renderers)
            {
                rend.enabled = false;
            }
        }
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!keypadOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        if (Mouse.current == null || player == null)
            return;

        float mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity;
        float mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.localRotation =
            Quaternion.Euler(verticalRotation, 0f, 0f);

        player.Rotate(Vector3.up * mouseX);
    }

    public static void UpdateCursor()
    {
        if (keypadOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
