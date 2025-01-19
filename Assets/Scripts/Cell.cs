using UnityEngine;

public class Cell : MonoBehaviour
{
    private Vector3 _position;
    private int _x;
    private int _y;


    public void CreateCell(Vector3 position, int x, int y)
    {
        _position = position;
        _x = x;
        _y = y;
    }

    public int GetX()
    {
        return _x;
    }

    public int GetY()
    {
        return _y;
    }

    public Vector3 GetPosition()
    {
        return _position;
    }
}