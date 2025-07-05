using Services.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    private const string SceneName = "LoadScene";

    private void Awake()
    {
        InputProviderResolver.Init();
    }

    private void Start()
    {
        SceneManager.LoadScene(SceneName);
    }
}