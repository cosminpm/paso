using System;
using System.Collections.Generic;
using DefaultNamespace.LevelTypes;
using UnityEngine;

public class MaximizeCellLevel : LevelCell
{
    private List<int[]> _longestPathListCells;

    public MaximizeCellLevel(Grid grid) : base(grid)
    {
    }

    public override bool EndCondition()
    {
        return true;
    }

    public override void GridSpecificLevel()
    {
        grid.SetRandomLimiterPerlinNoise(.5f, .75f);
        grid.SetRandomScaler(2f, 10f);
        grid.SetRandomColumnsAndRows(8, 10);
    }

    public override void CreateLevelSpecific()
    {
        int[] endPos = GetFinalCellInMaximize();
        grid.CreateFinalCellPosition(endPos);
        TransformSurroundingIntoAvailableCells(endPos);
    }

    int[] GetFinalCellInMaximize()
    {
        int[] endPos = null;

        List<int[]> positionsEnd = new List<int[]>();

        positionsEnd.Add(new int[] {grid.startingPosition[0] + 3, grid.startingPosition[1]});
        positionsEnd.Add(new int[] {grid.startingPosition[0] - 3, grid.startingPosition[1]});
        positionsEnd.Add(new int[] {grid.startingPosition[0], grid.startingPosition[1] + 3});
        positionsEnd.Add(new int[] {grid.startingPosition[0], grid.startingPosition[1] - 3});

        foreach (var pos in positionsEnd)
        {
            if (grid.IsPositionAvailable(pos))
            {
                endPos = pos;
                break;
            }
        }


        return endPos;
    }

    public void TransformSurroundingIntoAvailableCells(int[] endPos)
    {
        for (int i = endPos[0] - 3; i < endPos[0] + 3; i++)
        {
            for (int j = endPos[1] - 3; j < endPos[1] + 3; j++)
            {
                if ((grid.emptyArrIntsHashSet.Contains(new[] {i, j}) || grid.poisonArrIntHashSet.Contains(new[] {i, j})) &&
                    !(i == grid.startingPosition[0] &&
                      j == grid.startingPosition[1]) &&
                    !(i == endPos[0] &&
                      j == endPos[1]))
                {
                    grid.poisonArrIntHashSet.Remove(new[] {i, j});
                    grid.emptyArrIntsHashSet.Remove(new[] {i, j});
                    grid.TransformIntoDesert(i, j);


                }
            }
        }
    }
}