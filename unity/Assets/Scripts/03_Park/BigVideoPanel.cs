using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigVideoPanel : MonoBehaviour
{
    [SerializeField] private GameObject shutButton;

    // Start is called before the first frame update
    void Start()
    {
        shutButton.GetComponent<Button>().onClick.AddListener(ScaleDownPanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScaleDownPanel()
    {
        this.gameObject.SetActive(false);
        FindObjectOfType<Canvas>().transform.Find("smallVideoPanel").gameObject.SetActive(true);
    }
}
