using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survivor : MonoBehaviour
{
    private GameObject selectedGameObject;
    private bool isHidden;
    private string originalTag;
    private GameObject sprite;

    private void Awake() 
    {
        selectedGameObject = transform.Find("Selected").gameObject;
        SetSelectedVisible(false);
        isHidden = false;
        originalTag = this.gameObject.tag;
        sprite = transform.Find("Sprite").gameObject;
    }

    public void SetSelectedVisible(bool visible)
    {
        selectedGameObject.SetActive(visible);
    }

    public void MoveToPosition(Vector3 movePosition)
    {
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
}
