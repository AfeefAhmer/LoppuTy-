
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    

    public void PlayGame()
    {
        SceneManager.LoadScene("Level 1 Talo");
    }

   

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game closed");
    }
}
