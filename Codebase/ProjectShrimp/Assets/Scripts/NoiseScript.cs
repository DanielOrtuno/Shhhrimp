using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseScript : MonoBehaviour
{

    public int priority;



    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Enemy")
        {
            if (other.transform.GetComponent<GuardScript>() != null)
            {
                GuardScript gd = other.gameObject.GetComponent<GuardScript>();
                if (gd.state != GuardScript.GuardStates.dead)
                    gd.ReactToSound(transform.position + new Vector3(0, 1, 0));

            }
            else
            {
                HeavyGuardScript gd = other.gameObject.GetComponent<HeavyGuardScript>();
                if (gd.state != HeavyGuardScript.GuardStates.dead)
                    gd.ReactToSound(transform.position + new Vector3(0, 1, 0));
            }

        }
    }


}
