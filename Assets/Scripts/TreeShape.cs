using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.Scripts;

public class TreeShape
{
    public void GenerateTreeWithShape(Shape shape)
    {
	    Tree tree = new Tree();
	    tree.shape = shape;
        switch (shape)
		{
			case Shape.Apple:
				tree.childParentRatio = 0.75f;
				tree.splitAngle =
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
}