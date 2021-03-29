using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawManager : MonoBehaviour
{
    [SerializeField] Text txtIndicator;
    [SerializeField] Bezier bezier;

    [HideInInspector]
    GameObject refBezier;

    double bezierIdx = -1; //current bezier index


    public double BezierIdx
    {
        set { bezierIdx = value; }
        get { return bezierIdx; }
    }

    public GameObject RefBezier
    {
        set { refBezier = value; }
        get { return refBezier; }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            bezier.ResetStatus();
        else if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
            bezier.SetLineType(1);
        else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
            bezier.SetLineType(2);
        else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
            bezier.SetLineType(3);

        else if(Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift))
        {
            if (refBezier == null)
                refBezier = Instantiate(bezier.gameObject, transform);
        }
    }

    public void SetIndicator(int idx)
    {
        string[] txt = txtIndicator.text.Split('\n');

        for (int i = 0; i < txt.Length; i++)
        {
            if (txt[i].Contains("<b>"))
            {
                txt[i] = txt[i].Replace("<b><i>", "");
                txt[i] = txt[i].Replace("</i></b>", "");
            }
        }
        txt[idx - 1] = "<b><i>" + txt[idx - 1] + "</i></b>";

        txtIndicator.text = "";
        for (int i = 0; i < txt.Length; i++)
            txtIndicator.text += txt[i] + "\n";
    }
}