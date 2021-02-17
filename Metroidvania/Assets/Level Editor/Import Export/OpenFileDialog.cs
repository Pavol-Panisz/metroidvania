﻿using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;
using System;

public class OpenFileDialog : MonoBehaviour, IPointerDownHandler
{
    [DllImport("__Internal")] private static extern void FocusFileUploader();

    public Importer importer;

    public ScreenFader screenFader;

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
}