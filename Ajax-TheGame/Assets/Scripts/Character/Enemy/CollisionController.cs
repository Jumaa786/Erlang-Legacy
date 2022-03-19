﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Effect;
using Core.Character.Player;

namespace Enemy
{
    public class CollisionController : MonoBehaviour
    {
        [SerializeField] HealthTaker healthTakerEffect;

        Collider2D myCollider;
        Collider2D playerCollider;

        void Awake()
        {
            myCollider = GetComponent<Collider2D>();
            playerCollider = BasePlayer.Instance.GetComponent<Collider2D>();
        }

        void FixedUpdate()
        {
            if (IsTouchingPlayer())
            {
                CollidePlayer();
            }
        }

        bool IsTouchingPlayer()
        {
            return myCollider.IsTouching(playerCollider);
        }

        void CollidePlayer()
        {
            if (BasePlayer.Instance.CanBeTouch())
            {
                BasePlayer.Instance.OnCollision(gameObject);
            }
        }
    }
}
