using System.Collections.Generic;
using UnityEngine;

public class BendyBonesController : MonoBehaviour
{
    [SerializeField] private Transform chainParent;
    public float stiffness = 0.5f;

    private List<Transform> bones;
    private List<float> boneLengths;
    void Start()
    {
        bones = new List<Transform>();
        boneLengths = new List<float>();

        // Cashe All Bones.
        for (int i = 0; i < chainParent.childCount; i++)
        {
            bones.Add(chainParent.GetChild(i));
        }

        // Initialize all bone lengths.
        for (int i = 0; i < bones.Count - 1; i++)
        {
            boneLengths.Add(Vector3.Distance(bones[i].position, bones[i + 1].position));
        }
        boneLengths.Add(0);
    }
    void Update()
    {
        for (int i = 1; i < bones.Count; i++)
        {
            // Rotate bone towards previous' bone end.
            if (GetBoneEnd(i - 1) != bones[i].position)
            {
                Quaternion target = Quaternion.LookRotation(GetBoneEnd(i - 1) - GetBoneEnd(i), Vector3.up);
                bones[i].rotation = target;
            }

            // Rotate to match previous' bone rotation. This gives bones their stiffness.
            bones[i].rotation = Quaternion.Lerp(bones[i].rotation, bones[i - 1].rotation, stiffness);

            // Make sure that tip of the bone is where previous ends. Also ensures conservation of length.
            bones[i].position += GetBoneEnd(i - 1) - bones[i].position;
        }
    }

    // Calculate End of the bone by moving along it's -forward by the length of the bone.
    private Vector3 GetBoneEnd(int boneIndex)
    {
        return bones[boneIndex].position - bones[boneIndex].forward * boneLengths[boneIndex];
    }

    // Drawing Debug Info.
    void OnDrawGizmos()
    {
        if (bones == null) return;
        for (int i = 1; i < bones.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GetBoneEnd(i - 1), 0.03f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(bones[i].position, 0.05f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(bones[i].position, GetBoneEnd(i - 1));
        }
    }
}