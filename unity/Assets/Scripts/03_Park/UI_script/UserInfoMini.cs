using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UserInfoMini : MonoBehaviour
{

    public TextMeshProUGUI userNickNameText;
    public Button infoButton;

    private string nickname;
    private string id;
    private ParkFollow followPage;
    // Start is called before the first frame update
    void Awake()
    {
       
        transform.SetParent(GameObject.Find("Canvas").transform);
        RectTransform rect = GetComponent<RectTransform>();
        rect.localScale = new Vector3(1, 1, 1);
        rect.position = new Vector3(rect.position.x, rect.position.y, 0);
        infoButton.onClick.AddListener(delegate { CallUserProfile(id); });
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetUserNickName(string nickname, string id)
    {
        this.nickname = nickname;
        this.id = id;
        userNickNameText.text = nickname + "(" + id + ")";
    }
    public void SetFollowPage(ParkFollow pf)
    {
        followPage = pf;
    }
    void CallUserProfile(string _id)
    {
        if (followPage == null)
        {
            followPage = GameObject.FindObjectOfType<ParkFollow>();
        }
        followPage.Open(_id);
        Destroy(gameObject);
    }
}
