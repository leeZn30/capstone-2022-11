using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class FolderAddPage : Page
{
    public Button okayBtn;
    public TMP_InputField inputField;

    public delegate void FolderMakeHandler(string name);
    public event FolderMakeHandler OnMakeFolder;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Init()
    {
        if (isAlreadyInit == false)
        {
            isAlreadyInit = true;
            okayBtn.onClick.AddListener(MakeFolderAsync);
        }

    }
    public override void Reset()
    {
        inputField.text = "";
    }
    async void MakeFolderAsync()
    {
        if (inputField.text.Length <= 0 && inputField.text.Length > 10 ) return;

        StringList sl= await POST_MakeListAsync(inputField.text);
        if (sl != null)
        {
            UserData.Instance.user.listName = sl.stringList;   
            MusicController.Instance.SetOptions(UserData.Instance.user.listName);
            OnMakeFolder?.Invoke(inputField.text);
            Close();
        }
        else
        {
            //폴더 생성 실패
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
