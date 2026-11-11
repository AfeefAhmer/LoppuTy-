
using UnityEngine;
using TMPro;
using System.Collections;

public class Keypad : MonoBehaviour
{
    [SerializeField] private TMP_Text Ans;
    [SerializeField] private MoveObjectController door;
    [SerializeField] private KeypadVisibility visibility;

    private string Answer = "6379";

    private bool unlocked = false;

    public void Number(int number)
    {
        if (unlocked)
            return;

        Ans.text += number.ToString();
    }

    public void Enter()
    {
        if (unlocked)
            return;

        if (Ans.text == Answer)
        {
            unlocked = true;

            door.OpenDoor();

            visibility.Unlock();
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

        yield return new WaitForSeconds(1f);

        Ans.text = "";
    }
    public void ResetCode()
    {
        StopAllCoroutines();
        Ans.text = "";
    }
}
