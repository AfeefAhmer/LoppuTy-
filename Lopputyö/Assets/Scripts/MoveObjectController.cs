
using UnityEngine;

public class MoveObjectController : MonoBehaviour
{
    public float reachRange = 3f;

    private Animator anim;
    private Camera fpsCam;
    private GameObject player;

    private const string animBoolName = "isOpen_Obj_";

    private bool playerEntered;

    private int rayLayerMask;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        fpsCam = Camera.main;

        if (fpsCam == null)
        {
            Debug.LogError("MainCamera puuttuu!");
        }

        anim = GetComponent<Animator>();

        if (anim == null)
        {
            Debug.LogError("Animator puuttuu ovesta!");
        }
        else
        {
            anim.enabled = true;
        }

        int layer = LayerMask.NameToLayer("InteractRaycast");

        if (layer == -1)
        {
            rayLayerMask = Physics.DefaultRaycastLayers;
        }
        else
        {
            rayLayerMask = 1 << layer;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEntered = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerEntered = false;
        }
    }

    void Update()
    {
        if (!playerEntered || fpsCam == null || anim == null)
        {
            return;
        }

        Ray ray = fpsCam.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, reachRange, rayLayerMask))
        {
            MoveableObject moveableObject =
                hit.collider.GetComponentInParent<MoveableObject>();

            if (moveableObject != null)
            {
                string currentAnimBool =
                    animBoolName + moveableObject.objectNumber;

                bool isOpen = anim.GetBool(currentAnimBool);

                if (Input.GetKeyDown(KeyCode.R))
                {
                    anim.enabled = true;
                    anim.SetBool(currentAnimBool, !isOpen);

                    Debug.Log("Oven tila vaihdettu: " + !isOpen);
                }
            }
        }
    }
}
