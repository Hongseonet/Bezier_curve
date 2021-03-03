using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    enum lineTypeList { straight, curve, bezier };
    lineTypeList lineType;

    const int maxpoints = 10000;
    const int numpos = 20; //curved element : line renderer positions : 50
    int addWeight = 0; //for stack count
    public LineRenderer lineRenderer;
    public GameObject Linepoint;
    public GameObject ControlPoint;

    [SerializeField] List<GameObject> PointList = new List<GameObject>();
    [SerializeField] List<GameObject> ControlPointList = new List<GameObject>();

    public Vector3 sizee;
    public Vector3 newPos, oldPos;
    private Vector3[] pos;


    // Start is called before the first frame update
    void Start()
    {
        pos = new Vector3[maxpoints];

        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift))
        {
            newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPos.z = 0;

            if (CreatePointMarker(newPos) > 1)
            {
                Debug.Log("create new point");

                switch (lineType)
                {
                    case lineTypeList.straight:
                        break;
                    case lineTypeList.curve:
                        drawSingleCurve(oldPos, newPos);
                        break;
                    case lineTypeList.bezier:
                        drawBezierCurve(oldPos, newPos);
                        break;
                }
            }
            oldPos = newPos;
        }
    }

    private int CreatePointMarker(Vector3 pointPosition)
    {
        GameObject pm = Instantiate(Linepoint, pointPosition, Quaternion.identity, transform);
        PointList.Add(pm);

        MovePointMarker mctrlpt = pm.GetComponent<MovePointMarker>();
        mctrlpt.pointID = PointList.Count - 1;

        sizee = PointList[PointList.Count - 1].transform.position;

        return PointList.Count;
    }

    private int AddControlPoint(Vector3 pointPosition)
    {
        GameObject cp = Instantiate(ControlPoint, pointPosition, Quaternion.identity, transform);
        ControlPointList.Add(cp);

        MoveCtrlPt mctrlpt = cp.GetComponent<MoveCtrlPt>();
        mctrlpt.pointID = ControlPointList.Count - 1;

        return ControlPointList.Count;
    }

    private int InsertControlPoint(Vector3 pointPosition, int id)
    {
        GameObject cp = Instantiate(ControlPoint, pointPosition, Quaternion.identity, transform);
        ControlPointList.Insert(id, cp);

        MoveCtrlPt mctrlpt = cp.GetComponent<MoveCtrlPt>();
        mctrlpt.pointID = id;

        return ControlPointList.Count;
    }

    public void MovePointMarker(int id, Vector3 pos)
    {
        //don't move the first point....
        oldPos = PointList[PointList.Count - 1].transform.position;

        switch (lineType)
        {
            case lineTypeList.straight:
                if (id == 0)
                {
                }
                else
                {
                }
                break;
            case lineTypeList.curve:
                if (id == 0) //start point
                    editSingleCurve(pos, PointList[id + 1].transform.position, id, ControlPointList[id].transform.position);
                else if(id == (PointList.Count - 1)) //end point
                    editSingleCurve(PointList[id - 1].transform.position, pos, id - 1, ControlPointList[id - 1].transform.position);
                else //middle point
                {
                    editSingleCurve(PointList[id - 1].transform.position, pos, id - 1, ControlPointList[id - 1].transform.position);
                    editSingleCurve(pos, PointList[id + 1].transform.position, id, ControlPointList[id].transform.position);
                }
                break;
            case lineTypeList.bezier:
                if (id == 0) //start point
                    editBezierCurve(pos, PointList[id + 1].transform.position, id, ControlPointList[0].transform.position, ControlPointList[1].transform.position);
                else if (id == (PointList.Count - 1)) //end point
                    editBezierCurve(PointList[id - 1].transform.position, pos, id - 1, ControlPointList[(id - 1) * 2].transform.position, ControlPointList[(id - 1) * 2 + 1].transform.position);
                else //middle point
                {
                    editBezierCurve(PointList[id - 1].transform.position, pos, id - 1, ControlPointList[(id - 1) * 2].transform.position, ControlPointList[(id - 1) * 2 + 1].transform.position);
                    editBezierCurve(pos, PointList[id + 1].transform.position, id, ControlPointList[id * 2].transform.position, ControlPointList[id * 2 + 1].transform.position);
                }
                break;
        }
        Debug.Log("MovePointMarker : " + id + " / " + pos);
    }

    public void MoveControlPoint(int id, Vector3 pos)
    {
        switch (lineType)
        {
            case lineTypeList.curve:
                editSingleCurve(PointList[id].transform.position, PointList[id + 1].transform.position, id, pos);
                break;
            case lineTypeList.bezier:
                editBezierCurve(PointList[id / 2].transform.position, PointList[id / 2 + 1].transform.position, id / 2, ControlPointList[(id % 2 == 0) ? id : (id - 1)].transform.position, ControlPointList[(id % 2 == 0) ? (id + 1) : id].transform.position);
                break;
        }
        Debug.Log("MoveControlPoint : " + id + " / " + pos);
    }

    private void drawBezierCurve(Vector3 p1, Vector3 p2) //add point limit 500
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
            pos[(i - 1 + addWeight) + (numpos * (PointList.Count - 2))] = getcubemapoint((float)((float)i / (float)numpos), p1, CtrlPt1, CtrlPt2, p2);
            pos[(i - 1 + addWeight) + (numpos * (PointList.Count - 2))].z = 0;

            lineRenderer.SetPosition(i + addWeight, pos[(i - 1 + addWeight) + (numpos * (PointList.Count - 2))]);
        }
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(numpos - 1 + addWeight, p2);

        addWeight += numpos; //eg 0, 20, 40 ...
    }

    private void drawSingleCurve(Vector3 p1, Vector3 p2) //add point limit 500
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
            pos[(i - 1 + addWeight) + (numpos * (PointList.Count - 2))] = getquadmapoint((float)((float)i / (float)numpos), p1, p2, CtrlPt);
            pos[(i - 1 + addWeight) + (numpos * (PointList.Count - 2))].z = 0;

            lineRenderer.SetPosition(i + addWeight, pos[(i - 1 + addWeight) + (numpos * (PointList.Count - 2))]);
        }

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(numpos - 1 + addWeight, p2);

        addWeight += numpos; //eg 0, 20, 40 ...
    }

    private void editSingleCurve(Vector3 p1, Vector3 p2, int whichseg, Vector3 CtrlPt)
    {
        for (int i = 1; i <= numpos; i++)
        {
            pos[(i - 1) + (numpos * whichseg)] = getquadmapoint((float)((float)i / (float)numpos), p1, p2, CtrlPt);
            pos[(i - 1) + (numpos * whichseg)].z = 0;
        }
        lineRenderer.SetPositions(pos);
        //lineRenderer.SetPosition(0, PointList[id]);
    }

    private void editBezierCurve(Vector3 p1, Vector3 p2, int whichseg, Vector3 CtrlPt1, Vector3 CtrlPt2)
    {
        for (int i = 1; i <= numpos; i++)
        {
            pos[(i - 1) + (numpos * whichseg)] = getcubemapoint((float)((float)i / (float)numpos), p1, CtrlPt1, CtrlPt2, p2);
            pos[(i - 1) + (numpos * whichseg)].z = 0;
        }
        lineRenderer.SetPositions(pos);
    }

    private void drawLinearCurve(Vector3 p1, Vector3 p2)
    {
        for (int i = 1; i <= numpos; i++)
        {
            pos[(i - 1) + numpos * (PointList.Count - 1)] = getmapoint((float)((float)i / (float)numpos), p1, p2);
            pos[(i - 1) + numpos * (PointList.Count - 1)].z = 0;
        }
        lineRenderer.SetPositions(pos);
    }

    private Vector3 getmapoint(float t, Vector3 p1, Vector3 p2)
    {
        Vector3 returnpoint;
        returnpoint.x = p1.x + t * (p2.x - p1.x);
        returnpoint.y = p1.y + t * (p2.y - p1.y);
        returnpoint.z = 0;

        return returnpoint;
    }

    private Vector3 getquadmapoint(float t, Vector3 p1, Vector3 p2, Vector3 controlPoint)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p1;
        p += u * 2 * t * controlPoint;
        p += tt * p2;

        return p;
    }

    private Vector3 getcubemapoint(float t, Vector3 start, Vector3 ctrl1, Vector3 ctrl2, Vector3 end)
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

    public void Clear()
    {
        PointList.Clear();
        ControlPointList.Clear();
        pos = null;
        pos = new Vector3[maxpoints];

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