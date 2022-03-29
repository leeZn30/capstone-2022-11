using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPartsSlot : MonoBehaviour
{
    [SerializeField]
    private int currentParts;
    private int partsNum;

    public string partsName;
    public int size;
    public int cur
    {
        get { return currentParts; }
        set { currentParts = value; LoadImage(); }
    }
    public Button nextBtn;
    public Button prevBtn;
    public Image slotImage;

    public delegate void ChangeImageHandler(int partNum, int idx, Sprite sprite);
    public event ChangeImageHandler OnChangeSlotImage;
    // Start is called before the first frame update
    void Start()
    {
        nextBtn.onClick.AddListener(OnClickNext);
        prevBtn.onClick.AddListener(OnClickPrev);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetNum(int num)
    {
        partsNum = num;
        
    }
    void LoadImage()
    {
        slotImage.sprite = Resources.Load<Sprite>("Image/Character/" + partsName + currentParts);
        OnChangeSlotImage(partsNum, currentParts, slotImage.sprite);
    }
    void OnClickNext()
    {
        currentParts = (currentParts + 1 )%size;
        LoadImage();
    }
    void OnClickPrev()
    {
        currentParts = (currentParts - 1 + size) % size;
        LoadImage();
    }
}
