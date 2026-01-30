using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public AudioClip clickSound;
    public float soundDelay = 0.2f;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void LoadGameScene()
    {
        StartCoroutine(PlaySoundAndLoad("MainScene"));
    }

    public void LoadMenuScene()
    {
        StartCoroutine(PlaySoundAndLoad("MenuScene"));
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private IEnumerator PlaySoundAndLoad(string sceneName)
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);

        yield return new WaitForSeconds(soundDelay);

        SceneManager.LoadScene(sceneName);
    }
}
