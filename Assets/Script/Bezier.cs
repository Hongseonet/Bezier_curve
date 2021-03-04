using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    enum lineTypeList { none, straight, curve, bezier };
    lineTypeList lineType;

    const int maxpoints = 10000;
    const int numpos = 20; //curved element : line renderer positions : 50
    int cntDraw = 0; //how many draw?
    int addWeight = 0; //for stack count
    public LineRenderer lineRenderer;
    public GameObject Linepoint;
    public GameObject ControlPoint;

    //display inspector for debug
    [SerializeField] List<GameObject> listPoint = new List<GameObject>();
    [SerializeField] List<GameObject> listControlPoint = new List<GameObject>();
    Dictionary<int, lineTypeList> dicPointType = new Dictionary<int, lineTypeList>();

    public Vector3 sizee;
    public Vector3 newPos, oldPos;
    private Vector3[] pos;

    Ray ray;
    RaycastHit hit;
    Camera cam;


    // Start is called before the first frame update
    void Start()
    {
        pos = new Vector3[maxpoints];
        lineType = lineTypeList.none;

        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 2f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //for debug
            foreach(KeyValuePair<int, lineTypeList> abc in dicPointType)
                Debug.Log("a : " + abc.Key + " / " + abc.Value);
        }
        else if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift))
        {
            if (lineType == lineTypeList.none)
                return;

            //newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //orthographic
            newPos = Camera.main.ScreenPointToRay(Input.mousePosition).GetPoint(Mathf.Abs(cam.transform.localPosition.z)); //perspective
            newPos.z = 0;

            Debug.LogWarning("dd : " + newPos);

            if(dicPointType.Count == 0) //head point
                dicPointType.Add(cntDraw, lineType);

            if (CreatePointMarker(newPos) > 1)
            {
                Debug.Log("create new point");
                
                //add lineType on hashmap
                cntDraw++;
                dicPointType.Add(cntDraw, lineType);
                //end

                switch (lineType)
                {
                    case lineTypeList.straight:
                        //DrawStraitghtCurve(oldPos, newPos);
                        break;
                    case lineTypeList.curve:
                        DrawSingleCurve(oldPos, newPos);
                        break;
                    case lineTypeList.bezier:
                        DrawBezierCurve(oldPos, newPos);
                        break;
                }

                if(lineType != lineTypeList.straight)
                    addWeight += numpos; //eg 0, 20, 40 ...
                else
                    addWeight += 2; //eg 2, 4, 6 ...
            }
            oldPos = newPos;
        }
    }

    private int CreatePointMarker(Vector3 pointPosition)
    {
        GameObject pm = Instantiate(Linepoint, pointPosition, Quaternion.identity, transform);
        listPoint.Add(pm);

        MovePointMarker mctrlpt = pm.GetComponent<MovePointMarker>();
        mctrlpt.pointID = listPoint.Count - 1;

        sizee = listPoint[listPoint.Count - 1].transform.position;

        return listPoint.Count;
    }

    private int AddControlPoint(Vector3 pointPosition)
    {
        GameObject cp = Instantiate(ControlPoint, pointPosition, Quaternion.identity, transform);
        listControlPoint.Add(cp);

        MoveCtrlPt mctrlpt = cp.GetComponent<MoveCtrlPt>();
        mctrlpt.pointID = listControlPoint.Count - 1;

        if (pointPosition == new Vector3(100, -100, 100))
            cp.SetActive(false);

        return listControlPoint.Count;
    }

    private int InsertControlPoint(Vector3 pointPosition, int id)
    {
        GameObject cp = Instantiate(ControlPoint, pointPosition, Quaternion.identity, transform);
        listControlPoint.Insert(id, cp);

        MoveCtrlPt mctrlpt = cp.GetComponent<MoveCtrlPt>();
        mctrlpt.pointID = id;

        return listControlPoint.Count;
    }

    public void MovePointMarker(int pointID, Vector3 pos)
    {
        //issue : don't move the head point
        oldPos = listPoint[listPoint.Count - 1].transform.position;
        lineType = dicPointType[pointID];
        //Debug.LogWarning("dd; " + lineType);

        //editable none type
        switch (lineType)
        {
            case lineTypeList.straight:
                if (pointID == 0)
                    EditStraitghtCurve(pos, listPoint[pointID + 1].transform.position);
                else
                    EditStraitghtCurve(listPoint[pointID - 1].transform.position, pos);
                break;
            case lineTypeList.curve:
                if (pointID == 0) //start point
                    EditSingleCurve(pos, listPoint[pointID + 1].transform.position, pointID, listControlPoint[pointID].transform.position);
                else if(pointID == (listPoint.Count - 1)) //end point
                    EditSingleCurve(listPoint[pointID - 1].transform.position, pos, pointID - 1, listControlPoint[pointID - 1].transform.position);
                else //middle point
                {
                    EditSingleCurve(listPoint[pointID - 1].transform.position, pos, pointID - 1, listControlPoint[pointID - 1].transform.position);
                    EditSingleCurve(pos, listPoint[pointID + 1].transform.position, pointID, listControlPoint[pointID].transform.position);
                }
                break;
            case lineTypeList.bezier:
                if (pointID == 0) //start point
                    EditBezierCurve(pos, listPoint[pointID + 1].transform.position, pointID, listControlPoint[0].transform.position, listControlPoint[1].transform.position);
                else if (pointID == (listPoint.Count - 1)) //end point
                    EditBezierCurve(listPoint[pointID - 1].transform.position, pos, pointID - 1, listControlPoint[(pointID - 1) * 2].transform.position, listControlPoint[(pointID - 1) * 2 + 1].transform.position);
                else //middle point
                {
                    EditBezierCurve(listPoint[pointID - 1].transform.position, pos, pointID - 1, listControlPoint[(pointID - 1) * 2].transform.position, listControlPoint[(pointID - 1) * 2 + 1].transform.position);
                    EditBezierCurve(pos, listPoint[pointID + 1].transform.position, pointID, listControlPoint[pointID * 2].transform.position, listControlPoint[pointID * 2 + 1].transform.position);
                }
                break;
        }
        //Debug.Log("MovePointMarker : " + id + " / " + pos);
    }

    public void MoveControlPoint(int ctrlID, Vector3 pos)
    {
        switch (lineType) //##
        {
            case lineTypeList.straight: //straight doesn't have ctr point
                break;
            case lineTypeList.curve:
                EditSingleCurve(listPoint[ctrlID].transform.position, listPoint[ctrlID + 1].transform.position, ctrlID, pos);
                break;
            case lineTypeList.bezier:

                EditBezierCurve(listPoint[ctrlID / 2].transform.position, listPoint[ctrlID / 2 + 1].transform.position, ctrlID / 2, listControlPoint[(ctrlID % 2 == 0) ? ctrlID : (ctrlID - 1)].transform.position, listControlPoint[(ctrlID % 2 == 0) ? (ctrlID + 1) : ctrlID].transform.position);
                break;
        }
        //Debug.Log("MoveControlPoint : " + id + " / " + pos);
    }

    private void DrawStraitghtCurve(Vector3 p1, Vector3 p2)
    {
        lineRenderer.positionCount+=2;
        
        AddControlPoint(new Vector3(100, -100, 100)); //handle 1
        
        pos[addWeight] = p1;
        pos[addWeight+1] = p2;

        lineRenderer.SetPositions(pos);
    }

    private void DrawBezierCurve(Vector3 p1, Vector3 p2) //add point limit 500
    {
        Vector3 CtrlPt1, CtrlPt2;
        CtrlPt1.z = 0;
        //CtrlPt1.x = ((float)((p1.x + p2.x + 1.7320508076 * (p1.y - p2.y)) / 2));
        //CtrlPt1.y = ((float)((p1.y + p2.y + 1.7320508076 * (p2.x - p1.x)) / 2));
        CtrlPt1.x = Vector3.Lerp(p1, p2, 0.25f).x;
        CtrlPt1.y = Vector3.Lerp(p1, p2, 0.25f).y;

        CtrlPt2 = CtrlPt1;
        //CtrlPt2.x += 2;
        CtrlPt2.x = Vector3.Lerp(p1, p2, 0.75f).x;
        CtrlPt2.y = Vector3.Lerp(p1, p2, 0.75f).y;

        AddControlPoint(CtrlPt1); //handle 1
        AddControlPoint(CtrlPt2); //handle 2

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(0 + addWeight, p1);

        for (int i = 1; i < numpos - 1; i++)
        {
            lineRenderer.positionCount++;
            pos[(i - 1 + addWeight) + (numpos * (listPoint.Count - 2))] = Getcubemapoint((float)((float)i / (float)numpos), p1, CtrlPt1, CtrlPt2, p2);
            lineRenderer.SetPosition(i + addWeight, pos[(i - 1 + addWeight) + (numpos * (listPoint.Count - 2))]);
        }
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(numpos - 1 + addWeight, p2);
    }

    private void DrawSingleCurve(Vector3 p1, Vector3 p2) //add point limit 500
    {
        //point of control
        Vector3 CtrlPt;
        CtrlPt.z = 0f;
        //CtrlPt.x = ((float)((p1.x + p2.x + 1.7320508076 * (p1.y - p2.y)) / 2));
        //CtrlPt.y = ((float)((p1.y + p2.y + 1.7320508076 * (p2.x - p1.x)) / 2));
        CtrlPt.x = Vector3.Lerp(p1, p2, 0.5f).x;
        CtrlPt.y = Vector3.Lerp(p1, p2, 0.5f).y;

        AddControlPoint(CtrlPt); //handle

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(0 + addWeight, p1);

        for (int i = 1; i < numpos - 1; i++) //add new point only
        {
            lineRenderer.positionCount++;
            pos[(i - 1 + addWeight) + (numpos * (listPoint.Count - 2))] = Getquadmapoint((float)((float)i / (float)numpos), p1, p2, CtrlPt);
            lineRenderer.SetPosition(i + addWeight, pos[(i - 1 + addWeight) + (numpos * (listPoint.Count - 2))]);
        }

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(numpos - 1 + addWeight, p2);
    }

    private void EditStraitghtCurve(Vector3 p1, Vector3 p2)
    {
        Debug.LogWarning("dd : " + p1 + " / " + p2);
        Debug.LogWarning("aa : " + addWeight);


        //get own id
        //reduce addWeight

        return;

        pos[addWeight] = p1; 
        pos[addWeight + 1] = p2;

        lineRenderer.SetPositions(pos);
    }

    private void EditSingleCurve(Vector3 p1, Vector3 p2, int whichseg, Vector3 CtrlPt)
    {
        for (int i = 1; i <= numpos; i++)
        {
            pos[(i - 1) + (numpos * whichseg)] = Getquadmapoint((float)((float)i / (float)numpos), p1, p2, CtrlPt);
            pos[(i - 1) + (numpos * whichseg)].z = 0;
        }
        lineRenderer.SetPositions(pos);
        //lineRenderer.SetPosition(0, listPoint[id]);
    }

    private void EditBezierCurve(Vector3 p1, Vector3 p2, int whichseg, Vector3 CtrlPt1, Vector3 CtrlPt2)
    {
        for (int i = 1; i <= numpos; i++)
        {
            pos[(i - 1) + (numpos * whichseg)] = Getcubemapoint((float)((float)i / (float)numpos), p1, CtrlPt1, CtrlPt2, p2);
            pos[(i - 1) + (numpos * whichseg)].z = 0;
        }
        lineRenderer.SetPositions(pos);
    }

    private Vector3 Getmapoint(float t, Vector3 p1, Vector3 p2)
    {
        Vector3 returnpoint;
        returnpoint.x = p1.x + t * (p2.x - p1.x);
        returnpoint.y = p1.y + t * (p2.y - p1.y);
        returnpoint.z = 0;

        return returnpoint;
    }

    private Vector3 Getquadmapoint(float t, Vector3 p1, Vector3 p2, Vector3 controlPoint)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p1;
        p += u * 2 * t * controlPoint;
        p += tt * p2;

        return p;
    }

    private Vector3 Getcubemapoint(float t, Vector3 start, Vector3 ctrl1, Vector3 ctrl2, Vector3 end)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;
        Vector3 p = uuu * start;
        p += 3 * uu * t * ctrl1;
        p += 3 * u * tt * ctrl2;
        p += ttt * end;

        return p;
    }

    public void ResetStatus()
    {
        lineType = lineTypeList.none;
        addWeight = 0;
        
        listPoint.Clear();
        listControlPoint.Clear();
        dicPointType.Clear();

        pos = null;
        pos = new Vector3[maxpoints];

        lineRenderer.positionCount = 0;

        for (int i = 0; i < this.transform.childCount; i++)
            Destroy(this.transform.GetChild(i).gameObject);
    }

    public void SetLineType(int type)
    {
        switch (type)
        {
            case 1:
                lineType = lineTypeList.straight;
                break;
            case 2:
                lineType = lineTypeList.curve;
                break;
            case 3:
                lineType = lineTypeList.bezier;
                break;
        }
    }
}