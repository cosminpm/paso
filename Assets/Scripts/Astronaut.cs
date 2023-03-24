using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astronaut : MonoBehaviour
{
    private int _x, _y;

    // Update is called once per frame
    void Update()
    {
        
    }

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
    
    
}
