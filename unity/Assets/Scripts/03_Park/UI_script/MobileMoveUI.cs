using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileMoveUI : MonoBehaviour
{
    PlayerControl pc;
    enum Dir { up,down,left,right}
    public PointObject[] pointObject;
    // Start is called before the first frame update
    void Start()
    {
    }
    public void SetPlayer(PlayerControl pc)
    {
        gameObject.SetActive(true);
        this.pc = pc;
        pointObject[(int)Dir.up].OnPointDown += Up;
        pointObject[(int)Dir.down].OnPointDown += Down;
        pointObject[(int)Dir.left].OnPointDown += Left;
        pointObject[(int)Dir.right].OnPointDown += Right;

        pointObject[(int)Dir.up].OnPointUp += StopUp;
        pointObject[(int)Dir.down].OnPointUp += StopDown;
        pointObject[(int)Dir.left].OnPointUp += StopLeft;
        pointObject[(int)Dir.right].OnPointUp += StopRight;
    }
    void Up()
    {
        pc.dir = new Vector2(pc.dir.x, 1);
    }
    void StopUp()
    {
        pc.dir = new Vector2(pc.dir.x, 0);
    }
    void Down()
    {
        pc.dir = new Vector2(pc.dir.x, -1);
    }
    void StopDown()
    {
        pc.dir = new Vector2(pc.dir.x, 0);
    }
    void Left()
    {
        pc.dir = new Vector2(-1, pc.dir.y);
    }
    void StopLeft()
    {
        pc.dir = new Vector2(0, pc.dir.y);
    }
    void Right()
    {
        pc.dir = new Vector2(1, pc.dir.y);
    }
    void StopRight()
    {
        pc.dir = new Vector2(0, pc.dir.y);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
