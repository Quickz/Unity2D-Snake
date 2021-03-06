﻿using System.Collections;
// for lists
using System.Collections.Generic;
using System.Linq;

public class AStarSearch
{

    public int[,] grid;

	// Use this for initialization
	public AStarSearch(int[,] grid)
    {
        this.grid = grid;
    }

    public List<int[]> run(int[] source, int[] target)
    {
        

        if (source[0] == target[0] && source[1] == target[1])
            return null;
        
        // searching for the path
        Node node = runAlgorithm(source, target);
        
        // formatting the results
        List<int[]> path = findPath(node);

        return path.Count > 1 ? path : null;
    }

    Node runAlgorithm(int[] source, int[] target)
    {
        Node start = new Node(source[0], source[1], source, target);
        
        // discovered nodes to be evaluated
        List<Node> open = new List<Node>();

        // set of nodes already evaluated
        List<Node> closed = new List<Node>();

        open.Add(start);

        Node current;
        while (true)
        {
            // searching for the lowest f cost
            current = lowestCost(open);

            // removing current from open list
            open.Remove(current);

            // adding current to closed list
            closed.Add(current);

            // path found
            if (current.gridX == target[0] &&
                current.gridY == target[1])
            {
                // return the path
                return current;
            }

            // obtaining neighbours
            current.neighbours = getNeighbours(
                current.gridX, current.gridY, source, target
            );

            // looping through neighbours
            //  int lng = current.neighbours.Count;
            foreach (Node currNode in current.neighbours)
            {
                
                // checking if it's in closed list
                if (findIndex(closed, currNode) > -1)
                    continue;
                
                // if shorter path found
                int existing = findIndex(open, currNode);
                if (existing > -1 &&
                    open[existing].parent.fCost >= current.fCost &&
                    open[existing].parent.gCost > current.gCost)
                {

                    // removing old (bad) node
                    open.Remove(open[existing]);

                }
                
                // not in open
                if (findIndex(open, currNode) < 0)
                {
                    currNode.parent = current;

                    // adding neighbour to open nodes
                    open.Add(currNode);

                }

            }

            // no path found
            if (open.Count == 0)
                return current;

        }

    }

    List<int[]> findPath(Node node)
    {
        List<int[]> path = new List<int[]>();
        while (node != null)
        {
            path.Add(new int[] { node.gridX, node.gridY });
            node = node.parent;
        }
        path.Reverse();
        return path;
    }

    public void ClearGrid()
    {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
                grid[i, j] = 0;
        }
    }

    Node lowestCost(List<Node> nodes)
    {
         Node node = nodes.Aggregate(
             (x, y) => x.fCost <= y.fCost &&
                       x.hCost <  y.hCost ? x : y
         );
         return node;
    }

    int findIndex(List<Node> nodes, Node node)
    {
        foreach (Node curr in nodes)
        {
            if (curr.gridX == node.gridX &&
                curr.gridY == node.gridY)
                return nodes.IndexOf(curr);
        }
        return -1;
    }

    bool validCoord (int x, int y, int[] target)
    {
        // 2 is used to exlude the border
               // inside bounds
        return x >= 2 && x < grid.GetLength(0) - 2 &&
               y >= 2 && y < grid.GetLength(1) - 2 &&
               // walkable
               grid[x, y] != 1 ||
               // targets can't be an obstacle
               target[0] == x && target[1] == y;
    }

    List<Node> getNeighbours(int x, int y, int[] source, int[] target)
    {
        List<Node> neighbours = new List<Node>();

        // top
        if (this.validCoord(x, y - 1, target))
            neighbours.Add(new Node(x, y - 1, source, target));
        // right
        if (this.validCoord(x + 1, y, target))
            neighbours.Add(new Node(x + 1, y, source, target));
        // bottom
        if (this.validCoord(x, y + 1, target))
            neighbours.Add(new Node(x, y + 1, source, target));
        // left
        if (this.validCoord(x - 1, y, target))
            neighbours.Add(new Node(x - 1, y, source, target));

        return neighbours;
    }

}
