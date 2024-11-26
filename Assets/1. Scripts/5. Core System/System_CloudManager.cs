using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using static GameSystem;

public class System_CloudManager : MonoBehaviour
{
    /*
    private static FirebaseStorage storage;

    private void Start()
    {
        storage = FirebaseStorage.DefaultInstance; 
    }

    public static void UploadData(string nameInCloud, string nameInLocalRepository)
    {
        // Create a storage reference from our storage service
        StorageReference storageRef = storage.RootReference.Child(nameInCloud);

        // File located on disk
        string localFile = Path.Combine(Application.persistentDataPath, nameInLocalRepository);

        // Upload the file to the path.
        storageRef.PutFileAsync(localFile)
            .ContinueWith((Task<StorageMetadata> task) => {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
                    // Uh-oh, an error occurred!
                }
                else
                {
                    // Metadata contains file metadata such as size, content-type, and download URL.
                    StorageMetadata metadata = task.Result;
                    string md5Hash = metadata.Md5Hash;
                    Debug.Log("Finished uploading...");
                    Debug.Log("md5 hash = " + md5Hash);
                }
            });
    }

    public static void DownloadData(string nameInCloud, string nameInLocalRepository)
    {
        StorageReference pathRef = storage.GetReference(nameInCloud);

        // Create local filesystem URL
        string localFile = Path.Combine(Application.persistentDataPath, nameInLocalRepository);

        // Download to the local filesystem
        pathRef.GetFileAsync(localFile).ContinueWithOnMainThread(task => {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("File downloaded.");
            }
        });
    }*/
}