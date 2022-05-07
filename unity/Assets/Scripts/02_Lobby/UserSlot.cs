using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class UserSlot : MonoBehaviour, IPointerDownHandler
{
    public User user;
    public FollowPage.FollowSystemType type;

    public TextMeshProUGUI userName;

    public Button button;//del 버튼, 추가버튼, 둘다 될 수 있음. searchslot 여부에 따라 판단

    public delegate void ClickHandler(UserSlot us);
    public event ClickHandler OnClickAddButton;
    public event ClickHandler OnClickDelButton;
    public event ClickHandler OnClickSlot;

    private bool isFollow;
    public bool Follow{
        get { return isFollow; }
        set 
        { 
            isFollow = value;
            OnOffButton(!isFollow);
        }
    }
    public void SetType(FollowPage.FollowSystemType type)
    {
        this.type = type;
        if (type == FollowPage.FollowSystemType.follower)
            button.gameObject.SetActive(false);

    }
    public void SetUser(User user)
    {
        this.user = user;
        userName.text = user.nickname + "(" + user.id + ")";
    }
    public User GetUser()
    {
        return user;
    }
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(ClickButton);
    }
    public void OnOffButton(bool on)
    {
        button.gameObject.SetActive(on);
    }
    void ClickButton()
    {
        if (type==FollowPage.FollowSystemType.searched)
        {   //검색된 슬롯이면
            
            if (isFollow==false)
            {//추가되어있지않으면 추가버튼 기능
                Debug.Log(user.id + " 팔로우");
                OnClickAddButton(this);
                Follow = true;
            }
            else
            {//추가되어있으면 취소버튼 기능
                Debug.Log(user.id + " 팔로우 취소");
                OnClickDelButton(this);
                Follow = false;

            }
        }
        else
        {   //내 팔로우, 팔로워 목록 슬롯이면
            //취소버튼 기능
            OnClickDelButton(this);
            Follow = false;

        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnClickSlot(this);
    }


}
