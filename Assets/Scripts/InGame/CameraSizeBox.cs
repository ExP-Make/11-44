using System;
using UnityEngine;

public class CameraSizeBox : MonoBehaviour
{
    public Vector2 BoxSize = new Vector2(10, 10);
    public Vector3 BoxCenter = Vector3.zero;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(BoxCenter, new Vector3(BoxSize.x, BoxSize.y, 0));
    }
}
