using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;//정규표현식
using LitJson;
using UnityEngine.Networking;


public class inputObject : MonoBehaviour
{



    public int type;//0:중복체크   1:regex 체크(no 버튼)   2:regex체크(버튼 o)   3: toggle 선택
    public string key;
    public string name_content;
    public string btn_content;
    public string under_content;
    public string regex_str;
    public string[] strs;

    public TextMeshProUGUI nameText;
    public Button btn;
    public TMP_InputField inputField;

    public TextMeshProUGUI underText;


    public inputObject sub_obj;

    private Regex regex;

    public bool isOkay = false;

    public delegate void OkayHandler(string str);
    public event OkayHandler OnClickButton_;

    private TextMeshProUGUI btn_text;
    private Toggle[] toggles;
    private TextMeshProUGUI[] toggleTexts;
    // Start is called before the first frame update
    void Awake()
    {

        
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(delegate { checkRegex(); });
            

            if (btn_content == "")
            {
                btn.gameObject.SetActive(false);
            }
            else
            {
                btn_text = (btn.GetComponentInChildren<TextMeshProUGUI>());
                btn_text.text = btn_content;
                btn.onClick.AddListener(onClickButton);
            }
        }
        if (key=="fav")
        {
            
            toggles = GetComponentsInChildren<Toggle>();
            toggleTexts = new TextMeshProUGUI[toggles.Length];
            for (int i=0; i<toggles.Length; i++)
            {
                toggleTexts[i] = toggles[i].GetComponentInChildren<TextMeshProUGUI>();
                toggleTexts[i].text = GlobalData.Genre[i + 1];
                toggles[i].isOn = false;
            }

            isOkay = true;
        }

        nameText.text = name_content;


    }
    public void setRegex_str(string str)
    {
        regex_str = str;
        regex = new Regex(regex_str);
    }
    public void reset()
    {
  
        if (key=="password2" || key=="email2" )
        {
            setRegex_str("");
        }
        else
        {
            setRegex_str(regex_str);
        }

        if (inputField != null)
        {
            inputField.text = "";
            checkRegex();
        }

        isOkay = false;

        if (toggles != null)
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].isOn = false;
            }
            isOkay = true;
        }
        

    }
    void changeUnderTextColor()
    {//완료 상황에 따라 색 바꾸는 함수
        if (isOkay)
        { 
            underText.text = "완료";
            underText.color = new Color(0f, 255f, 0f);
        }
        else
        {
            
            underText.color = new Color(255f, 0f, 0f);
        }

    }
    bool checkRegex()
    {
        isOkay = false;
        //조건을 확인하고 알맞은 메시지를 보여주도록 하는 함수
        //지금은 임시로 기본 메시지가 나오도록 함
        if (inputField != null)
        {
            if (key == "email")
            {
                btn_text.text = btn_content;
            }
            if (regex.IsMatch(inputField.text))
            {
                if (underText != null)
                {
                    underText.text = "";
                }
                if (type == 1 && regex_str!="")
                {
                    isOkay = true;

                    if (sub_obj != null)
                    {
                        sub_obj.setRegex_str(inputField.text);
                        sub_obj.checkRegex();
                    }
                    changeUnderTextColor();
                }
            }
            else
            {
                if (underText != null)
                {
                    underText.text = under_content;
                }
                changeUnderTextColor();

                return false;
            }
        }

        return true;
    }



    void onClickButton()
    {
        if (inputField != null && type!=2)
        {
            if (underText.text == under_content) return;
        }

        switch (type)
        {
                //0:중복체크   1:regex 체크(no 버튼)   2:regex체크(버튼 o)   3: toggle 선택
                case 0:

                    if (key == "email" && btn_text.text!=btn_content)//보내기 단계라면
                    {
                        EmailAuth();
                    }
                    else
                    {
                        StartCoroutine(GET_Check());
                    }
                    
                    return;

                case 2:
                    //확인 팝업띄우고 regex체크
                    if (regex_str != "")
                    {
                        if (checkRegex())
                        {
                            isOkay = true;
                        }
                    }
                    break;

                default:
                    break;

        }
        if (strs.Length > 0)
        {//결과 팝업이 뜨는 오브젝트라면
            OnClickButton_(isOkay ? strs[1] : strs[0]);
        }

        changeUnderTextColor();
    }
    private void EmailAuth()
    {
        if (key != "email") return;

        //이메일 전송하고
        var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var Charsarr = new char[6];
        var random = new System.Random();

        for (int i = 0; i < Charsarr.Length; i++)
        {
            Charsarr[i] = characters[random.Next(characters.Length)];
        }

        var resultString = new string(Charsarr);

        //전송
        StartCoroutine(POST_EmailCode(inputField.text, resultString));

        sub_obj.setRegex_str("^" + resultString + "$");
        sub_obj.checkRegex();

        
        
        
    }
    public List<string> GetPreferredGenres()
    {
        List<string> list = new List<string>();
        for (int i = 0; i<toggles.Length; i++)
        {
            if (toggles[i].isOn == true)
            {
                list.Add(toggleTexts[i].text);
            }
        }
        return list;
    }
    public string GetText()
    {
        return inputField.text;
    }
    IEnumerator GET_Check()
    {

        IdEmail check = new IdEmail();
        if (key == "id")
            check.id = inputField.text;
        else if (key == "email")
            check.email = inputField.text;

        string json = JsonUtility.ToJson(check);

        Debug.Log(json);
        using (UnityWebRequest www = UnityWebRequest.Get(GlobalData.url + "/user/check"))
        {

            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("accept", "text/plain");
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

            yield return www.SendWebRequest();

            if (www.error == null)
            {
                if (www.isDone)
                {
                    if (key == "email")
                    {                       
                        btn_text.text = "인증 코드";
                    }
                    else if (key == "id")
                    {
                        isOkay = true;
                        changeUnderTextColor();
                    }
                    OnClickButton_(strs[1]);
                }
                else
                {
                    isOkay = false;
                }                    
                
            }
            else
            {
                if (www.responseCode == 400) {//중복
                    isOkay = false;
                    if (key == "email") {
                        
                    }
                    else if (key == "id")
                    {
                        changeUnderTextColor();
                    }
                    OnClickButton_(strs[0]);
                }
                Debug.Log(www.error.ToString());
            }
        }



    }
    IEnumerator POST_EmailCode(string email, string key)
    {
        EmailKey emailKey = new EmailKey();
        emailKey.email = email;
        emailKey.key = key;
        string json = JsonUtility.ToJson(emailKey);
        using (UnityWebRequest request = UnityWebRequest.Post(GlobalData.url + "/auth/email", json))
        {// 보낼 주소와 데이터 입력
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();//응답 기다리기

            if (request.error == null)//가입 성공
            {
                isOkay = true;
                //보내기 성공

            }
            else//보내기 실패
            {
                isOkay = false;
                Debug.Log(request.error.ToString());

            }
            OnClickButton_(isOkay ? strs[3] : strs[2]);
            changeUnderTextColor();
        }

    }
    JsonData JsonToObject(string json)
    {
        return JsonMapper.ToObject(json);
    }
}
