using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dice : MonoBehaviour
{
    public Rigidbody rb;
    private Dictionary<Vector3, int> dices;
    private Vector3 direction = new Vector3(0.0f, 1.0f, 0.0f);//上向き

    public float positionX;
    public bool reset = false;
    public bool stop = false;
    public bool groundcollision = false;
    public int diceValue;
    string myTag;
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        float x = Random.Range(0f, 360f);
        float y = Random.Range(0f, 360f);
        float z = Random.Range(0f, 360f);

        transform.rotation = Quaternion.Euler(x, y, z);

    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.useGravity = true;
        }

        if (!stop && groundcollision && rb.linearVelocity.magnitude < 0.001f)
        {
            dices = new Dictionary<Vector3, int>
            {
                { transform.forward, 1 },
                { transform.up, 2 },
                { -transform.right, 3 },
                { transform.right, 4 },
                { -transform.up, 5 },
                { -transform.forward, 6 }
            };
            stop = true;
            foreach (KeyValuePair<Vector3, int> dice in dices)
            {
                if (Vector3.Angle(dice.Key, direction) < 45f)
                //・Vector3.Angle(a, b) は「aとbの角度差（0〜180度）」を返す
                //・direction は 真上方向
                {
                    diceValue = dice.Value;
                    Debug.Log(diceValue);
                }
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        groundcollision = true;
    }

    public void DiceStart()
    {
        //サイコロの位置の操作
        float positionY = 3f;
        float positionZ = 0f;
        transform.position = new Vector3(positionX, positionY, positionZ);

        //サイコロの傾きの操作
        float rotationX = Random.Range(0f, 360f);
        float rotationY = Random.Range(0f, 360f);
        float rotationZ = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
        //reset = false;
    }

    public bool IsStopped()
    {
        return stop && groundcollision;
    }
}
