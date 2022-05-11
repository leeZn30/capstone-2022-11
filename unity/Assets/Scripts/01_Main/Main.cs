using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;	// UnityWebRequest사용을 위해서 적어준다.
using LitJson;

[System.Serializable]
public class IdEmail
{
    public string id;
    public string email;
}
[System.Serializable]
public class Auth
{
    public string id;
    public string password;
}
public class Main : MonoBehaviour
{
    private string url = GlobalData.url;

    public Button joinBtn;
    public Button loginBtn;
    public TMP_InputField id_input;
    public TMP_InputField password_input;
    public Join join;

    public GameObject wrong_obj;

    private Animator animator;
    private TextMeshProUGUI wrongText;

    IEnumerator timewaitter;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        joinBtn.onClick.AddListener(delegate { join.OpenJoinPanel(); });
        loginBtn.onClick.AddListener(OnClickLoginButton);
        wrong_obj.SetActive(false);
        wrongText = wrong_obj.GetComponentInChildren<TextMeshProUGUI>();
        join.OnClickJoinButton_ += PostJoin;
        timewaitter = waitTime(5);
        
    }
    void PostJoin(User user)
    {
        if (timewaitter != null)
        {
            StopCoroutine(timewaitter);
            StartCoroutine(timewaitter);
        }
        StartCoroutine(Join_UnityWebRequestPOST(user));
    }
    void PostEmailCode(string email, string code)
    {
        StartCoroutine(POST_EmailCode(email,code));
    }
    void OnClickLoginButton()
    {//로그인 버튼이 눌렸을 때 
        if (timewaitter != null)
        {
            StopCoroutine(timewaitter);
            StartCoroutine(timewaitter);
        }
        StartCoroutine(Login_UnityWebRequestPOST());     
        
    }
    IEnumerator waitTime(float value)
    {
        yield return new WaitForSeconds(value);
        StopAllCoroutines();
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


            if (timewaitter != null)
            {
                StopCoroutine(timewaitter);
            }
            animator.SetBool("isLoading", false);

            if (request.error == null)//로그인 성공
            {
                if (request.isDone)
                {   
                    string jsonResult = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);


                    JsonData jsonData = JsonToObject(jsonResult);
                    Debug.Log("결과 " + jsonData[1]);
                    User user = new User();
             
                    user.SetUser((string)(jsonData[1]["id"]),
                            (string)(jsonData[1]["email"]),
                            (string)(jsonData[1]["nickname"]),
                            (int)(jsonData[1]["character"])
                            );

                    UserData.Instance.Token = (string)jsonData[0];                    
                    UserData.Instance.user = user;
                }
                Debug.Log(request.downloadHandler.text);

                SceneManager.LoadScene("02_Lobby");
            }
            else//로그인 실패
            {
                if (request.responseCode == 400)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
                    JsonData jsonData = JsonToObject(jsonResult);
                    if (jsonData["msg"] != null)
                    {
                        wrongText.text = (string)jsonData["msg"];
                    }
                    wrong_obj.SetActive(true);
                }
                else if (request.responseCode == 0)
                {
                    wrongText.text = "서버와의 연결이 끊어졌습니다.";
                    wrong_obj.SetActive(true);
                }
                Debug.Log(request.error);
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
            if (timewaitter != null)
            {
                StopCoroutine(timewaitter);
            }

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
    
    IEnumerator POST_EmailCode(string email, string key)
    {
        EmailKey emailKey = new EmailKey();
        emailKey.email = email;
        emailKey.key = key;
        string json = JsonUtility.ToJson(emailKey);
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/auth/email", json))
        {// 보낼 주소와 데이터 입력
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();//응답 기다리기

            if (request.error == null)//가입 성공
            {

                //join 성공

            }
            else//로그인 실패
            {

                Debug.Log(request.error.ToString());

            }
        }


    }
    JsonData JsonToObject(string json)
    {
        return JsonMapper.ToObject(json);
    }

}
public class EmailKey
{
    public string email;
    public string key;
}