using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Character : MonoBehaviour
{
    public bool isUI;
    public int id;
    private List<SpriteRenderer> sprites;
    private List<Image> imgs;
    enum PartsName
    {
        face,body
    };
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        if (isUI)
        {
            if (imgs ==null)
            {
                imgs = new List<Image>(GetComponentsInChildren<Image>());
                imgs.RemoveAt(0);
            }
        }
        else if (sprites== null)
        {
            sprites = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
            sprites.RemoveAt(0);
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeSprite(int characterId)
    {
        Init();
        id = characterId;
        int tmp = 0;
        int idx = 0;
        if (isUI)
        {
            for (int i = 0; i < imgs.Count; i++)
            {
                tmp = (characterId % (int)Mathf.Pow(10, (i + 1) * 2)) / (int)Mathf.Pow(10, i * 2);
                imgs[i].sprite = Resources.Load<Sprite>("Image/Character/" + imgs[i].gameObject.name + tmp);
                //Debug.Log(((PartsName)idx).ToString() + tmp);
            }
        }
        else
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                tmp = (characterId % (int)Mathf.Pow(10, (i + 1) * 2)) / (int)Mathf.Pow(10, i * 2);
                sprites[i].sprite = Resources.Load<Sprite>("Image/Character/" + sprites[i].gameObject.name + tmp);
                //Debug.Log(((PartsName)idx).ToString() + tmp);
            }
        }

    }
}
