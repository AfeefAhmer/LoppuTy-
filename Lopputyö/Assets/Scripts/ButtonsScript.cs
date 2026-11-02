
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsScript: MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Level 1 Talo");
    }

    public void Options()
    {
        
    }

    public void Quit()
    {
        Application.Quit();
    }
}
