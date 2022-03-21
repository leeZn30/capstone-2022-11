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
        sprites = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeSprite(int characterId)
    {
        int tmp = 0;
        int idx = 0;
        for (int i = 0; i < sprites.Count; i++)
        {
            idx = sprites.Count - 1 - i;
            tmp = (characterId % (int)Mathf.Pow(10, (i + 1) * 2)) / (int)Mathf.Pow(10, i * 2);
            sprites[idx].sprite = Resources.Load<Sprite>("Image/Character/" + ((PartsName)idx).ToString() + tmp);
        }
    }
}
