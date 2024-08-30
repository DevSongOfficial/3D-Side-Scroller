using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ObjectSelectionButton : MonoBehaviour
{
    public Button Button { get; private set; }
    public PlaceableObject Prefab { get; private set; }

    [SerializeField] private TMP_Text text;

    private void Awake()
    {
        Button = GetComponent<Button>();
    }

    public void Initialize(PlaceableObject prefab)
    {
        Prefab = prefab;

        text.text = Prefab.DisplayName;
        Button.onClick.AddListener(Prefab.OnSelectObjectWhenPlacing);
    }
}