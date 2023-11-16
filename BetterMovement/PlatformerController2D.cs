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
        public bool HorizontalBottomUp;
        public bool HorizontalBottom;
        public bool VerticalBottom;
    }

    public Collisions collisions;


    public void VerticalRaycasts(CapsuleCollider2D cc, float extraVerticalHeight)
    {
        // Cast a ray straight down.

        Vector3 originPos = new Vector3(cc.bounds.center.x, cc.bounds.center.y, 0);
        float raycastDistance = cc.bounds.extents.y + extraVerticalHeight;

        RaycastHit2D hit = Physics2D.Raycast(originPos, Vector2.down, raycastDistance, layerMask);


        if (hit.collider != null)
        {
            //Debug.Log("imfucinghererwehat");
            Debug.DrawRay(originPos, new Vector2(0, Vector2.down.y * raycastDistance), Color.green);
            collisions.VerticalBottom = true;
        }

        else
        {
            collisions.VerticalBottom = false;
            Debug.DrawRay(originPos, new Vector3(0, Vector2.down.y * raycastDistance), Color.red);
        }



    }  // function



    public void HorizontalRaycasts(float vectorDir, CapsuleCollider2D cc, float length, bool bottomRaycast = true, bool bottomUpRaycast = true, bool debug = true)
    {
  
        Vector2 wallraycast = new Vector2(vectorDir, 0);
        Vector3 originPosBottom = new Vector3(cc.bounds.center.x, (cc.bounds.center.y - cc.bounds.extents.y), 0);
        Vector3 originPosBottomUp = new Vector3(cc.bounds.center.x, (cc.bounds.center.y - cc.bounds.extents.y)+.2f, 0);
        Vector3 direction = new Vector3(cc.bounds.extents.x * wallraycast.x + length * wallraycast.x, 0, 0);

        if (bottomRaycast)
        {
            RaycastHit2D hitBottom = Physics2D.Raycast(
                originPosBottom,
                wallraycast,
                cc.bounds.extents.x + length,
                layerMask);

            if (hitBottom.collider != null)
            {
                collisions.HorizontalBottom = true;
                hit = hitBottom.collider;
                hitDistance = hitBottom.distance;

                if (debug)
                    Debug.DrawRay(originPosBottom, direction, Color.green);
            }
            else
            {
                collisions.HorizontalBottom = false;

                if (debug)
                    Debug.DrawRay(originPosBottom, direction, Color.red);
            }
                    
        }

        if (bottomUpRaycast)
        {
            RaycastHit2D hitBottomUp = Physics2D.Raycast(
                originPosBottomUp,
                wallraycast,
                cc.bounds.extents.x + length,
                layerMask);

            if (hitBottomUp.collider != null)
            {
                collisions.HorizontalBottomUp = true;
                hit = hitBottomUp.collider;
                hitDistance = hitBottomUp.distance;
                if (debug)
                    Debug.DrawRay(originPosBottomUp, direction, Color.green);
            }
            else
            {
                collisions.HorizontalBottomUp = false;
                if (debug)
                    Debug.DrawRay(originPosBottomUp, direction, Color.red);
            }
                
        }

    } // function
}
