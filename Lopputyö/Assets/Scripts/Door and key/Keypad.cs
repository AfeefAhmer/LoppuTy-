
using UnityEngine;
using TMPro;
using System.Collections;

public class Keypad : MonoBehaviour
{
    [SerializeField] private TMP_Text Ans;
    [SerializeField] private MoveObjectController door;
    [SerializeField] private GameObject keypadPanel;

    private string Answer = "6379";
    private bool unlocked = false;

    public void Number(int number)
    {
        if (unlocked) return;

        Ans.text += number.ToString();
    }

    public void Enter()
    {
        if (unlocked) return;

        if (Ans.text == Answer)
        {
            unlocked = true;

            door.OpenDoor();

            Ans.text = "CORRECT";

            FirstPersonCamera.keypadOpen = false;
            FirstPersonCamera.UpdateCursor();

            FindFirstObjectByType<KeypadVisibility>().Unlock();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(IncorrectMessage());
        }
    }

    IEnumerator IncorrectMessage()
    {
        Ans.text = "INCORRECT";

        yield return new WaitForSeconds(2f);

        Ans.text = "";
    }
}
