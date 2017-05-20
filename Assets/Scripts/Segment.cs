using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Segment
{
    public Vector3 bottom;
    public Vector3 top;
    public float bottomRadius;
    public float topRadius;
    public float length;
    public Quaternion bottomRotation;
    public Quaternion topRotation;
    public List<Branch> childBranches;
    public int numberOfSectors;
    public int bottomOffset;
    public int topOffset;

    public List<int> indices;
    public List<Vector3> bottomVertices;
    public List<Vector3> topVertices;

    public Segment parent;


    public void GenerateSegment()
    {
        bottomVertices = new List<Vector3>();
        topVertices = new List<Vector3>();
        childBranches = new List<Branch>();
        indices = new List<int>();
        top = bottom + topRotation * new Vector3(0, length, 0);
    }

    public void GenerateSegmentMeshWithSplits()
    {
        for (int i = 0; i < numberOfSectors; i++)
        {
            Vector3 bottomVerticeBase = new Vector3((float) (bottomRadius * Math.Sin(i * 2 * Math.PI / numberOfSectors)), 0, (float) (bottomRadius * Math.Cos(i * 2 * Math.PI / numberOfSectors)));
            Vector3 bottomVertice = bottom + bottomRotation * bottomVerticeBase;

            bottomVertices.Add(bottomVertice);
        }
        for (int i = 0; i < numberOfSectors; i++)
        {
            Vector3 topVerticeBase = new Vector3((float) (topRadius * Math.Sin(i * 2 * Math.PI / numberOfSectors)), length, (float) (topRadius * Math.Cos(i * 2 * Math.PI / numberOfSectors)));
            Vector3 topVertice = bottom + topRotation * topVerticeBase;

            topVertices.Add(topVertice);
        }

        for (int i = 0; i < numberOfSectors-1; i++)
        {
            indices.Add(i + bottomOffset);
            indices.Add(numberOfSectors + i + 1 + topOffset);
            indices.Add(numberOfSectors + i + topOffset);
            indices.Add(i + bottomOffset);
            indices.Add(i + 1 + bottomOffset);
            indices.Add(numberOfSectors + i + 1 + topOffset);
        }

        indices.Add(numberOfSectors-1 + bottomOffset);
        indices.Add(numberOfSectors + topOffset);
        indices.Add(2 * numberOfSectors-1 + topOffset);
        indices.Add(numberOfSectors-1 + bottomOffset);
        indices.Add(0 + bottomOffset);
        indices.Add(numberOfSectors + topOffset);
    }
}
