using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Threading.Tasks;
using static GameSystem;

public class System_CloudManager : MonoBehaviour
{
    private FirebaseFirestore db;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        db = FirebaseFirestore.DefaultInstance; 
    }
    
    public async Task UploadStageDataAsync(string collectionName, string title, string description)
    {
        // Get map file from local disk.
        var mapData = SaveManager.LoadStageData(title);

        // Upload the file to the path with title and description.
        DocumentReference docRef = db.Collection(collectionName).Document($"{SaveManager.prefix}{title}");
        Dictionary<string, object> stageData = new Dictionary<string, object>
        {
            { "Title", title },
            { "Description", description},
            { "Map", JsonUtility.ToJson(mapData) }
        };

        await docRef.SetAsync(stageData).ContinueWithOnMainThread(task => {
            Debug.Log("<color=cyan>Upload Completed</color>");
        });

    }

    private string url_getRandomDocuments = "https://us-central1-golfer-a4ebb.cloudfunctions.net/getRandomDocument";
    public async Task<string> DownloadRandomStageDataAsync()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url_getRandomDocuments))
        {
            await WaitForRequest(request);

            if (request.result == UnityWebRequest.Result.Success)
                return request.downloadHandler.text;
        }

        return null;
    }

    private Task WaitForRequest(UnityWebRequest request)
    {
        var tcs = new TaskCompletionSource<bool>();


        request.SendWebRequest().completed += (op) => tcs.SetResult(true);

        return tcs.Task;
    }
}