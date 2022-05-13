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
        "¾øÀ½","´í½º","¹ß¶óµå","ÆË","·¦/ÈüÇÕ","ÀÎµðÀ½¾Ç","·Ï/¸ÞÅ»"
    };
    public static readonly string url = "http://localhost:8080/api"; //http://metabusking.c.cs.kookmin.ac.kr/api
}
