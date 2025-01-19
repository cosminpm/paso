
using UnityEngine;

public class FollowPlayerCamera : MonoBehaviour
{
    private Camera _camera;
    public float distanceFromGround;
    
    private Vector3 CalculateMiddleMap(float rows, float columns, float sizeSquare)
    {
        float middleRows = rows * sizeSquare / 2.0f;
        float middleColums = columns * sizeSquare / 2.0f;
        return new Vector3(middleRows, distanceFromGround, middleColums);
    }

    public void SetCameraMiddleMap(int rows, int columns, float sizeSquare)
    {
        Vector3 center = CalculateMiddleMap(rows, columns, sizeSquare);
        transform.position = center;
    }
}