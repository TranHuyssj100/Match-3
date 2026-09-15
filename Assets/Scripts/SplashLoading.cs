using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashLoading : MonoBehaviour
{
    private float delayTime = 1f;
    private void Start()
    {
        StartCoroutine(nameof(LoadGame));
    }

    private IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(delayTime);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(Constants.SCENE_GAME);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }
}
