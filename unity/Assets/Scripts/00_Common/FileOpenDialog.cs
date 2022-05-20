
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if !UNITY_ANDROID
using System.Windows.Forms;
using Ookii.Dialogs;


public class FileOpenDialog : MonoBehaviour
{
    public enum Type
    {
        Music,Image
    }
    VistaOpenFileDialog OpenDialog;
    Stream openStream = null;
    // Start is called before the first frame update
    void Start()
    {
        OpenDialog = new VistaOpenFileDialog();
        //OpenDialog.FilterIndex = 3;
        OpenDialog.Title = "파일 탐색기";
        
    }
    public string FileOpen(Type type)
    {
        string fileName=null;
        switch (type)
        {
            case Type.Music:
                OpenDialog.Filter = "오디오 파일 (*.wav, *.mp3, *.ogg) | *.wav; *.mp3; *.ogg;";
                break;
            case Type.Image:
                OpenDialog.Filter = "이미지 파일 (*.jpg, *.png) | *.jpg; *.png;";
                break;
        }
        
        if (OpenDialog.ShowDialog() == DialogResult.OK)
        {
            if((openStream = OpenDialog.OpenFile()) != null)
            {
                openStream.Close();
                fileName= OpenDialog.FileName;
            }
        }
        return fileName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
#endif