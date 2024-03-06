using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astronaut : MonoBehaviour
{
    
    public float rotateSpeed = 100f; // The speed at which the player will rotate
    private int _x, _y;
    

    public Vector3 GetPosition()
    {
        return transform.position;
    }
    
    public void InstantiateAstronaut(int x, int y, Vector3 position)
    {
        _x = x;
        _y = y;
        transform.position = position;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetX(int x)
    {
        _x = x;
    }

    public void SetY(int y)
    {
        _y = y;
    }

    
    
    
    public void SetAllPositionAstronaut(int x, int y, Vector3 position)
    {
        SetX(x);
        SetY(y);
        SetPosition(position);
    }
    
    public int GetX()
    {
        return _x;
    }

    public int GetY()
    {
        return _y;
    }
    
    public void RotatePlayer(Vector3 direction)
    {
        // Calculate the desired rotation based on the specified direction
        Quaternion desiredRotation = Quaternion.identity;
        if (direction == Vector3.forward)
            desiredRotation = Quaternion.Euler(0, 0, 0);
        else if (direction == Vector3.back)
            desiredRotation = Quaternion.Euler(0, 180, 0);
        else if (direction == Vector3.right)
            desiredRotation = Quaternion.Euler(0, 90, 0);
        else if (direction == Vector3.left)
            desiredRotation = Quaternion.Euler(0, -90, 0);
    
        // Set the player's rotation to the desired rotation
        transform.rotation = desiredRotation;
    }
    
}
