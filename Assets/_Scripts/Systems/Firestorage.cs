using Firebase.Extensions;
using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Database
{
    public class FireStorage : MonoBehaviour
    {
        [SerializeField] private Material playerIcons;
        [SerializeField] private Material chooseIcons;

        public List<Sprite> sprites;

        public static FireStorage Instance;

        private void Awake()
        {
            Instance = this;
        }

        public void GetImage(string _unity_material, int _firestore_image)
        {
            // Get a reference to the storage service, using the default Firebase App
            FirebaseStorage storage = FirebaseStorage.DefaultInstance;

            StorageReference storageRef = storage.GetReference(_unity_material + "/" + _firestore_image + ".png");

            storageRef.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    Material materialToGet = _unity_material switch
                    {
                        "playerIcons" => playerIcons,
                        "chooseIcons" => chooseIcons,
                        _ => null,
                    };

                    StartCoroutine(DownloadImage(task.Result.ToString(), materialToGet));
                }
            });
        }

        public IEnumerator DownloadImage(string url, Material _materialToGet)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error downloading image: " + www.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

                _materialToGet.mainTexture = texture;
            }
        }
    }
}