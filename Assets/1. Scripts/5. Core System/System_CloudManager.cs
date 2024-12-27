using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Threading.Tasks;
using static GameSystem;
using Firebase.Functions;

public class System_CloudManager : MonoBehaviour
{
    private FirebaseFirestore db;
    private FirebaseFunctions functions;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        db = FirebaseFirestore.DefaultInstance;
        functions = FirebaseFunctions.DefaultInstance;
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


    private string url_getDocumentByCollectionAndId = "https://us-central1-golfer-a4ebb.cloudfunctions.net/getDocumentByCollectionAndId";
    public async Task<string> DownloadStageDataAsnyc(string collectionName, string title)
    {
        collectionName = Uri.EscapeDataString(collectionName);
        title = Uri.EscapeDataString(title);

        string url = $"{url_getDocumentByCollectionAndId}?collectionName={collectionName}&documentId=Stage_{title}";

        try
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return request.downloadHandler.text;
                }
                else
                {
                    Debug.LogError($"Error: {request.error}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception occurred: {ex.Message}");
        }

        return null;
    }

    private string url_getDocumentByCollection = "https://us-central1-golfer-a4ebb.cloudfunctions.net/getAllDocumentsFromCollection";
    public async Task<string> DownloadStageDataAsnyc(string collectionName)
    {
        collectionName = Uri.EscapeDataString(collectionName);

        string url = $"{url_getDocumentByCollection}?collectionName={collectionName}";

        try
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return request.downloadHandler.text;
                }
                else
                {
                    Debug.LogError($"Error: {request.error}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception occurred: {ex.Message}");
        }

        return null;
    }

    public async Task<bool> DeleteStageDataAsnyc(string collectionName, string documentId)
    {
        // Data to send 
        var data = new Dictionary<string, object>()
        {
            { "collectionName", collectionName},
            { "documentId", documentId }
        };

        try
        {
            var deleteDocumentCallable = functions.GetHttpsCallable("deleteDocument");
            var result = await deleteDocumentCallable.CallAsync(data);

            return true;
        }
        catch (System.Exception ex)
        {
            // Log any errors from the callable function
            Debug.LogError("Error calling Firebase Function: " + ex.Message);

            return false;
        }
    }
}