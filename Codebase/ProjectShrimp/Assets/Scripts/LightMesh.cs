using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]

public class LightMesh : MonoBehaviour
{

    public float rotationSpeed;
    public float rotationAngle1 = 10;
    public float rotationAngle2 = -10;
    public float distance;
    public int positions;

    Quaternion InitialRotation;

    Vector3[] vertices;
    int[] tri;

    Mesh mesh;

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    void Start()
    {
        //positions = (int)((Mathf.Abs(rotationAngle1) + Mathf.Abs(rotationAngle2)) * (1 / rotationSpeed));
        //while (positions % 3 != 0)
        //    positions++;

        //InitialRotation = transform.rotation;


        vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(1.4f, -4.5f, 0), new Vector3(-1.4f, -4.5f, 0) };

        tri = new int[] { 0, 1, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tri;
        // uv = new Vector2[] { };

        //Creation();
    }

    //void Creation()
    //{
    //    mesh.Clear();

    //    transform.rotation = InitialRotation;
    //    vertices[0] = transform.position;

    //    // Vertices 
    //    for (int i = 1; i < vertices.Length; i++)
    //    {
    //        transform.Rotate(-Vector3.forward, rotationSpeed);

    //        RaycastHit hit;
    //        if (Physics.Raycast(transform.position, transform.right * distance, out hit, distance))
    //        {
    //            if (hit.distance <= distance)
    //                vertices[i] = hit.point;
    //        }
    //        else
    //        {
    //            vertices[i] = new Vector3(distance * Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad) + transform.position.x, distance * Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad) + transform.position.y, transform.position.z);
    //        }

    //    }

    //    mesh.vertices = vertices;

    //    // Triangles 
    //    tri[0] = 0;
    //    tri[1] = 1;
    //    tri[2] = 2;

    //    for (int i = 3; i < tri.Length - 2; i += 3)
    //    {
    //        tri[i] = 0;
    //        tri[i + 1] = i;
    //        tri[i + 2] = i + 1;
    //    }

    //    mesh.triangles = tri;

    //    // UV
    //    for (int i = 0; i < uv.Length; i++)
    //    {
    //        uv[i] = vertices[i];
    //    }
    //    mesh.uv = uv;
    //}

}
