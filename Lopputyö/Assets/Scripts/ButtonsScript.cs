
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Level 1 Talo");
    }

    public void Options()
    {
        // Nothing yet
    }

    public void Quit()
    {
        Application.Quit();
    }
}
