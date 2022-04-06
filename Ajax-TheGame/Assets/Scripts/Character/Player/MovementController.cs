﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Core.Shared.Enum;

namespace Core.Character.Player
{
    public class MovementController : MonoBehaviour
    {
        [Header("Configurations")]
        [Tooltip("Displacement power on sides while running")][SerializeField] float basicSpeed;
        [Tooltip("Displacement power on sides while dashing")][SerializeField] float dashSpeed;
        [Tooltip("Displacement power on sides while jumping")][SerializeField] float jumpForce = 2;
        [Tooltip("How much time player can hold jump bottom")][SerializeField] float holdJump = 0.3f;
        [SerializeField] LayerMask whatIsGround;

        ////////////////////////////////////////////////////////////////////////////////////////////////

        Rigidbody2D rb;
        BoxCollider2D boxCollider2D;
        float jumpTimeCounter = 0.2f;
        bool isJumping = false;
        bool hasJumped = false;
        bool dashing = false;
        bool impulsed = false;
        float gravityScale = 1;
        Vector2 velocityModifyer;
        BasePlayer basePlayer;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            basePlayer = GetComponent<BasePlayer>();
            gravityScale = this.rb.gravityScale;
            velocityModifyer = Vector2.one;
        }

        void Update()
        {
            if (dashing) return;
            SmoothJump();
        }

        void FixedUpdate()
        {
            if (dashing) return;

            int xNormalized = basePlayer.HorizontalInputNormalized();
            // when Ajax is at the air, we let him take certain control of it's movement
            float vx = impulsed ?
            rb.velocity.x + xNormalized * basicSpeed * 0.05f
            : xNormalized * basicSpeed * velocityModifyer.x;

            rb.velocity = new Vector2(vx, rb.velocity.y);
            basePlayer.Run(Mathf.Abs(rb.velocity.x) > Mathf.Epsilon);

            if (hasJumped && IsGrounded())
            {
                basePlayer.Land();
                hasJumped = false;
            }

            velocityModifyer = Vector2.one;
        }

        /// <sumary>
        /// freze normal control for a certain time applying the impulse.
        /// if anything had change its gravity, 
        // the methods recover its firt local gravity scale
        /// <sumary>
        public void ImpulseUp(float force)
        {
            this.rb.gravityScale = gravityScale;
            impulsed = true;
            Freeze();
            rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }

        /// <sumary>
        /// freze normal control for a certain time applying the impulse.
        /// if anything had change its gravity, 
        // the methods recover its firt local gravity scale
        /// <sumary>
        public void Impulse(Vector2 impulse)
        {
            this.rb.gravityScale = gravityScale;
            impulsed = true;
            Freeze();
            rb.AddForce(impulse, ForceMode2D.Impulse);
        }

        void SmoothJump()
        {
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                isJumping = true;
                jumpTimeCounter = holdJump;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                basePlayer.Jump();
            }

            if (Input.GetButton("Jump") && isJumping)
            {
                if (jumpTimeCounter > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    jumpTimeCounter -= Time.deltaTime;
                }
                else
                {
                    isJumping = false;
                    hasJumped = true;
                }
            }

            if (Input.GetButtonUp("Jump"))
            {
                isJumping = false;
                hasJumped = true;
            }

        }

        // pre: --
        // post: adds force impulse with facing orientation
        public IEnumerator DashCoroutine(PlayerFacing facing, float duration, System.Action onComplete = null)
        {
            dashing = true;
            float gravityScale = this.rb.gravityScale;
            Freeze();
            this.rb.gravityScale = 0;
            var direction = facing == PlayerFacing.Left ? -1 : 1;
            this.rb.AddForce(new Vector2(dashSpeed * direction, 0f), ForceMode2D.Impulse);
            yield return new WaitForSeconds(duration);

            // avoids stack when you had dash
            // and them Ajax was trigger by impulse effect
            if (!impulsed)
            {
                Freeze();
            }
            this.rb.gravityScale = gravityScale;
            dashing = false;
            if (onComplete != null) onComplete();
        }

        public void Freeze()
        {
            this.rb.velocity = Vector2.zero;
        }

        bool IsGrounded()
        {
            float extra = 0.1f;
            RaycastHit2D ray = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0, Vector2.down, extra, whatIsGround);
            bool grounded = ray.collider != null;
            Color rayColor = grounded ? Color.green : Color.red;

            Debug.DrawRay(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extra), rayColor);
            Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extra), rayColor);
            Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, boxCollider2D.bounds.extents.y + extra), Vector2.right * (2 * boxCollider2D.bounds.extents.x), rayColor);

            return grounded;
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            // trigger effect ends when you had collide with something
            if (impulsed)
            {
                impulsed = false;
            }
        }

        //pre: -
        //post: velocity modifyer is updated with the values 
        //that are going to modify the velocity on ONE fixedUpdate
        public void ModifyVelocity(Vector2 velocityModifyer)
        {
            this.velocityModifyer = velocityModifyer;
        }

    }
}
