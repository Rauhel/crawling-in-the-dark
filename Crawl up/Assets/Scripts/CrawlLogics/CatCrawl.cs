using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatCrawl : MonoBehaviour
{
    private HashSet<KeyCode> leftKeys = new HashSet<KeyCode> { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B };
    private HashSet<KeyCode> rightKeys = new HashSet<KeyCode> { KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P, KeyCode.N, KeyCode.M };

    private int leftKeyCount = 0;
    private int rightKeyCount = 0;
    private bool hasPressedLeft = false;
    private bool hasPressedRight = false;
    private bool isMovingRight = true;

    // Update is called once per frame
    void Update()
    {
        int currentLeftKeyCount = 0;
        int currentRightKeyCount = 0;

        foreach (KeyCode key in leftKeys)
        {
            if (Input.GetKey(key))
            {
                currentLeftKeyCount++;
            }
        }

        foreach (KeyCode key in rightKeys)
        {
            if (Input.GetKey(key))
            {
                currentRightKeyCount++;
            }
        }

        if (currentLeftKeyCount > 5 && currentRightKeyCount > 5)
        {
            return; // Both sides pressed more than 5 keys, do nothing
        }

        if (currentLeftKeyCount > 5)
        {
            if (hasPressedLeft)
            {
                return; // Left side already pressed more than 5 keys before, do nothing
            }
            leftKeyCount++;
            hasPressedLeft = true;
        }

        if (currentRightKeyCount > 5)
        {
            if (hasPressedRight)
            {
                return; // Right side already pressed more than 5 keys before, do nothing
            }
            rightKeyCount++;
            hasPressedRight = true;
        }

        if (hasPressedLeft && hasPressedRight)
        {
            MoveForward();
            leftKeyCount = 0;
            rightKeyCount = 0;
            hasPressedLeft = false;
            hasPressedRight = false;
        }
    }

    void MoveForward()
    {
        if (isMovingRight)
        {
            // Implement the logic to move the character to the right
            transform.Translate(Vector3.right * 0.1f);
            Debug.Log("Moving right");
        }
        else
        {
            // Implement the logic to move the character to the left
            transform.Translate(Vector3.left * 0.1f);
            Debug.Log("Moving left");
        }
    }
}