using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public AudioClip clickSound;


    public void LoadGameScene()
    {
        AudioSource.PlayClipAtPoint(clickSound, transform.position);
        SceneManager.LoadScene("MainScene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
