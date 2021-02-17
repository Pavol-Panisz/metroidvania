using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;

public class OpenFileDialog : MonoBehaviour, IPointerDownHandler
{
    [DllImport("__Internal")] private static extern void FocusFileUploader();

    public Importer importer;

    public void OnPointerDown(PointerEventData eventData)
    {
        FocusFileUploader();
    }

    public void FileDialogResult(string fileUrl)
    {
        StartCoroutine(PreviewCoroutine(fileUrl));
    }

    IEnumerator PreviewCoroutine(string url)
    {
        var www = new WWW(url);
        yield return www;
        string contentOfFile = www.text;

        importer.Import(contentOfFile);
    }
}