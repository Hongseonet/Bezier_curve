using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common : MonoSingleton<Common>
{
    public static bool ISDEV, ISDRAWORTHO;


    public void ABCC()
    {
        
    }

    public void PrintLog(char type, string msg1, string msg2)
    {
        string str = msg1 + " : " + msg2;

        if (ISDEV)
        {
            switch (type)
            {
                case 'w':
                    Debug.LogWarning(str);
                    break;
                case 'e':
                    Debug.LogError(str);
                    break;
                default:
                    Debug.Log(str);
                    break;
            }
        }
    }
}