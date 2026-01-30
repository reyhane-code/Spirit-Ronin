using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public AudioClip clickSound;

    public void LoadGameScene()
    {
        StartCoroutine(PlayClickSoundAndLoadScene());
    }

    private IEnumerator PlayClickSoundAndLoadScene()
    {
        AudioSource.PlayClipAtPoint(clickSound, transform.position);
        yield return new WaitForSeconds(clickSound.length);
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
