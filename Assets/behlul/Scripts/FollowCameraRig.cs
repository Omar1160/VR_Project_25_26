using UnityEngine;

public class FollowCameraRig : MonoBehaviour
{
    public Transform rig; // OVRCameraRig or CenterEyeAnchor parent

    void Update()
    {
        Vector3 pos = rig.position;
        transform.position = pos;
        Debug.Log(rig.position);
    }
}
