using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongPage : Page
{

    public ListPageInSongPage listPage;
    public SearchPageInSongPage searchPage;
    public AddPageInSongPage addPage;
    public Button searchBtn;
    public Button addBtn;

    public List<SongFolder> songFolderList;
    void Start()
    {
        Init();
    }
    override public void Init()
    {
        if (isAlreadyInit == false)
        {
            songFolderList = new List<SongFolder>(GetComponentsInChildren<SongFolder>());
            for (int i = 0; i < songFolderList.Count; i++)
            {
                songFolderList[i].OnClickButton_ += listPage.Open;
            }

            searchBtn.onClick.AddListener(searchPage.Open);
            addBtn.onClick.AddListener(addPage.Open);
            Debug.Log(gameObject.name + "open");
            isAlreadyInit = true;
        }
    }
    override public void Reset()
    {
        listPage.Close();
        searchPage.Close();
        addPage.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
