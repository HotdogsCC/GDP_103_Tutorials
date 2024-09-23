using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum LegStates
{
    NONE = 0,
    ON_GROUND,
    IN_AIR
}

public class LegScript : MonoBehaviour
{
    LegStates legState = LegStates.NONE;
    // Start is called before the first frame update
    public Transform shoulderJoint;
    public Transform kneeJoint;
    public Transform ankleJoint;
    public Transform IKTarget;
    float upperLength;
    float lowerLength;
    float totalLength;

    public LayerMask grondLayer;
    public float StrideLength;
    Vector3 vAnchorPoint;
    Vector3 vAttachmentNormal;
    Vector3 OldAnchorPoint;
    Vector3 OldtransformPosition;
    public float stepFowardSpeed = 3.0f;
    float stepHeight = 0.5f;
    public float startOffset;

    void Start()
    {
        Debug.Log("Setting up Leg");
        if (shoulderJoint == null)
        {
            Debug.LogError("Shoulder joint needs initializing");
            return;
        }
        if (kneeJoint == null)
        {
            Debug.LogError("Shoulder joint needs initializing");
            return;
        }
        if (IKTarget == null)
        {
            Debug.LogError("Target needs initializing");
            return;
        }
        upperLength = Vector3.Distance(shoulderJoint.position, kneeJoint.position);
        lowerLength = Vector3.Distance(kneeJoint.position, ankleJoint.position);
        totalLength = upperLength + lowerLength;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        switch (legState)
        {
            case LegStates.NONE:
                if(findNextAttachmentPoint(startOffset * StrideLength / 2))
                {
                    legState = LegStates.ON_GROUND;
                }
            break;
            case LegStates.ON_GROUND:
                IK(vAnchorPoint);
                ankleJoint.transform.up = vAttachmentNormal;
                ankleJoint.transform.forward = transform.forward;
                Ray ray = new Ray(transform.position, -transform.up);
                const float MAX_RAY_LENGTH = 4;
                RaycastHit hit;
                // Find the dot product between the forward vector and the displacement vector from 
                // the shoulder to the leg attachment, if it’s negative the leg is behind the body

                float dot = Vector3.Dot(transform.forward, (vAnchorPoint - transform.position));
                // find the ponit on the ground imediately below the should joint
                if (Physics.Raycast(ray, out hit, MAX_RAY_LENGTH, grondLayer) && (dot < 0.0f))
                {
                    Vector3 groundPoint = hit.point;
                    float distance = Vector3.Distance(groundPoint, vAnchorPoint);
                    if(distance > StrideLength / 2)
                    {
                        OldAnchorPoint = vAnchorPoint;
                        OldtransformPosition = transform.position;
                        findNextAttachmentPoint(StrideLength * 0.5f);
                        legState = LegStates.IN_AIR;
                    }
                }
            break;
            case LegStates.IN_AIR:
                // Work out how far through the forward step we are
                float StepForwardFraction = Vector3.Distance(OldtransformPosition, transform.position) * stepFowardSpeed;
                // Use lerp to work out the X and Z values for the foot position
                Vector3 inAirPos = Vector3.Lerp(OldAnchorPoint, vAnchorPoint, StepForwardFraction);
                // Add in a value using sine to raise and lower the foot correctly
                inAirPos.y += Mathf.Sin(StepForwardFraction * Mathf.PI) * stepHeight;
                // move the foot to the correct position
                IK(inAirPos);
                // If we have reached the end of the forward step then set the state to Grouned
                if(StepForwardFraction>=1)
                {
                    legState = LegStates.ON_GROUND;
                }
            break;
        }
    }

    bool findNextAttachmentPoint(float scale)
    {
        // find attachment Point
        // Calculate ray
        Ray ray = new Ray(transform.position + (transform.forward * scale), -transform.up);
        const float MAX_RAY_LENGTH = 4;
        Debug.DrawLine(transform.position + (transform.forward * scale), transform.position + (transform.forward * scale) -(transform.up * MAX_RAY_LENGTH), Color.white);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, MAX_RAY_LENGTH, grondLayer))
        {
            vAnchorPoint = hit.point;
            vAttachmentNormal = hit.normal;
            return true;
        }
        return false;
    }

    void IK(Vector3 vIKTargetPoint)
    {
        float distance = Vector3.Distance(transform.position, vIKTargetPoint);
        if(distance > totalLength)
        {
            // If target is beyond the reach of the leg then don't move it
            Debug.DrawLine(transform.position, vIKTargetPoint, Color.red);
        }
        else
        {
            Debug.DrawLine(transform.position, vIKTargetPoint, Color.green);
            // FInd the mid point
            Vector3 midPoint = (shoulderJoint.position + vIKTargetPoint) / 2.0f;
            Vector3 displacementVector = (vIKTargetPoint - shoulderJoint.position).normalized;
            // Find the tangent
            Vector3 tangent = new Vector3(displacementVector.x, displacementVector.z, -displacementVector.y);
            Debug.DrawLine(midPoint, midPoint + tangent, Color.blue);
            // Use pythagoras to calculate the distance along the tangent to the knee joint
            float tangentLength = Mathf.Sqrt(Mathf.Pow(upperLength, 2) - Mathf.Pow(distance / 2.0f, 2));
            Vector3 kneePostion = midPoint + (tangent * tangentLength);
            Debug.DrawLine(transform.position, kneePostion, Color.yellow);
            Debug.DrawLine(kneePostion, vIKTargetPoint, Color.yellow);
            // get the displacement vector from the shoulder to the knee
            Vector3 kneeDisplacement = shoulderJoint.position - kneePostion;
            // convert our vector into an angle
            float shoulderAngle = Mathf.Atan2(kneeDisplacement.z, kneeDisplacement.y);
            shoulderJoint.localEulerAngles = new Vector3(shoulderAngle * Mathf.Rad2Deg, 0, 0);
            // Do the same thing for the knee
            Vector3 ankleDisplacement = kneePostion - vIKTargetPoint;
            float kneeAngle = Mathf.Atan2(ankleDisplacement.z, ankleDisplacement.y);
            kneeJoint.localEulerAngles = new Vector3((kneeAngle - shoulderAngle) * Mathf.Rad2Deg, 0, 0);

        }
    }
}