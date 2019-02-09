using UnityEngine;

public class FlashLight : MonoBehaviour
{

    float angleBetween;
    float rotationAngle1;
    float rotationAngle2;
    float distance;
    int positions;
    bool firstTime;
    float InitialRotation;
    int layermask;
    int flipper;

    float initialParentRotation;

    Transform parent;
    Transform pivot;

    void Start()
    {
        angleBetween = 1;
        rotationAngle1 = 4;
        rotationAngle2 = 4;
        distance = 6;

        positions = (int)((Mathf.Abs(rotationAngle1) + Mathf.Abs(rotationAngle2)) * 2 * (1 / angleBetween));
        GetComponent<LineRenderer>().positionCount = positions;
        InitialRotation = Mathf.Abs(rotationAngle1);
        layermask = LayerMask.GetMask("Default", "Ground", "Player", "GrappleSpot");
        if (transform.parent.parent.GetComponent<DroneGuard>())
        {
            initialParentRotation = transform.parent.eulerAngles.y;
            parent = transform.parent.parent;
            pivot = transform.parent;
        }
        else if (transform.parent.parent.parent.GetComponent<HeavyGuardScript>())
        {
            initialParentRotation = transform.parent.parent.parent.transform.eulerAngles.y;
            parent = transform.parent.parent.parent;
            pivot = transform.parent.parent;
        }
        else if (transform.parent.parent.parent.GetComponent<GuardScript>())
        {
            initialParentRotation = transform.parent.parent.parent.transform.eulerAngles.y;
            parent = transform.parent.parent.parent;
            pivot = transform.parent.parent;
        }



        if (parent.eulerAngles.y == 90)
            transform.eulerAngles = new Vector3(0, 0, 0);
        else
            transform.eulerAngles = new Vector3(0, 0, 180);

        if (parent.GetComponent<DroneGuard>())
            transform.eulerAngles = new Vector3(0, 0, -90);


    }

    void Update()
    {
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
        if (parent.eulerAngles.y != initialParentRotation)
        {
            flipper *= -1;
            InitialRotation = Mathf.Abs(rotationAngle1);
            initialParentRotation = parent.transform.eulerAngles.y;
            firstTime = !firstTime;
                if (parent.eulerAngles.y <= 90.00001)
                    transform.eulerAngles = new Vector3(0, 0, 0 - Mathf.Abs(pivot.eulerAngles.x));
                else
                    transform.eulerAngles = new Vector3(0, 0, 180 + Mathf.Abs(pivot.eulerAngles.x));
        }


        Creation();

        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
    }

    void Creation()
    {
        if (!firstTime)
        {
            transform.Rotate(Vector3.forward, InitialRotation);
            InitialRotation += Mathf.Abs(rotationAngle2);
            firstTime = !firstTime;
        }
        else
            transform.Rotate(Vector3.forward, InitialRotation);

        for (int i = 0; i < positions;)
        {
            transform.Rotate(Vector3.forward, -angleBetween);
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.right * distance, out hit, distance, layermask);
            float radians = transform.eulerAngles.z * Mathf.Deg2Rad;
            GetComponent<LineRenderer>().SetPosition(i++, transform.position);
            GetComponent<LineRenderer>().SetPosition(i++, new Vector3(((distance * Mathf.Cos(radians)) + transform.position.x), ((distance * Mathf.Sin(radians)) + transform.position.y), transform.position.z));
            try
            {
                if (hit.transform)
                    if (hit.distance <= distance)
                    {
                        GetComponent<LineRenderer>().SetPosition(i - 2, transform.position);
                        GetComponent<LineRenderer>().SetPosition(i - 1, hit.point);
                        if (hit.transform.tag == "Player")
                            hit.transform.GetComponent<PlayerController>().inSpotLightTimer = .2f;
                    }
            }
            catch (System.Exception)
            { }
        }
    }
}
