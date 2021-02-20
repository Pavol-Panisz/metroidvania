using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;
using System;
using TMPro;

public class OpenFileDialog : MonoBehaviour/*, IPointerDownHandler*/
{
    [DllImport("__Internal")] private static extern void FocusFileUploader();

    [DllImport("__Internal")] private static extern void PasteHereWindow(string gettext);

    public Importer importer;

    public Transform panelTransform;
    private Vector3 openPosition;
    public Vector3 closePosition;

    public ScreenFader screenFader;

    public TextMeshProUGUI inputField;

    /*public void OnPointerDown(PointerEventData eventData)
    {
        //FocusFileUploader();

        // LEFT OFF
        FileDialogResult("blob:null/c9529ddf-0054-4e12-af9c-d67947db515e");
    }*/

    private void Awake()
    {
        openPosition = panelTransform.position;
    }

    private void Start()
    {
        Close();
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

        yield return new WaitForSeconds(1f);

        Debug.Log(contentOfFile);

        // export what you imported right away for debug
        //WebGLFileSaver.SaveFile(contentOfFile, "content_from_import.metlvl");

        try
        {
            importer.Import(contentOfFile);
        }
        catch (Exception e) {
            Debug.Log("exception: " + e.Message);
        }
    }

    public void Done()
    {
        //importer.Import(inputField.text);

    }

    public void Close()
    {
        inputField.text = "";
        //panelTransform.position = closePosition;
        panelTransform.gameObject.SetActive(false);
    }

    public void Open()
    {
        //panelTransform.position = openPosition;
        //panelTransform.gameObject.SetActive(true);
        try
        {
            PasteHereWindow("text"); // if built, this happens
        }
        catch // if not buil
        {
            //panelTransform.position = openPosition;
            panelTransform.gameObject.SetActive(true);
        }
    }

    public void ImportFromPasted(string str)
    {
        if (str != null) importer.Import(str);
    }
}