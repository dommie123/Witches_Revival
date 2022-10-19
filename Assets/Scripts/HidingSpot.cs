using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class HidingSpot : MonoBehaviour
{
    [SerializeField] private int capacity;
    [SerializeField] private GameObject selectedGameObject;

    private List<Survivor> occupants;  // # of people in hiding spot
    private bool isSelected;
    private float cooldown;
    private Collider2D hsCollider;

    private void Awake() 
    {
        occupants = new List<Survivor>();
        isSelected = false;
        selectedGameObject.SetActive(false);
        cooldown = 0f;
        hsCollider = GetComponent<Collider2D>();
    }

    private void Update() 
    {
        UpdateCooldown();
        hsCollider.isTrigger = !HasNoVacancy();
        
        if (!(HasNoOccupants() || !isSelected))
        {
            UpdatePlayerInputs();
        }        

    }

    public void SetSelected(bool isSelected)
    {        
        this.isSelected = isSelected;
        selectedGameObject.SetActive(isSelected);
    }

    private void UpdatePlayerInputs()
    {
        if (Input.GetMouseButtonUp(1))  // right mouse button is released
        {
            foreach (Survivor survivor in occupants)
            {
                if (survivor.GetIsHidden())
                    survivor.ToggleHidden();
                
                Vector3 movePosition = UtilsClass.GetMouseWorldPosition();
                survivor.MoveToPosition(movePosition);
            }

            occupants.Clear();
            cooldown = 0.5f;
        }
    }

    private void UpdateCooldown()
    {
        if (cooldown <= 0f)
            return;
        
        cooldown -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player" && !HasNoVacancy() && cooldown <= 0f)
        {
            Survivor survivor = other.gameObject.GetComponent<Survivor>();
            survivor.ToggleHidden();
            occupants.Add(survivor);
        }    
    }

    private bool HasNoOccupants()
    {
        return occupants.Count == 0;
    }

    private bool HasNoVacancy()         // In other words, is this hiding spot full?
    {
        return occupants.Count == capacity;
    }
}
