using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image image_Page1;
    [SerializeField] private Image image_Page2;

    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }
}
