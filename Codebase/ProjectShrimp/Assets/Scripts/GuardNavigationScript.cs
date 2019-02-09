using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardNavigationScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveToDestination(Transform guardPosition, Vector3 destination, float speed, bool canMove)
    {
        bool wakaWaka = false;

        if(guardPosition.GetComponent<GuardScript>())
        {
            if (guardPosition.GetComponent<GuardScript>().state == GuardScript.GuardStates.patrol)
                wakaWaka = true;
        }  
        else
        {
            if (guardPosition.GetComponent<HeavyGuardScript>().state == HeavyGuardScript.GuardStates.patrol)
                wakaWaka = true;
        }

        if (MovementCheck(guardPosition, wakaWaka))
        {
            if (canMove)
            {
                guardPosition.Translate(Vector3.forward * speed * Time.deltaTime);
                if (guardPosition.GetComponent<GuardScript>())
                    guardPosition.Find("GuardSprite").GetComponent<Animator>().SetBool("isMoving", true);
                else
                    guardPosition.Find("HeavyGuard").GetComponent<Animator>().SetBool("IsMoving", true);
            }

        }
        else
        {
            if(guardPosition.GetComponent<GuardScript>() != null)
            {
                if (guardPosition.GetComponent<GuardScript>().isNormalGuard)
                    guardPosition.Find("GuardSprite").GetComponent<Animator>().SetBool("isMoving", false);
                else
                    guardPosition.Find("HeavyGuard").GetComponent<Animator>().SetBool("IsMoving", false);
                canMove = false;
            }
            else
            {
                canMove = false;
            }

        }

    }

    bool MovementCheck(Transform guardPosition, bool isPatrolling)
    {
        bool result = false;

        //Set origin vector

        Vector3 originVector;

        if (guardPosition.GetComponent<GuardScript>())
        {
            originVector = guardPosition.Find("Shoulder").Find("Arm").Find("Light").transform.position;
        }
        else
        {
            originVector = guardPosition.Find("Shoulder").Find("Arm").Find("Light").transform.position;
        }

        originVector = new Vector3(originVector.x, originVector.y - .5f, originVector.z);

        if (guardPosition.transform.position.x < originVector.x)
            originVector += new Vector3(.5F, 0, 0);
        else
            originVector -= new Vector3(.5F, 0, 0);

        RaycastHit hit;
        int groundLayer = LayerMask.GetMask("Ground");

        Debug.DrawRay(originVector, -Vector3.up * 1.5f, Color.cyan);
        //Check for ledges

        if (Physics.Raycast(originVector, -Vector3.up, out hit, 1.5f, groundLayer))
        {
            if (hit.transform.tag != "stairs")
            {
                //Debug.Log(hit.distance);
                Vector3 destinationVector;
                if (guardPosition.transform.position.x < originVector.x)
                    destinationVector = Vector3.right;
                else
                    destinationVector = Vector3.left;

                //Check for walls
                if (Physics.Raycast(originVector, destinationVector, out hit, 3f))
                {
                    if (hit.transform.tag == "Wall")
                    {
                        result = false;
                    }
                    else if (hit.transform.tag == "Enemy" && hit.distance <= .5f && !isPatrolling)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
                else
                {
                    result = true;
                }
            }
            else
                return false;
        }
        else
        {
            result = false;
        }

        return result;
    }
}
