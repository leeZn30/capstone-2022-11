using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User
{
    public string id;
    public string password;
    public string email;
    public string nickname;
    public int character;
    public int followNum;
    public int followerNum;
    public List<string> preferredGenres;
    public List<string> followIds;

    public void SetUser(string id,string email,string nickname,int character, List<string> preferredGenres=null, List<string> followIds = null)
    {
        this.id = id;
        this.email = email;
        this.nickname = nickname;
        this.character = character;
        this.preferredGenres = preferredGenres;
        this.followIds = followIds;


        if (preferredGenres == null)
            this.preferredGenres = new List<string>();
        if (followIds == null)
            this.followIds = new List<string>();
        
    }
    public void AddFollow(string id)
    {
        followIds.Add(id);
    }
    public void DelFollow(string id)
    {
        followIds.Remove(id);
    }
    public void Clear()
    {
        this.id = "";
        this.email = "";
        this.nickname = "";
        this.character = 0;
        this.preferredGenres.Clear();
        this.followIds.Clear();
    }
    public string GetName()
    {
        return nickname+"("+id+")";
    }
}
public class UserData : Singleton<UserData>
{

    public User user;
    public string Token;
    public string id
    {
        get { return user.id; }    // _data 반환
        set { user.id = value; }   // value 키워드 사용
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Clear()
    {
        Token = "";
        user.Clear();
    }
}
