using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [ContextMenu("Reload")]
    public void ReloadScnene()
    {
        SceneManager.LoadScene(0);

    }
}
