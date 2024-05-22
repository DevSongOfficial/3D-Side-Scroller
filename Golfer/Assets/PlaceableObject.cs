using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    // This function is called when player click a certain object selection button in the level editor
    public void SelectObject()
    {
        LevelEditorManager.TEST_OBJECT = GameObject.Instantiate(gameObject);
    }
    
    public void Place()
    {
    }    
}