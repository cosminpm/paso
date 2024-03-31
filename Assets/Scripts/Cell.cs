using UnityEngine;

public class Cell : MonoBehaviour
{
    private Vector3 _position;
    private int _x;
    private int _y;
    private GameObject _cellObject;
    private Grid.CellType _cellType;


    public void CreateCell(Vector3 position, int x, int y, GameObject cellObject, Grid.CellType cellType)
    {
        _position = position;
        _x = x;
        _y = y;
        _cellObject = cellObject;
        _cellType = cellType;
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