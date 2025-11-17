using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

public class ABhand : MonoBehaviour
{
    public enum Handedness
    {
        Left,
        Right
    }
    [System.Serializable]
    public struct JointToTransformReference
    {
        public XRHandJointID jointID;
        public Transform jointTransform;
        public ArticulationBody articulationBody;
    }

    public Vector3 offsetEuler = new Vector3(0, 90f, 0); // Adjust Y offset as needed

    public Handedness handedness;
    public List<JointToTransformReference> m_JointTransformReferences;

    public void Update()
    {
        foreach (var jointReference in m_JointTransformReferences)
        {
            if (jointReference.articulationBody.isRoot)
            {
                Quaternion offsetRotation = Quaternion.Euler(offsetEuler);
                Quaternion finalRotation = jointReference.jointTransform.rotation * offsetRotation;
                jointReference.articulationBody.TeleportRoot(jointReference.jointTransform.position, finalRotation);
            }
            else if (jointReference.jointTransform != null && jointReference.articulationBody != null)
            {
                // Convert quaternion to angle around X-axis using atan2 for proper range [-180, 180]
                Quaternion localRot = jointReference.jointTransform.localRotation;
                float xAngle = Mathf.Atan2(2.0f * (localRot.w * localRot.x + localRot.y * localRot.z), 
                                          1.0f - 2.0f * (localRot.x * localRot.x + localRot.y * localRot.y)) * Mathf.Rad2Deg;
                jointReference.articulationBody.SetDriveTarget(ArticulationDriveAxis.X, xAngle);
            }
        }
    }

}
