using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;

public class OpenFileDialog : MonoBehaviour, IPointerDownHandler
{
    [DllImport("__Internal")] private static extern void FocusFileUploader();

    public SavingSystem savingSystem;
    public TextMeshProUGUI urlDisplayText;

    public void OnPointerDown(PointerEventData eventData)
    {
        FocusFileUploader();
    }

    public void FileDialogResult(string fileUrl)
    {
        if (urlDisplayText != null) { urlDisplayText.text = fileUrl; }
        StartCoroutine(PreviewCoroutine(fileUrl));
    }

    IEnumerator PreviewCoroutine(string url)
    {
        var www = new WWW(url);
        yield return www;
        string contentOfFile = www.text;

        savingSystem.DisplayFileContent(contentOfFile);
    }
}