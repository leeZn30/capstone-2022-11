using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Join : MonoBehaviour
{
    public Button exitBtn;
    public Button joinBtn;
    public inputObject[] inputObjects;

    public TextMeshProUGUI resultPopup_text;
    private Animator animator;

    public delegate void JoinHandler(User user);
    public event JoinHandler OnClickJoinButton_;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        joinBtn.onClick.AddListener(OnClickJoinButton);
        exitBtn.onClick.AddListener(OnClickExitButton);
        for(int i=0; i<inputObjects.Length; i++)
        {
            inputObjects[i].OnClickButton_ += playAppearResultPopup;
        }
    }
    public void OpenJoinPanel()
    {

        for (int i = 0; i < inputObjects.Length; i++)
        {
            inputObjects[i].reset();
        }
        animator.SetBool("isOpen", true);

    }
    void OnClickJoinButton()
    {
        User user = new User();
        for (int i=0; i<inputObjects.Length; i++)
        {
            if (inputObjects[i].isOkay == false)
            {
                //잘못 입력한 값이 있을 경우
                playAppearResultPopup("모든 문항에 올바른 값을 입력해주세요.");
                return;
            }
            switch (inputObjects[i].key)
            {
                case "id":
                    user.id = inputObjects[i].GetText();
                    break;
                case "password":
                    user.password = inputObjects[i].GetText();
                    break;
                case "nickname":
                    user.nickname = inputObjects[i].GetText();
                    break;
                case "email":
                    user.email = inputObjects[i].GetText();
                    break;
                case "fav":
                    user.preferredGenres = inputObjects[i].GetPreferredGenres();
                    break;
            }
        }
        user.character = 0;
        //가입 요청을 보냄
        OnClickJoinButton_(user);


    }
    public void LoadingJoin()
    {//로딩 애니메이션을 띄움
        animator.SetTrigger("Join");
    }
    public void SuccessJoin()     
    {//로딩 애니메이션을 끝내고
        animator.SetTrigger("Success");
        //가입 완료 애니메이션을 띄움
        animator.SetBool("isOpen", false);
    }
    public void FailJoin()
    {//로딩 애니메이션을 끝내고
        animator.SetTrigger("Fail");

    }

    void OnClickExitButton()
    {
        animator.SetBool("isOpen", false);

    }
    void playAppearResultPopup(string str)
    {
        resultPopup_text.text = str;
        animator.SetTrigger("AppearResultPopup");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
