﻿using System.Collections;
using UnityEngine;

namespace Core.UI.Notifications
{
    public class NotificationManager : MonoBehaviour
    {
        [SerializeField] GameObject notification;

        public static NotificationManager Instance;

        const float notificationLifeTime = 5f;

        //pre: --
        //post: if these is no NotificationDisposer this becomes the one
        //      else it destroys itself
        private void Awake()
        {
            var matches = FindObjectsOfType<NotificationManager>();

            if (matches.Length > 1)
                Destroy(gameObject);
            else Instance = this;
        }

        //pre: --
        //post: instanciates a new notification 
        private void NewNotification(string title, string description, Sprite sprite)
        {
            var currentNotification = Instantiate(notification, this.transform);
            NotificationBehaviour notificationBehaviour = currentNotification.GetComponent<NotificationBehaviour>();
            if (notificationBehaviour != null)
            {
                StartCoroutine(notificationBehaviour.DisplayNotification(
                    title,
                    description,
                    sprite,
                    notificationLifeTime
                ));
            }
            else
            {
                Destroy(currentNotification);
            }
        }

        //pre: --
        //post: it's shows a new notification
        public void PostNotification(string title, string description, Sprite sprite)
        {
            NewNotification(title, description, sprite);
        }

        //pre: --
        //post: it's shows a new notification after delay 
        public void PostNotificationWithDelay(string title, string description, Sprite sprite, float delay)
        {
            StartCoroutine(WaitAndPostNotification(title, description, sprite, delay));
        }

        private IEnumerator WaitAndPostNotification(string title, string description, Sprite sprite, float delay)
        {
            yield return new WaitForSeconds(delay);
            NewNotification(title, description, sprite);
        }
    }
}