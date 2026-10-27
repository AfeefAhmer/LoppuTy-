
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

        // Piilota pelaajan capsule kamerasta
        Renderer[] renderers = player.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            rend.enabled = false;
        }
    }

    void Update()
    {
        if (Mouse.current == null || player == null)
            return;

        float mouseX = Mouse.current.delta.x.ReadValue()
                       * mouseSensitivity;

        float mouseY = Mouse.current.delta.y.ReadValue()
                       * mouseSensitivity;

        // Hiiri ylös = katse ylös
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        // Käännä kameraa pystysuunnassa
        transform.localRotation =
            Quaternion.Euler(verticalRotation, 0f, 0f);

        // Käännä pelaajaa vaakasuunnassa
        player.Rotate(Vector3.up * mouseX);
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
