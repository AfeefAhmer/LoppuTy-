
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Key Settings")]
    public string requiredKeyID = "HouseKey";

    [Header("Interaction")]
    public float interactionDistance = 0.1f;
    public KeyCode interactKey = KeyCode.R;

    [Header("Animation")]
    public string animBoolName = "isOpen_Obj_";

    private Transform player;
    private PlayerHand playerHand;

    private Animator anim;
    private MoveableObject moveableObject;

    private bool isOpen = false;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHand = playerObject.GetComponent<PlayerHand>();
        }

        anim = GetComponent<Animator>();

        moveableObject = GetComponent<MoveableObject>();

        if (anim == null)
            Debug.LogError("DoorController: Animator puuttuu!");

        if (moveableObject == null)
            Debug.LogError("DoorController: MoveableObject puuttuu!");
    }

    void Update()
    {
        if (player == null || playerHand == null ||
            anim == null || moveableObject == null)
            return;

        float distance = Vector3.Distance(
            player.position,
            transform.position
        );

        if (distance <= interactionDistance &&
            Input.GetKeyDown(interactKey))
        {
            if (!isOpen)
            {
                if (HasKey())
                {
                    OpenDoor();
                }
                else
                {
                    Debug.Log("Tarvitset avaimen!");
                }
            }
            else
            {
                CloseDoor();
            }
        }
    }

    bool HasKey()
    {
        foreach (Item item in playerHand.inventory)
        {
            KeyItem key = item as KeyItem;

            if (key != null && key.keyID == requiredKeyID)
            {
                return true;
            }
        }

        return false;
    }

    void OpenDoor()
    {
        string currentAnimBool =
            animBoolName + moveableObject.objectNumber;

        anim.enabled = true;
        anim.SetBool(currentAnimBool, true);

        isOpen = true;

        Debug.Log("Ovi avattu avaimella!");
    }

    void CloseDoor()
    {
        string currentAnimBool =
            animBoolName + moveableObject.objectNumber;

        anim.SetBool(currentAnimBool, false);

        isOpen = false;

        Debug.Log("Ovi suljettu!");
    }
}
