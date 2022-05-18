using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class UIDCreator
{
    private static uint lettertonumber(char letter)
    {
        uint index = (uint) letter - 64;
        return index;
    }

    public static uint createUID(string userID)
    {
        uint uid = 0;
        string id_str = "";

        foreach (char c in userID)
        {
            if (!char.IsDigit(c))
            {
                id_str += lettertonumber(c).ToString();
            }
            else
            {
                id_str += char.GetNumericValue(c).ToString();
            }
        }

        while (id_str.Length >= 10)
        {
            string subString1 = id_str.Substring(0, id_str.Length / 2);
            string subString2 = id_str.Substring(id_str.Length / 2);

            ulong sub1 = ulong.Parse(subString1);
            ulong sub2 = ulong.Parse(subString2);

            ulong rest1 = sub1 % 10;
            ulong rest2 = sub2 % 10;

            sub1 /= 2;
            sub1 += rest1;

            sub2 /= 2;
            sub2 += rest2;

            //id_str = sub1.ToString() + rest1.ToString() + sub2.ToString() + rest2.ToString();
            id_str = sub1.ToString() + sub2.ToString();
        }

        Debug.Log("UID: " + id_str);
        uid = (uint)int.Parse(id_str);
        return uid;
    }
}
