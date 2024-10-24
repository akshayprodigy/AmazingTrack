using UnityEngine;

namespace AmazingTrack
{
    public struct BallComponent
    {
        public int? PreviousHitEntity;
        public Vector3 Direction;
        public float Speed;
        public Vector3 LastFallingDirection;
    }
}