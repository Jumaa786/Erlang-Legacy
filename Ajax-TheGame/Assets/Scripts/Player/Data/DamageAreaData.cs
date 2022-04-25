
using System;
using Core.Combat;
using UnityEngine;

namespace Core.Player.Data
{

    [Serializable]
    public class DamageAreaData
    {
        [SerializeField] Triggerable dash;
        [SerializeField] Triggerable punch; // basic attack damage area

        public Triggerable Dash { get => dash; set => dash = value; }
        public Triggerable Punch { get => punch; set => punch = value; }
    }

}