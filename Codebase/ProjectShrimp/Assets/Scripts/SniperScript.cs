using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperScript : MonoBehaviour
{

    public bool rotate;
    public float angle;
    public float rotationSpeed;
    public float rotationAngle1;
    public float rotationAngle2;
    public float distance;
    bool canShoot;
    Transform trans;

    void Start()
    {
        rotate = true;
        angle = transform.rotation.z;
        GetComponent<LineRenderer>().positionCount = 2;
        trans = transform.parent;
        canShoot = true;
    }
    void Update()
    {
        angle = trans.eulerAngles.z;
        if (!rotate && rotationAngle1 < angle && angle < 180)
            rotate = true;

        else if (rotate && angle < 360 + rotationAngle2 && angle > 180)
            rotate = false;



        if (FindObjectOfType<PlayerController>().GetPlayerHP() <= 0)
            canShoot = true;

        if (rotate) // counter clockwise
        {
            trans.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.up * -distance, out hit))
            {
                
                GetComponent<LineRenderer>().SetPosition(0, transform.position);
                GetComponent<LineRenderer>().SetPosition(1, hit.point);

                if (hit.transform.gameObject.tag == "Player" && canShoot)
                {
                    Fire(4);
                    canShoot = false;
                }

            }
            else
            {
                GetComponent<LineRenderer>().SetPosition(0, transform.position);
                float radians = transform.eulerAngles.z * Mathf.Deg2Rad;
                GetComponent<LineRenderer>().SetPosition(1, new Vector3(((distance * -Mathf.Sin(radians)) + transform.position.x), ((distance * -Mathf.Cos(radians)) + transform.position.y), transform.position.z));
                Debug.Log(Mathf.Cos(radians));
                Debug.Log(Mathf.Sin(radians));
            }
        }

        if (!rotate) // clockwise
        {
            trans.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.up * -distance, out hit))
            {
                GetComponent<LineRenderer>().SetPosition(0, transform.position);
                GetComponent<LineRenderer>().SetPosition(1, hit.point);

                if (hit.transform.gameObject.tag == "Player" && canShoot)
                {
                    Fire(4);
                    canShoot = false;
                }
            }
            else
            {
                GetComponent<LineRenderer>().SetPosition(0, transform.position);
                float radians = transform.eulerAngles.z * Mathf.Deg2Rad;
                GetComponent<LineRenderer>().SetPosition(1, new Vector3(((distance * -Mathf.Sin(radians)) + transform.position.x), ((distance * -Mathf.Cos(radians)) + transform.position.y), transform.position.z));
            }

        }
    }


    virtual public void Fire(ushort _damage)
    {
        // Origin Vector
        Vector3 bulletPos;

        if (transform.eulerAngles.y == 90)
        {
            bulletPos = new Vector3(transform.position.x + .5f, transform.position.y, transform.position.z);

        }
        else
        {
            bulletPos = new Vector3(transform.position.x - .5f, transform.position.y, transform.position.z);

        }

        GameObject bullet = trans.parent.Find("Bullet").gameObject;
        //Assing new position to the object
        bullet.transform.position = bulletPos;
        bullet.transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 180);

        bullet.gameObject.SetActive(true);

        bullet.GetComponent<TrailRenderer>().Clear();
        bullet.GetComponent<Rigidbody>().velocity = transform.Find("Gun").right * Time.deltaTime * 80000;

        trans.parent.Find("Gunshot").GetComponent<AudioSource>().Play();
        FindObjectOfType<PlayerController>().DamagePlayerHP(4);
    }

}
