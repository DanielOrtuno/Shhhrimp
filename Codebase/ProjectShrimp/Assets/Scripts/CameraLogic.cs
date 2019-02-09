using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    Transform player;
    readonly float smoothTime = .2f;
    private Vector3 velocity = Vector3.zero;
    bool isShaking;
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        transform.parent = null;     
    }
    
    void Update()
    {
        if (!isShaking)
        {
            if (!player.gameObject.GetComponent<PlayerController>().isHidden || !Input.GetKeyDown(KeyCode.LeftControl))
            {
                Vector3 targetposition = player.TransformPoint(new Vector3(0, 0, -15));
                transform.position = Vector3.SmoothDamp(transform.position, targetposition, ref velocity, smoothTime);

            }
            else
            {
                Vector3 targetposition = player.TransformPoint(new Vector3(0, 0, -20));
                transform.position = Vector3.SmoothDamp(transform.position, targetposition, ref velocity, smoothTime);
            }
        }     
    }
     
    public void Shake(float intensity, float duration)
    {
        isShaking = true;
        StopAllCoroutines();
        StartCoroutine(ShakeEnum(intensity, duration));
    }

    IEnumerator ShakeEnum(float intensity, float duration)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float percentCompleted = elapsedTime / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentCompleted - 3.0f, 0.0f, 1.0f);

            float xPos = Random.value * 2.0f - 1.0f;
            float yPos = Random.value * 2.0f - 1.0f;
            xPos *= intensity * damper;
            yPos *= intensity * damper;

            transform.position = player.transform.position + new Vector3(xPos, yPos, -15);

            yield return null;

        }

        isShaking = false;
    }

}

