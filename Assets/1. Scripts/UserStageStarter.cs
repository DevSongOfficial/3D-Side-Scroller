using System.Collections;
using UnityEngine;
using static GameSystem;

public class UserStageStarter : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.GetUserStageData() != null)
            StartCoroutine(PlayUserCourseRoutine());
    }

    private IEnumerator PlayUserCourseRoutine()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        GameManager.SetupStage(JsonUtility.FromJson<StageDataHandler>(GameManager.GetUserStageData()["Map"].ToString()));

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
       
        LevelEditorManager.SetPlayMode(PlayMode.Playing);
    }
}