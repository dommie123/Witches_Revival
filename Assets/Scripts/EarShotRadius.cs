using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarShotRadius : MonoBehaviour
{
    [SerializeField] private Witch witch;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            witch.AlertToPlayerPosition(other.transform.position);
        }
    }
}
