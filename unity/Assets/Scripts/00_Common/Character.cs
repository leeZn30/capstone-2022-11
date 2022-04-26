using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Character : MonoBehaviour
{
    public List<SpriteRenderer> sprites;
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
        if (sprites.Count == 0)
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
        int tmp = 0;
        int idx = 0;
        for (int i = 0; i < sprites.Count; i++)
        {
            tmp = (characterId % (int)Mathf.Pow(10, (i + 1) * 2)) / (int)Mathf.Pow(10, i * 2);
            sprites[i].sprite = Resources.Load<Sprite>("Image/Character/" + sprites[i].gameObject.name + tmp);
            //Debug.Log(((PartsName)idx).ToString() + tmp);
        }
    }
}
