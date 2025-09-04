using UnityEngine;
using UnityEngine.InputSystem; // �VInput System

// �T�C�R�����ɕt����
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class HoldDice : MonoBehaviour
{
    [Header("Camera / Anchor")]
    public Camera cam;                 // ���ݒ�Ȃ� Start �� Camera.main
    public float anchorHeight = 5.0f;  // �A���J�[�́g�����h (Y�Œ�)
    public float rayMaxDistance = 200f;

    [Header("Spring Joint Tuning")]
    public float spring = 300f;        // �Ǐ]�̋���
    public float damper = 30f;         // �����i�U���}���j
    public float maxDistance = 0.05f;  // ���e�����i�������قǋz���j

    // ����
    Rigidbody rb;
    SpringJoint joint;
    bool dragging;
    Vector3 anchorPos;                 // ���t���[���X�V����g�����Ȃ��A���J�[�h�̃��[���h�ʒu

    void Start()
    {
        if (!cam) cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (cam == null) return;

        // 1) �}�E�X���u����=anchorHeight �̐����ʁv�ɓ��e���ăA���J�[�ʒu���X�V
        if (TryGetMouseOnYPlane(anchorHeight, out Vector3 onPlane))
            anchorPos = onPlane;

        // 2) ���́i�V Input System�j
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame) TryBeginDrag(mouse);
        if (dragging) UpdateDrag();
        if (mouse.leftButton.wasReleasedThisFrame) EndDrag();
    }

    // �}�E�X���烌�C �� �������̐����ʂƌ���
    bool TryGetMouseOnYPlane(float y, out Vector3 hitPos)
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Unity �� Plane �͖@���Ƌ����Œ�`�i������A���_����̋���=y�j
        Plane plane = new Plane(Vector3.up, new Vector3(0f, y, 0f));
        if (plane.Raycast(ray, out float enter))
        {
            hitPos = ray.GetPoint(enter);
            return true;
        }
        hitPos = Vector3.zero;
        return false;
    }

    void TryBeginDrag(Mouse mouse)
    {
        if (dragging) return;

        // �N���b�N���g���̃T�C�R�����g�h�ɓ������������`�F�b�N
        Ray ray = cam.ScreenPointToRay(mouse.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, rayMaxDistance) && hit.rigidbody == rb)
        {
            // �N���b�N�n�_�i�������[�J���j���A���J�[�ɂ���Ƌz���t��
            Vector3 localAnchor = rb.transform.InverseTransformPoint(hit.point);

            joint = gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedBody = null;                    // ���[���h��̓_�Ɛڑ�
            joint.anchor = localAnchor;                    // �������A���J�[�i���[�J���j
            joint.connectedAnchor = anchorPos;             // ���葤�A���J�[�i���[���h�j
            joint.spring = spring;
            joint.damper = damper;
            joint.maxDistance = maxDistance;

            dragging = true;
        }
    }

    void UpdateDrag()
    {
        if (joint == null) return;

        // ���t���[���A�Œ荂���̃A���J�[���}�E�X�ɒǏ]
        joint.connectedAnchor = anchorPos;
    }

    void EndDrag()
    {
        dragging = false;
        if (joint) Destroy(joint);
        joint = null;
    }

#if UNITY_EDITOR
    // �f�o�b�O�\���iScene�r���[�����Ō�����j
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.2f, 0.7f);
        Gizmos.DrawSphere(anchorPos, 0.02f);
        // ���̍����K�C�h
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.2f);
        Gizmos.DrawCube(new Vector3(transform.position.x, anchorHeight, transform.position.z),
                        new Vector3(10f, 0.002f, 10f));
    }
#endif
}

