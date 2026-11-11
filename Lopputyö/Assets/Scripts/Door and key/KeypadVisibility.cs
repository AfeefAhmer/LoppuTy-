
using UnityEngine;

public class KeypadVisibility : MonoBehaviour
{
    public GameObject keypadPanel;
    public float showDistance = 1f;

    private Transform player;
    private Keypad keypad;

    private bool unlocked = false;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;

        keypad = keypadPanel.GetComponentInChildren<Keypad>(true);

        keypadPanel.SetActive(false);

        SetCursor(false);
    }

    void Update()
    {
        if (player == null || keypadPanel == null || unlocked)
            return;

        Vector3 playerPosition = player.position;
        Vector3 doorPosition = transform.position;

        // Lasketaan vaakasuora etäisyys
        playerPosition.y = 0f;
        doorPosition.y = 0f;

        float distance = Vector3.Distance(
            playerPosition,
            doorPosition
        );

        bool nearDoor = distance <= showDistance;

        if (nearDoor)
        {
            if (!keypadPanel.activeSelf)
            {
                keypadPanel.SetActive(true);
                SetCursor(true);
            }
        }
        else
        {
            if (keypadPanel.activeSelf)
            {
                keypadPanel.SetActive(false);

                if (keypad != null)
                    keypad.ResetCode();

                SetCursor(false);
            }
        }
    }

    public void Unlock()
    {
        unlocked = true;

        keypadPanel.SetActive(false);

        SetCursor(false);
    }

    void SetCursor(bool visible)
    {
        Cursor.visible = visible;

        Cursor.lockState = visible
            ? CursorLockMode.None
            : CursorLockMode.Locked;
    }
}
