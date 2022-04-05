using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;	// UnityWebRequest사용을 위해서 적어준다.

[System.Serializable]
public class IdEmail
{
    public string id;
    public string email;
}
[System.Serializable]
public class User
{
    public string id;
    public string password;
    public string email;
    public string nickname;
    public int character;
}
public class Auth
{
    public string id;
    public string password;
}
public class Main : MonoBehaviour
{
    string url = "http://localhost:8080/api";

    public Button joinBtn;
    public Button loginBtn;
    public TMP_InputField id_input;
    public TMP_InputField password_input;
    public Join join;

    public GameObject wrong_obj;

    private Animator animator;


    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        joinBtn.onClick.AddListener(delegate { join.OpenJoinPanel(); });
        loginBtn.onClick.AddListener(OnClickLoginButton);
        wrong_obj.SetActive(false);

        join.OnClickJoinButton_ += PostJoin;
    }
    void PostJoin(User user)
    {
        StartCoroutine(Join_UnityWebRequestPOST(user));
    }
    void OnClickLoginButton()
    {//로그인 버튼이 눌렸을 때 
        if (id_input.text.Length <= 0 || password_input.text.Length <= 0)
        {
            //ㅠㅠ
        }
        else
        {
            StartCoroutine(Login_UnityWebRequestPOST());
        }
        
    }
    IEnumerator Login_UnityWebRequestPOST()
    {
        animator.SetBool("isLoading", true);
        Auth auth = new Auth//현재 inputfield에 작성된 값 클래스로 변환
        {
            id = id_input.text,
            password = password_input.text
        };
        Debug.Log("로그인 시도 : " + id_input.text + " " + password_input.text);


        string json = JsonUtility.ToJson(auth);
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/auth", json))
        {// 보낼 주소와 데이터 입력
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();//결과 응답이 올 때까지 기다리기

            animator.SetBool("isLoading", false);

            if (request.error == null)//로그인 성공
            {

                Debug.Log(request.downloadHandler.text);

                //아이디 저장
                UserData.Instance.id = auth.id;
                SceneManager.LoadScene("02_Lobby");
            }
            else//로그인 실패
            {
                Debug.Log(request.error.ToString());
                wrong_obj.SetActive(true);
            }
        }


    }
    IEnumerator Join_UnityWebRequestPOST(User user)
    {
        //join 로딩 애니메이션
        join.LoadingJoin();

        string json = JsonUtility.ToJson(user);
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/user", json))
        {// 보낼 주소와 데이터 입력
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();//응답 기다리기


            if (request.error == null)//가입 성공
            {

                Debug.Log(request.downloadHandler.text);
                //join 성공
                join.SuccessJoin();
            }
            else//로그인 실패
            {

                Debug.Log(request.error.ToString());
                join.FailJoin();
            }
        }


    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
