using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameSystem;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image image_Page1;
    [SerializeField] private Image image_Page2;


    private void Start()
    {
        if (CurrentScene == Scene.Menu) return;
        LoadScene(Scene.Menu);
    }

    public void StartGame()
    {
        LoadScene(Scene.Main);
    }

    public void StartMaker()
    {
        LoadScene(Scene.Maker);
    }
}
