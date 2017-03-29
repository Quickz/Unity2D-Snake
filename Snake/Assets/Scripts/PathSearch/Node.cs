using System;
using System.Collections.Generic;
public class Node
{
    public int gridX;
    public int gridY;

    // distance from starting node
    public int gCost;

    // distance from end node
    public int hCost;
    
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    Node nodeParent;
    public Node parent
    {

        set
        {
            // calculating the cost
            // based on parent's path
            gCost = value.gCost + 1;

            // setting parent
            nodeParent = value;
        }

        get
        {
            return nodeParent;
        }

    }

    public List<Node> neighbours;

    public Node(int gridX, int gridY, int[] source, int[] target)
    {
        this.gridX = gridX;
        this.gridY = gridY;

        gCost = 0;
        hCost = getCost(gridX, gridY, target);

    }

    int getCost(int x, int y, int[] target)
    {
        var n1 = Math.Abs(target[0] - x);
        var n2 = Math.Abs(target[1] - y);
        return n1 + n2;
    }

}
