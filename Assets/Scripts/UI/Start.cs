using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
    public string sceneNameToLoad; // The name of the scene you want to load (set in the Inspector).

    public void LoadTargetScene()
    {
        // Load the scene by its name.
        SceneManager.LoadScene(sceneNameToLoad);
    }
}
