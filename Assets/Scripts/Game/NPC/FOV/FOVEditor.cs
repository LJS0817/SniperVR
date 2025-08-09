using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.blue;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.radius);

        Vector3 viewA1 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewA2 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewA1 * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewA2 * fov.radius);

        if(fov.VisibleObject())
        {
            Handles.color = Color.red;
            for(int  i = 0; i < fov.Targets.Count; i++)
            {
                Handles.DrawLine(fov.transform.position, fov.Targets[i].transform.position);
            }
        }
    }

    Vector3 DirectionFromAngle(float e, float d)
    {
        d += e;
        return new Vector3(Mathf.Sin(d * Mathf.Deg2Rad), 0, Mathf.Cos(d * Mathf.Deg2Rad));
    }
}
