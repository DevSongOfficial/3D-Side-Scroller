using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    // Ground Check
    private const float groundCheckDistance = 1.05f;
    [SerializeField] private Transform groundCheckStartPoint;
    private readonly float PlayerColliderLength = 0.4f;
    public bool IsOnGround
    {
        get
        {
            bool groundLeft = Physics.Raycast(groundCheckStartPoint.position + new Vector3(PlayerColliderLength / 2f, 0, 0),
                Vector3.down, out RaycastHit hitLeft, groundCheckDistance);
            bool groundRight = Physics.Raycast(groundCheckStartPoint.position - new Vector3(PlayerColliderLength / 2f, 0, 0),
                Vector3.down, out RaycastHit hitRight, groundCheckDistance);

            return groundLeft || groundRight;
        }
    }
}