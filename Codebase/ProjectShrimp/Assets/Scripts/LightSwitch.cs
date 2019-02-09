using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{

    bool isActive;
    List<GameObject> children = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        isActive = true;

        Transform[] Achildren = GetComponentsInChildren<Transform>();

        for (int i = 0; i < Achildren.Length; i++)
        {
            if (Achildren[i].tag == "BreakableLight")
                children.Add(Achildren[i].gameObject);
        }
    }
   
    void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E) && isActive)
        {
            isActive = false;
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i])
                {
                    children[i].transform.Find("Cone").gameObject.SetActive(false);
                    children[i].transform.Find("Spotlight").gameObject.SetActive(false);
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            isActive = true;
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i])
                {
                    children[i].transform.Find("Cone").gameObject.SetActive(true);
                    children[i].transform.Find("Spotlight").gameObject.SetActive(true);
                }
            }
        }
    }

}
