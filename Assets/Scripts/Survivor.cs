using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Survivor : MonoBehaviour
{
    [SerializeField] private Sprite portrait;

    private GameObject selectedGameObject;
    private bool isHidden;
    private string originalTag;
    private GameObject sprite;
    private bool isDead;
    private Collider2D sCollider;
    private AudioSource footsteps;
    private AudioSource deathSound;
    private Rigidbody2D body;
    private bool executedDeathSequence;

    private void Awake() 
    {
        selectedGameObject = transform.Find("Selected").gameObject;
        SetSelectedVisible(false);
        isHidden = false;
        originalTag = this.gameObject.tag;
        sprite = transform.Find("Sprite").gameObject;
        isDead = false;
        sCollider = GetComponent<Collider2D>();
        footsteps = GetComponent<AudioSource>();
        body = GetComponent<Rigidbody2D>();
        deathSound = transform.Find("Death Sound").GetComponent<AudioSource>();
        executedDeathSequence = false;
    }

    private void Update() 
    {
        if (isDead && !executedDeathSequence)
        {
            sprite.transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
            footsteps.Stop();
            deathSound.Play();
            executedDeathSequence = true;
        }
        else if (isDead)
        {
            return;
        }
        else
        {
            UpdateAudioLogic();
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

    public Sprite GetPortrait()
    {
        return portrait;
    }

    private void UpdateAudioLogic()
    {
        if (body.velocity.magnitude > 0)
        {
            footsteps.Play();
        }
        else
        {
            footsteps.Stop();
        }
    }
}
