using UnityEngine;
using UnityEngine.InputSystem; // 新Input System

// サイコロ側に付ける
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class HoldDice : MonoBehaviour
{
    [Header("Camera / Anchor")]
    public Camera cam;                 // 未設定なら Start で Camera.main
    public float anchorHeight = 5.0f;  // アンカーの“高さ” (Y固定)
    public float rayMaxDistance = 200f;

    [Header("Spring Joint Tuning")]
    public float spring = 300f;        // 追従の強さ
    public float damper = 30f;         // 減衰（振動抑制）
    public float maxDistance = 0.05f;  // 許容距離（小さいほど吸着）

    // 内部
    Rigidbody rb;
    SpringJoint joint;
    bool dragging;
    Vector3 anchorPos;                 // 毎フレーム更新する“見えないアンカー”のワールド位置

    void Start()
    {
        if (!cam) cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (cam == null) return;

        // 1) マウスを「高さ=anchorHeight の水平面」に投影してアンカー位置を更新
        if (TryGetMouseOnYPlane(anchorHeight, out Vector3 onPlane))
            anchorPos = onPlane;

        // 2) 入力（新 Input System）
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame) TryBeginDrag(mouse);
        if (dragging) UpdateDrag();
        if (mouse.leftButton.wasReleasedThisFrame) EndDrag();
    }

    // マウスからレイ → 高さ一定の水平面と交差
    bool TryGetMouseOnYPlane(float y, out Vector3 hitPos)
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Unity の Plane は法線と距離で定義（上向き、原点からの距離=y）
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

        // クリックが“このサイコロ自身”に当たったかをチェック
        Ray ray = cam.ScreenPointToRay(mouse.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, rayMaxDistance) && hit.rigidbody == rb)
        {
            // クリック地点（自分ローカル）をアンカーにすると吸い付く
            Vector3 localAnchor = rb.transform.InverseTransformPoint(hit.point);

            joint = gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedBody = null;                    // ワールド上の点と接続
            joint.anchor = localAnchor;                    // 自分側アンカー（ローカル）
            joint.connectedAnchor = anchorPos;             // 相手側アンカー（ワールド）
            joint.spring = spring;
            joint.damper = damper;
            joint.maxDistance = maxDistance;

            dragging = true;
        }
    }

    void UpdateDrag()
    {
        if (joint == null) return;

        // 毎フレーム、固定高さのアンカーをマウスに追従
        joint.connectedAnchor = anchorPos;
    }

    void EndDrag()
    {
        dragging = false;
        if (joint) Destroy(joint);
        joint = null;
    }

#if UNITY_EDITOR
    // デバッグ表示（Sceneビューだけで見える）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.2f, 0.7f);
        Gizmos.DrawSphere(anchorPos, 0.02f);
        // 床の高さガイド
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.2f);
        Gizmos.DrawCube(new Vector3(transform.position.x, anchorHeight, transform.position.z),
                        new Vector3(10f, 0.002f, 10f));
    }
#endif
}

