﻿using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Core.Combat.IA.Conditional
{
    public class IsDestroyable : EnemyConditional
    {
        public override TaskStatus OnUpdate()
        {
            bool destroyed = destroyable.IsDestroyed;
            return destroyed ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
