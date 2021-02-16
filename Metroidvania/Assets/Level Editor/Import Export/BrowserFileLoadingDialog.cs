using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
public class BrowserFileLoadingDialog : MonoBehaviour
{
    [DllImport("__Internal")] private static extern void AddClickListenerForFileDialog();

    void Awake()
    {
        AddClickListenerForFileDialog();
    }

    public void FileDialogResult(string fileUrl)
    {
        Debug.Log(fileUrl);
        //UrlTextField.text = fileUrl;
        StartCoroutine(LoadBlob(fileUrl));
    }

    IEnumerator LoadBlob(string url)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        if (!webRequest.isNetworkError && !webRequest.isHttpError)
        {
            // Get text content like this:
            Debug.Log(webRequest.downloadHandler.text);

        }
    }
}