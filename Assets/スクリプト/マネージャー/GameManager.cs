using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

//GitKraken�ŊǗ��J�n
public class GameManager : MonoBehaviour
{

    public Dice diceA;
    public Dice diceB;
    public Dice diceC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        diceA.DiceStart();
        diceB.DiceStart();
        diceC.DiceStart();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
