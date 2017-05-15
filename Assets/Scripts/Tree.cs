using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.Scripts;

public class Tree
{
    public System.Random random;

    public int recursionDepth;
    public Shape shape;
    public Vector3 basePoint;
    public float widthLengthRatio;
    public float childParentRatio;

    public Trunk trunk;

    public List<Vector3> vertices;
    public List<int> indices;

    public void SetTreeShape(Shape s)
    {
        shape = s;
        switch (shape)
        {
            case Shape.Apple:
                childParentRatio = 0.75f;
                recursionDepth = 2;
                break;
            case Shape.Palm:
                break;
            case Shape.Pine:
                break;
            case Shape.Willow:
                break;
            default:
                Debug.Log("Tree Shape does not exist");
                break;
        }
    }

    public void GenerateForTesting()
    {
        random = new System.Random();
        childParentRatio = 0.8f;
        GenerateTree();
    }

    public void GenerateTree()
    {
        vertices = new List<Vector3>();
        indices = new List<int>();
        trunk = new Trunk();
        trunk.tree = this;
        trunk.GenerateForTesting();
        GenerateTreeMesh();
    }

    public void GenerateTreeUsingEditorParams()
    {
        vertices = new List<Vector3>();
        indices = new List<int>();

        trunk.GenerateTrunk();
        GenerateTreeMesh();
    }

    public void GenerateTreeMesh()
    {
        GenerateMeshRecursively(trunk);
    }

    public void GenerateMeshRecursively(Stem stem)
    {
        int offset = vertices.Count;
        foreach (var vertice in stem.vertices)
        {
            vertices.Add(vertice);
        }
        foreach (var indice in stem.indices)
        {
            indices.Add(offset + indice);
        }
        foreach (var segment in stem.segments)
        {
            foreach (var branch in segment.branchList)
            {
                GenerateMeshRecursively(branch);
            }
        }
    }
}
