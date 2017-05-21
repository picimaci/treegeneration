using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Leaf
{
    public Vector3 basePoint;
    public Quaternion baseRotation;
    public float width;
    public float length;

    public List<Vector3> vertices;
    public List<int> indices;
    public List<Vector2> uvs;

    public void GenerateLeaf()
    {
        vertices = new List<Vector3>();
        indices = new List<int>();
        uvs = new List<Vector2>();

        Vector3 bottomLeft = basePoint + baseRotation * new Vector3(0,0,- width / 2);
        Vector3 bottomRight = basePoint + baseRotation * new Vector3(0,0, width / 2);
        Vector3 topLeft = basePoint + baseRotation * new Vector3(0,length,- width / 2);
        Vector3 topRight = basePoint + baseRotation * new Vector3(0,length, width / 2);

        vertices.Add(bottomRight);
        vertices.Add(bottomLeft);
        vertices.Add(topRight);
        vertices.Add(topLeft);

        indices.Add(0);
        indices.Add(1);
        indices.Add(3);
        indices.Add(0);
        indices.Add(3);
        indices.Add(2);

        uvs.Add(new Vector2(1,0));
        uvs.Add(new Vector2(0,0));
        uvs.Add(new Vector2(1,1));
        uvs.Add(new Vector2(0,1));
    }
}