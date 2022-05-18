using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SpecificMusic
{
    popular,personalGenre,recent
}
public static class GlobalData
{
    public static readonly string[] Genre =
    {
        "없음","댄스","발라드","팝","랩/힙합","인디음악","록/메탈"
    };
    public static readonly string url = "http://localhost:8080"; //http://metabusking.c.cs.kookmin.ac.kr/api
}
