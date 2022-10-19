using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survivor : MonoBehaviour
{
    private GameObject selectedGameObject;
    private bool isHidden;
    private string originalTag;
    private GameObject sprite;
    private bool isDead;
    private Collider2D sCollider;

    private void Awake() 
    {
        selectedGameObject = transform.Find("Selected").gameObject;
        SetSelectedVisible(false);
        isHidden = false;
        originalTag = this.gameObject.tag;
        sprite = transform.Find("Sprite").gameObject;
        isDead = false;
        sCollider = GetComponent<Collider2D>();
    }

    private void Update() 
    {
        if (isDead)
        {
            sprite.transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
        }    
    }

    public void SetSelectedVisible(bool visible)
    {
        if (!isDead)
            selectedGameObject.SetActive(visible);
    }

    public void MoveToPosition(Vector3 movePosition)
    {
        if (!isDead)
            GetComponent<IMovePosition>().SetMovePosition(movePosition);
    }

    public void ToggleHidden()
    {
        isHidden = !isHidden;

        if (isHidden)
        {
            this.gameObject.tag = $"{originalTag} (Hidden)";
            MoveToPosition(transform.position);
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
            this.gameObject.tag = originalTag;
        }
    }

    public bool GetIsHidden()
    {
        return isHidden;
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    public void KillSurvivor()
    {
        isDead = true;
        selectedGameObject.SetActive(false);
        Destroy(sCollider);
        GetComponent<IMovePosition>().SetMovePosition(transform.position);
    }
}
