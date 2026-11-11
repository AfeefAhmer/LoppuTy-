
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class NoteController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode closeKey = KeyCode.T;

    [Header("Player")]
    [SerializeField] private FirstPersonCamera player;

    [Header("UI")]
    [SerializeField] private GameObject noteCanvas;
    [SerializeField] private TMP_Text noteTextAreaUI;

    [Header("Note Text")]
    [SerializeField] private string noteText;

    [Header("Events")]
    [SerializeField] private UnityEvent openEvent;

    private bool isOpen = false;

    public bool IsOpen => isOpen;

    private void Start()
    {
        noteCanvas.SetActive(false);
    }

    public void ShowNote()
    {
        if (isOpen) return;

        noteTextAreaUI.text = noteText;
        noteCanvas.SetActive(true);

        isOpen = true;

        if (player != null)
            player.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        openEvent?.Invoke();
    }

    private void Update()
    {
        if (isOpen && Input.GetKeyDown(closeKey))
        {
            CloseNote();
        }
    }

    private void CloseNote()
    {
        noteCanvas.SetActive(false);

        isOpen = false;

        if (player != null)
            player.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
