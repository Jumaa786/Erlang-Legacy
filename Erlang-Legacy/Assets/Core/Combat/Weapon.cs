﻿using System;
using Core.Combat.Projectile;
using UnityEngine;

namespace Core.Combat
{
    [Serializable]
    public class Weapon
    {
        public AbstractProjectile projectilePrefab;
        public Transform weaponTransform;
        public float horizontalForce = 5.0f;
        public float verticalForce = 4.0f;
    }
}