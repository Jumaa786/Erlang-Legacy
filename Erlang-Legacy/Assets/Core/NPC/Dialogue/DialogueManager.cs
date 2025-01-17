﻿using Core.Player.Controller;
using Core.Shared.Enum;
using Core.Player.Utility;
using Core.Shared;
using UnityEngine;
using Core.NPC.Util;
using UnityEngine.Events;

namespace Core.NPC
{
    public class DialogueManager : MonoBehaviour
    {
        public NPCData npcData;
        [SerializeField] Transform talkPoint;
        [SerializeField] Animator npcAnimator;
        public UnityEvent actionAtEndOfConversation;
        private bool playerIn = false;
        private bool inConversation = false;

        //pre: --
        //post: if player is in range for conversation and it not already in one, 
        //      when interacts it Runs Dialogue. 
        private void Update()
        {
            if (playerIn && !inConversation)
            {
                if (Input.GetButton(CharacterActions.Interact))
                {
                    TriggerDialogue();
                }
            }
        }

        //pre: --
        //post: if is player playerIn = true;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                playerIn = true;
            }
        }

        //pre: --
        //post: if is player playerIn = false;
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                playerIn = false;
            }
        }

        //pre: --
        //post: positions player to tlkpoint
        //      starts conversation with NPC
        //      disables player controll 
        //      return when it's conversation is over
        public void TriggerDialogue()
        {
            inConversation = true;

            var player = PlayerController.Instance;
            player.Controllable = false;
            Face facing = talkPoint.position.x - player.gameObject.transform.position.x > 0
            ? Face.Right : Face.Left;

            MovePlayer.Trigger(talkPoint, 0f, facing, 0, () =>
            {

                facing = talkPoint.position.x - transform.position.x > 0
                ? Face.Left : Face.Right;
                player.SetFacing(facing);

                GetComponentInChildren<Dialogue>().StartConversation(npcData, npcAnimator, () =>
                {
                    player.Controllable = true;
                    inConversation = false;
                    actionAtEndOfConversation?.Invoke();
                });
            });
        }

    }

}