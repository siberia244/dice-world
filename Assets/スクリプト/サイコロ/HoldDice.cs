using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class DragRigidbody3D : MonoBehaviour
{
    public Camera cam;
    public float spring = 100f;
    public float damper = 5f;
    public float maxDistance = 0.2f;

    Rigidbody rb;
    SpringJoint joint;
    bool dragging;

    void Start()
    {
        if (!cam) cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null || cam == null) return;

        if (mouse.leftButton.wasPressedThisFrame) TryBeginDrag(mouse);
        if (dragging) UpdateDrag(mouse);
        if (mouse.leftButton.wasReleasedThisFrame) EndDrag();
    }

    void TryBeginDrag(Mouse mouse)
    {
        Ray ray = cam.ScreenPointToRay(mouse.position.ReadValue());
        if (Physics.Raycast(ray, out var hit, 100f) && hit.rigidbody == rb)
        {
            var attach = new GameObject("DragAnchor");
            attach.transform.position = hit.point;

            joint = gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedBody = null;
            joint.anchor = rb.transform.InverseTransformPoint(hit.point);
            joint.connectedAnchor = attach.transform.position;
            joint.spring = spring;
            joint.damper = damper;
            joint.maxDistance = maxDistance;

            dragging = true;
        }
    }

    void UpdateDrag(Mouse mouse)
    {
        if (joint == null) return;

        Ray ray = cam.ScreenPointToRay(mouse.position.ReadValue());
        if (Physics.Raycast(ray, out var hit, 200f))
            joint.connectedAnchor = hit.point;
        else
            joint.connectedAnchor = ray.GetPoint(5f);
    }

    void EndDrag()
    {
        dragging = false;
        if (joint) Destroy(joint);
        var anchor = GameObject.Find("DragAnchor");
        if (anchor) Destroy(anchor);
    }
}

