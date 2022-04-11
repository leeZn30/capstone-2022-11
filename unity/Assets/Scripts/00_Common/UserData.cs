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

    public void SetUser(string id,string email,string nickname,int character)
    {
        this.id = id;
        this.email = email;
        this.nickname = nickname;
        this.character = character;
    }
    public void Clear()
    {
        this.id = "";
        this.email = "";
        this.nickname = "";
        this.character = 0;
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
