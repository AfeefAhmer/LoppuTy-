
using UnityEngine;

public class KeypadVisibility : MonoBehaviour
{
    public GameObject keypadPanel;
    public float showDistance = 1f;

    private Transform player;
    private bool unlocked = false;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;

        keypadPanel.SetActive(false);
    }

    void Update()
    {
        if (player == null || keypadPanel == null || unlocked)
            return;

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        if (distance <= showDistance && !keypadPanel.activeSelf)
        {
            keypadPanel.SetActive(true);

            FirstPersonCamera.keypadOpen = true;
            FirstPersonCamera.UpdateCursor();
        }
    }

    public void Unlock()
    {
        unlocked = true;
        keypadPanel.SetActive(false);
    }
}
