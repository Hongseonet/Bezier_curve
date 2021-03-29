using UnityEngine;

public class MovePointMarker : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;

    public int pointID;
    Bezier bezier;


    public void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseWorldPos();

        //Debug.LogWarning("MovePointMarker : " + gameObject.name);
        Debug.Log("MovePointMarker : " + transform.GetComponentInParent<DrawManager>().BezierIdx + " / " + pointID);
        transform.GetComponentInParent<DrawManager>().RefBezier = transform.GetComponentInParent<Bezier>().gameObject;
    }

    public Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    public void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + mOffset;
        bezier = transform.parent.GetComponent<Bezier>();
        bezier.MovePointMarker(pointID, transform.position);
    }
}