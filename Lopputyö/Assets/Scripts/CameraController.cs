
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 0.15f;

    private float verticalRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (player != null)
        {
            Renderer[] renderers =
                player.GetComponentsInChildren<Renderer>();

            foreach (Renderer rend in renderers)
            {
                rend.enabled = false;
            }
        }
    }

    void Update()
    {
<<<<<<< Updated upstream
        // Älä liikuta kameraa, kun keypad on käytössä
        if (Cursor.lockState != CursorLockMode.Locked)
            return;
=======
        if (Time.timeScale == 0f) return;
>>>>>>> Stashed changes

        if (Mouse.current == null || player == null)
            return;

        float mouseX = Mouse.current.delta.x.ReadValue()
                       * mouseSensitivity;

        float mouseY = Mouse.current.delta.y.ReadValue()
                       * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(
            verticalRotation, -90f, 90f);

        transform.localRotation =
            Quaternion.Euler(verticalRotation, 0f, 0f);

        player.Rotate(Vector3.up * mouseX);
    }
}
