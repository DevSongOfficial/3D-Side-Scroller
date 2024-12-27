using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tutorial_Golfbag : GolfBag
{
    [SerializeField] private Image image_guide;
    [SerializeField] protected TMP_Text text_guide;

    [SerializeField] private GolfBall golfBall;
    [SerializeField] private GameObject invisibleWalls;

    private bool isDone;

    private void Start()
    {
        golfBall.gameObject.SetActive(false);
        invisibleWalls.SetActive(true);
        isDone = false;
    }

    private void LateUpdate()
    {
        image_guide.transform.localEulerAngles = -transform.eulerAngles;
    }

    protected override void OpenTheBag()
    {
        base.OpenTheBag();

        text_guide.text = "Press Q to switch club";

        if (isDone) image_guide.gameObject.SetActive(false);
    }

    protected override void CloseTheBag()
    {
        base.CloseTheBag();

        text_guide.text = "Press R to pick up the bag";
    }

    public override void OnPickedUp(Transform carryPoint)
    {
        base.OnPickedUp(carryPoint);

        text_guide.text = "Press R to drop it off";
    }

    public override void OnDroppedOff()
    {
        base.OnDroppedOff();

        text_guide.text = "Now take your club and hit the ball";
        golfBall.gameObject.SetActive(true);
        invisibleWalls.SetActive(false);
        isDone = true;
    }
}
