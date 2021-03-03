using UnityEngine;

public class MovePointMarker : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;

    public int pointID;
    Bezier bezier;


    private void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseWorldPos();

        Debug.LogWarning("MovePointMarker : " + pointID);
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPos() + mOffset;
        bezier = transform.parent.GetComponent<Bezier>();
        bezier.MovePointMarker(pointID, transform.position);
    }
}