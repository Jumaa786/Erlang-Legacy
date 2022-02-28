﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This enum is fot the type of lever we have.
//Buton Levers only are activated once, 
//Handlers, activates when Structure it not on the point and then are desactivated. 
public enum LeverType
{
    Button, Handler
}

public class MoveStructureToPointCaller : MonoBehaviour
{
    [SerializeField] LeverType type;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite activatedSprite;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float waitTime;
    [SerializeField] MoveStructureToPoint moveStructureToPointEngine;
    [SerializeField] Rigidbody2D structure;
    [SerializeField] Transform myPoint;
    bool activated;

    HashSet<GameObject> memory = new HashSet<GameObject>();


    //pre: --
    //post: seting defautl sprite to gameobject
    void Start()
    {
        spriteRenderer.sprite = defaultSprite;
    }

    //pre: -- 
    //post: if lever is activated, and structure is on myPoint
    //      activaed is set to false 
    //      if type is Handler it changes sprite. 
    void FixedUpdate()
    {
        if (activated && IsStructureOnPoint())
        {
            if (type == LeverType.Handler)
            {
                ChangeSprite();
            }
            activated = false;
        }
    }

    //pre: --
    //post: if collider is player and is moveStructure not previously activated
    //      is structure is not on Point we activate moveStructure and change the sprite to activated
    //       if structure is on point and lever is type Handler, 
    //      coroutine () is activated to show the user that platform is on the "myPoint"
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Player" && !activated)
        {
            if (memory.Contains(other.gameObject)) return;

            memory.Add(other.gameObject);

            if (!IsStructureOnPoint())
            {
                ChangeSprite();
                moveStructureToPointEngine.Activate(myPoint);
                activated = true;
            }
            else if (type == LeverType.Handler)
            {
                StartCoroutine(IActivateAndDesactivateHandler());
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (memory.Contains(other.gameObject))
        {
            memory.Remove(other.gameObject);
        }
    }

    //pre: type is Handler
    //post: Active sprite is set and in waitTime time is set to default. 
    IEnumerator IActivateAndDesactivateHandler()
    {
        ChangeSprite();
        yield return new WaitForSeconds(waitTime);
        ChangeSprite();
    }

    //pre: - 
    //post: changes sprite between activated - default
    private void ChangeSprite()
    {
        if (spriteRenderer.sprite != activatedSprite)
        {
            spriteRenderer.sprite = activatedSprite;
        }
        else
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }

    //pre: --
    //post: returns true if structure is on myPoint position
    private bool IsStructureOnPoint()
    {
        return structure.transform.position == myPoint.position;
    }
}
