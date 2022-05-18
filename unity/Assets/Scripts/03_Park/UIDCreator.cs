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
        //임시
        return 0;


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
            char[] reverString = id_str.ToCharArray();
            System.Array.Reverse(reverString);
            string newString = new string(reverString);

            ulong tmp = Math.Max(ulong.Parse(id_str), ulong.Parse(newString)) - Math.Min(ulong.Parse(id_str), ulong.Parse(newString));

            id_str = tmp.ToString();
        }

        uid = (uint)int.Parse(id_str);
        return uid;
    }
}
