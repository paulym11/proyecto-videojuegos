using UnityEngine;
using System.Collections;

namespace scgFullBodyController
{
    public class DontGoThroughThings : MonoBehaviour
    {
        
        public bool sendTriggerMessage = false;

        public LayerMask layerMask = -1; 
        public float skinWidth = 0.1f; 

        private float minimumExtent;
        private float partialExtent;
        private float sqrMinimumExtent;
        private Vector3 previousPosition;
        private Rigidbody myRigidbody;
        private Collider myCollider;

        
        void Start()
        {
            myRigidbody = GetComponent<Rigidbody>();
            myCollider = GetComponent<Collider>();
            previousPosition = myRigidbody.position;
            minimumExtent = Mathf.Min(Mathf.Min(myCollider.bounds.extents.x, myCollider.bounds.extents.y), myCollider.bounds.extents.z);
            partialExtent = minimumExtent * (1.0f - skinWidth);
            sqrMinimumExtent = minimumExtent * minimumExtent;
        }

        void FixedUpdate()
        {
            
            Vector3 movementThisStep = myRigidbody.position - previousPosition;
            float movementSqrMagnitude = movementThisStep.sqrMagnitude;

            if (movementSqrMagnitude > sqrMinimumExtent)
            {
                float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
                RaycastHit hitInfo;

                
                if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, layerMask.value))
                {
                    if (!hitInfo.collider)
                        return;

                    if (hitInfo.collider.isTrigger)
                        hitInfo.collider.SendMessage("OnTriggerEnter", myCollider);

                    if (!hitInfo.collider.isTrigger)
                        myRigidbody.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;

                }
            }

            previousPosition = myRigidbody.position;
        }
    }
}