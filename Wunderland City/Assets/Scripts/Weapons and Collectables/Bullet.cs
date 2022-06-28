using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private Vector3 bulletDestination;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(LerpPosition(bulletDestination, 1f));
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        // smoothly lerp bullet to destination over time
        float time = 0;
        Vector3 startPosition = transform.position;

        Debug.Log("lerping from {transform.position}");

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            Debug.Log("currently at {transform.position}");
            yield return null;
        }

        transform.position = targetPosition;
    }

    public void SetDestination(Vector3 destination)
    {
        bulletDestination = destination;
      //  StartCoroutine(LerpPosition(bulletDestination, 5f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        // hit something
        if (collision.gameObject.tag == "SpiderQueen")
        {
            Debug.Log("hit Spider Squeen, BANG!!!!!");
        }
    }
}
