using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            Survivor survivor = other.gameObject.GetComponent<Survivor>();

            GameManager.instance.QueueEscapedSurvivor(survivor);
            Destroy(other.gameObject);
        }
    }
}
