
using UnityEngine;
using TMPro;

public class Raycast : MonoBehaviour
{
    [Header("Raycast Features")]
    [SerializeField] private float raylenght = 5f;

    private Camera _camera;

    [Header("Crosshair")]
    [SerializeField] private TMP_Text crosshair;

    [Header("Input Key")]
    [SerializeField] private KeyCode InteractKey = KeyCode.T;

    private NoteController currentNote;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
            return;
        }

        // Don't raycast while note is open
        if (currentNote != null && currentNote.IsOpen)
        {
            if (crosshair != null)
                crosshair.text = "";

            return;
        }

        currentNote = null;

        Ray ray = _camera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        RaycastHit[] hits = Physics.RaycastAll(ray, raylenght);

        foreach (RaycastHit hit in hits)
        {
            NoteController note =
                hit.collider.GetComponentInParent<NoteController>();

            if (note != null)
            {
                currentNote = note;
                break;
            }
        }

        if (currentNote != null)
        {
            if (crosshair != null)
                crosshair.text = "T";

            if (Input.GetKeyDown(InteractKey))
                currentNote.ShowNote();
        }
        else
        {
            if (crosshair != null)
                crosshair.text = "+";
        }
    }
}
