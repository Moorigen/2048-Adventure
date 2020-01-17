using System;
using UnityEngine;

namespace UnityStandardAssets.Utility {
    public class FollowTarget : MonoBehaviour {
        public Transform target;
        public Vector3 offset = new Vector3(0f, 7.5f, 0f);
        public float smoothSpeed = 0.125f;

        Vector3 velocity = Vector3.zero;

        void LateUpdate() {
            Vector3 desiredPos = target.position + offset;
            Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothSpeed);
            transform.position = smoothedPos;
        }
    }
}
