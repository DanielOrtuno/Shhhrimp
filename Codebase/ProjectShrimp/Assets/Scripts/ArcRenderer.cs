using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class ArcRenderer : MonoBehaviour
{

    LineRenderer arc;

    public float velocity;
    public float angle;
    public int resolution = 10;
    public float maxDistance;

    float g;
    public float radianAngle;

    void Awake()
    {
        arc = GetComponent<LineRenderer>();
        g = 35;
    }

    void OnValidate()
    {
        if (arc && Application.isPlaying)
        {
            SetAngle();
            SetVelocity();
            RenderArc();
        }
    }

    private void OnEnable()
    {
        if (arc && Application.isPlaying)
        {
            SetAngle();
            SetVelocity();
            RenderArc();
        }
    }

    // Use this for initialization
    void Start()
    {
        SetAngle();
        SetVelocity();
        RenderArc();
    }

    // sets line renderer
    void RenderArc()
    {
        arc.positionCount = (resolution + 1);
        arc.SetPositions(CalculateArcArray());
    }
    Vector3[] CalculateArcArray()
    {
        Vector3[] array = new Vector3[resolution + 1];
        radianAngle = Mathf.Deg2Rad * angle;

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, Camera.main.transform.position.z * -1));
        maxDistance = mouse.x - transform.parent.position.x;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / (float)resolution;
            array[i] = CalculateArcPoint(t, maxDistance) + new Vector3(transform.parent.position.x, transform.parent.position.y + 1, transform.parent.position.z - 1);
        }

        return array;
    }

    Vector3 CalculateArcPoint(float t, float maxDistance)
    {
        float x = t * maxDistance;
        float y = ((x * Mathf.Tan(radianAngle)) - ((g * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle))));
        return new Vector3(x, y);
    }

    void SetAngle()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, Camera.main.transform.position.z * -1));
        mouse.z = transform.parent.position.z;
        Vector3 a = transform.parent.position + new Vector3(0, 1, 0);
        Vector3 b = mouse;

        angle = (Mathf.Atan2(b.y - a.y, b.x - a.x) * (180 / Mathf.PI));
    }
    void SetVelocity()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, Camera.main.transform.position.z * -1));
        mouse.z = transform.parent.position.z;

        velocity = (mouse - (transform.parent.position + new Vector3(0, 1, 0))).magnitude * 2;
    }
}
