using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        foreach (char c in userID)
        {
            if (!char.IsDigit(c))
            {
                uid += lettertonumber(c);
            }
            else
            {
                uid += (uint) char.GetNumericValue(c);
            }
        }

        return uid;
    }
}
