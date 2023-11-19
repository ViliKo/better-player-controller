using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerController2D : MonoBehaviour
{
    public Collider2D hit;
    public float hitDistance;
    public LayerMask layerMask;

    // collision
    public struct Collisions
    {
        public bool HorizontalUp;
        public bool HorizontalUpLower;
        public bool HorizontalBottomUp;
        public bool HorizontalBottom;

        public bool VerticalBottom;
    }

    public Collisions collisions;

    private void PerformRaycast(Vector3 origin, Vector2 direction, float length, ref bool collisionFlag, bool debug = true)
    {
        RaycastHit2D hitResult = Physics2D.Raycast(origin, direction, length, layerMask);

        if (hitResult.collider != null)
        {
            collisionFlag = true;
            hit = hitResult.collider;
            hitDistance = hitResult.distance;

            if (debug)
                Debug.DrawRay(origin, direction * length, Color.green);
        }
        else
        {
            collisionFlag = false;

            if (debug)
                Debug.DrawRay(origin, direction * length, Color.red);
        }
    }

    public void VerticalRaycasts(CapsuleCollider2D cc, float extraVerticalHeight)
    {
        Vector3 originPos = new Vector3(cc.bounds.center.x, cc.bounds.center.y, 0);
        float raycastDistance = cc.bounds.extents.y + extraVerticalHeight;

        PerformRaycast(originPos, Vector2.down, raycastDistance, ref collisions.VerticalBottom);
    }

    public void HorizontalRaycasts(float vectorDir, CapsuleCollider2D cc, float length, 
        bool bottomRaycast = false, bool bottomUpRaycast = false, 
        bool upRaycast = false, bool upLowerRaycast = false,
        bool debug = true)
    {
        Vector2 wallraycast = new Vector2(vectorDir, 0);
        

        Vector3 originPosBottom = new Vector3(cc.bounds.center.x, (cc.bounds.center.y - cc.bounds.extents.y), 0);
        if (bottomRaycast)
            PerformRaycast(originPosBottom, wallraycast, cc.bounds.extents.x + length, ref collisions.HorizontalBottom, debug);
        
        Vector3 originPosBottomUp = new Vector3(cc.bounds.center.x, (cc.bounds.center.y - cc.bounds.extents.y) + 0.2f, 0);
        if (bottomUpRaycast)
            PerformRaycast(originPosBottomUp, wallraycast, cc.bounds.extents.x + length, ref collisions.HorizontalBottomUp, debug);

        Vector3 originPosUp = new Vector3(cc.bounds.center.x, (cc.bounds.center.y + cc.bounds.extents.y), 0);
        if (upRaycast)
            PerformRaycast(originPosUp, wallraycast, cc.bounds.extents.x + length, ref collisions.HorizontalUp, debug);

        Vector3 originPosUpLower = new Vector3(cc.bounds.center.x, (cc.bounds.center.y + cc.bounds.extents.y -0.2f), 0);
        if (upLowerRaycast)
            PerformRaycast(originPosUpLower, wallraycast, cc.bounds.extents.x + length, ref collisions.HorizontalUpLower, debug);

    }

    public void Reset()
    {
        collisions.HorizontalBottom = false;
        collisions.HorizontalBottomUp = false;
        collisions.HorizontalUp = false;
        collisions.HorizontalUpLower = false;

        collisions.VerticalBottom = false;
    }
}
