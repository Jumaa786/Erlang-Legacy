﻿using System.Collections;
using System.Collections.Generic;
using Core.Shared.Enum;
using DG.Tweening;
using UnityEngine;

namespace Core.Shared
{
    public static class Function
    {
        // pre: --
        // post: compute if collision is made from front or backs
        //      if collision is made in center, returns back and front randomly
        public static Side RelativeCollisionSide(Transform origin, Transform other)
        {
            Vector3 direction = origin.InverseTransformPoint(other.position);
            if (direction.x < 0)
            {
                return Side.Back;
            }
            else if (direction.x > 0)
            {
                return Side.Front;
            }
            return Random.Range(0, 2) == 0 ? Side.Back : Side.Front;
        }

        // def: from external point of view, compute if collision is made from front or backs
        public static Face CollisionSide(Transform origin, Transform other)
        {
            bool sameX = origin.position.x == other.position.x;

            if (sameX)
                 return Random.Range(0, 2) == 0 ? Face.Left : Face.Right;

            return origin.position.x < other.position.x ? Face.Left : Face.Right;
        }

        // pre: --
        // post: executes coroutines in order. 
        //      coroutine n+1 waits until coroutine n ends
        public static IEnumerator CoroutineChaining(params IEnumerator[] routines)
        {
            foreach (var item in routines)
            {
                while (item.MoveNext()) yield return item.Current;
            }
        }

        // pre: density >= 1
        // post: throw a burst of rays, if any of this rays touches layer mask returns true, otherwise false
        public static bool LookAround(in Transform origin, in Vector2 dir, float distance, float visualAngle, float density, LayerMask mask)
        {
            float phi = visualAngle / density;
            Function.Look(origin.position, dir, distance, mask);
            int i = 1;
            bool stop = false;
            while (i < density && !stop)
            {
                var r1 = Look(origin.position, Quaternion.Euler(0, 0, (visualAngle - phi * i) / 2) * dir, distance, mask);
                var r2 = Look(origin.position, Quaternion.Euler(0, 0, -(visualAngle - phi * i) / 2) * dir, distance, mask);
                i += 2;
                stop = r1 || r2;
            }
            return stop;
        }

        // pre: pairs > 0
        public static bool Look(in Vector2 origin, in Vector2 dir, float distance, LayerMask mask, float drawTime = 0.2f)
        {
            var ray = Physics2D.Raycast(origin, dir, distance, mask);
            Debug.DrawRay(origin, dir * distance, Color.red, drawTime);
            return ray.collider;
        }

        public static float VerticalExtentsDimention(Collider2D collider)
        {
            return collider.bounds.extents.y;
        }

        //pre: --
        //post: called in FixedUpdate, given a game object and its rotation makes object rotate over time.
        public static void RotateGameObject(Transform GameObjectTransform, float rotationAmount)
        {
            GameObjectTransform.Rotate(Vector3.forward * rotationAmount * Time.fixedDeltaTime);
        }

        // pre --
        // post: kill active tweens in collection
        public static void KillTweensThatStillAlive(List<Tween> tweens)
        {
            if (tweens == null)
                return;

            foreach (Tween tween in tweens)
            {
                if (tween.IsActive())
                    tween.Kill();
            }
        }

        //pre: c != null
        //post: returns c invisible if !makeVisible or visible if makeVisible
        public static Color ColorVisible(bool makeVisible, Color c)
        {
            if (makeVisible)
            {
                c.a = 1f;
            }
            else
            {
                c.a = 0f;
            }
            return c;
        }

    }

}

