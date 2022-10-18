using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private int sourceIndex;
    [SerializeField] private int targetIndex;
    
    [SerializeField] public bool PlayerIsExiting {get; set;}

    private void Awake() 
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        
    }
}
