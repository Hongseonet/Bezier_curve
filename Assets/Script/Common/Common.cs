using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common : MonoSingleton<Common>
{
    public static bool ISDEV, ISDRAWORTHO;


    public void ABCC()
    {
        
    }

    public void DebugPrint(char type, string grp, string msg)
    {
        if (ISDEV)
        {
            switch (type)
            {
                case 'w':
                    Debug.LogWarning(grp + " : " + msg);
                    break;
                case 'e':
                    Debug.LogError(grp + " : " + msg);
                    break;
                default:
                    Debug.Log(grp + " : " + msg);
                    break;
            }
        }
    }
}