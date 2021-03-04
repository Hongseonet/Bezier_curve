using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    [SerializeField] Bezier bezier;
    [SerializeField] Text txtIndicator;


    // Start is called before the first frame update
    void Start()
    {
        if (bezier == null)
            Debug.LogError("bezior not defined");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            bezier.ResetStatus();
        else if(Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
            bezier.SetLineType(1);
        else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
            bezier.SetLineType(2);
        else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
            bezier.SetLineType(3);
    }

    public void SetIndicator(int idx)
    {
        string[] txt = txtIndicator.text.Split('\n');

        for(int i=0; i<txt.Length; i++)
        {
            if (txt[i].Contains("<b>"))
            {
                txt[i] = txt[i].Replace("<b><i>", "");
                txt[i] = txt[i].Replace("</i></b>", "");
            }
        }
        txt[idx-1] = "<b><i>" + txt[idx-1] + "</i></b>";

        txtIndicator.text = "";
        for (int i=0; i<txt.Length; i++)
            txtIndicator.text += txt[i] + "\n";
    }
}
