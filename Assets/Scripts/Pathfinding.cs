using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    public GameObject nodePublic;
    public class pNode
    {
        public GameObject obj;
        public bool[] connections;
        //public pNode[] connectedNodes;
        public int F, G, H, X, Y;
        public pNode parent;
        public bool removed;
    }
    pNode[,] nodes;

    int nodeRows, nodeCols;

    Vector3[] dirs;

    pNode targetNode;

    float minDist;

	// Use this for initialization
	void Start ()
    {
        nodes = new pNode[100, 100];

       

        nodeRows = 12;
        nodeCols = 12;

        minDist = 2.0f;

        dirs = new Vector3[8];
        dirs[0] = new Vector3( 0,  1,  1);
        dirs[1] = new Vector3( 1,  1,  1);
        dirs[2] = new Vector3( 1,  0,  1);
        dirs[3] = new Vector3( 1, -1,  1);
        dirs[4] = new Vector3( 0, -1,  1);
        dirs[5] = new Vector3(-1, -1,  1);
        dirs[6] = new Vector3(-1,  0,  1);
        dirs[7] = new Vector3(-1,  1,  1);

        
	}

    public List<Vector3> FindPath(Vector3 playerPosition, Vector3 targetPosition)
    {
        CreateGrid();
        CastRays();
        CullNodes();
        return CalculatePath(playerPosition, targetPosition);
    }

    public void Test()
    {
        CreateGrid();
        CastRays();
        CullNodes();
    }

    void CreateGrid()
    {
        for (int i = 0; i < nodeRows; i++)
        {
            for (int j = 0; j < nodeCols; j++)
            {
                int offSet = 0;
                if(j % 2 != 0)
                {
                    //offSet = 1;
                }
                nodes[i, j] = new pNode();
                nodes[i, j].connections = new bool[8];

                nodes[i, j].obj = Instantiate(nodePublic, new Vector3(2 * i + offSet - 12, 1 * j - 7, 1), Quaternion.identity);
                nodes[i, j].X = i;
                nodes[i, j].Y = j;
                //nodes[i, j].connectedNodes = new pNode[8];
                nodes[i, j].removed = false;
               
                
            }
        }

        for (int i = 1; i < nodeRows - 1; i++)
        {
            for (int j = 1; j < nodeCols - 1; j++)
            {
                //nodes[i, j].connectedNodes[0] = nodes[i, j + 1];
                //nodes[i, j].connectedNodes[1] = nodes[i + 1, j + 1];
                //nodes[i, j].connectedNodes[2] = nodes[i + 1, j];
                //nodes[i, j].connectedNodes[3] = nodes[i + 1, j - 1];
                //nodes[i, j].connectedNodes[4] = nodes[i, j - 1];
                //nodes[i, j].connectedNodes[5] = nodes[i - 1, j - 1];
                //nodes[i, j].connectedNodes[6] = nodes[i - 1, j];
                //nodes[i, j].connectedNodes[7] = nodes[i - 1, j + 1];
                
            }
        }
    }

    void CastRays()
    {
        int layerMask = 1 << 8;
        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        for (int i = 0; i < nodeRows; i++)
        {
            for (int j = 0; j < nodeCols; j++)
            {
                for (int k = 0; k < dirs.Length; k++)
                {
                    // Does the ray intersect any objects excluding the player layer
                    RaycastHit2D hit = Physics2D.Raycast(new Vector2(nodes[i, j].obj.transform.position.x, nodes[i, j].obj.transform.position.y), dirs[k], 1, layerMask);
                    if (hit.collider != null)
                    {
                        Debug.DrawRay(nodes[i, j].obj.transform.position, dirs[k] * hit.distance, Color.yellow, 100);
                        //Debug.Log("Did Hit");
                        nodes[i, j].connections[k] = false;
                    }
                    else
                    {
                        Debug.DrawRay(nodes[i, j].obj.transform.position, dirs[k] * 1, Color.white, 100);
                        //Debug.Log("Did not Hit");
                        nodes[i, j].connections[k] = true;
                    }
                }
            }
        }
    }

    void CullNodes()
    {
        for (int i = 0; i < nodeRows; i++)
        {
            for (int j = 0; j < nodeCols; j++)
            {
                int count = 0;
                for (int k = 0; k < 8; k++)
                {
                    if (nodes[i,j].connections[k] == true)
                    {
                        count++;
                    }
                }
                if (count < 3 )
                {
                    nodes[i, j].removed = false;
                    Destroy(nodes[i,j].obj);
                }

            }
        }
    }

    List<Vector3> CalculatePath(Vector3 playerPos, Vector3 targetPos)
    {
        List<pNode> openList = new List<pNode>();
        List<pNode> closedList = new List<pNode>();

        int g = 0;
        
        pNode startNode;
        bool startFound = false;
        bool targetFound = false;
        int nodesDone = 0;

        for (int i = 0; i < nodeRows; i++)
        {
            for (int j = 0; j < nodeCols; j++)
            {
                //Debug.Log((nodes[i, j].obj.transform.position - playerPos).magnitude);
                if ((nodes[i, j].obj.transform.position - playerPos).magnitude < minDist && !startFound)
                {
                    Debug.Log("Found start node");
                    startFound = true;
                    startNode = nodes[i, j];
                    openList.Add(nodes[i, j]);
                    nodesDone++;
                }
                if ((nodes[i, j].obj.transform.position - targetPos).magnitude < minDist && !targetFound)
                {
                    Debug.Log("Found target node");
                    targetFound = true;
                    targetNode = nodes[i, j];
                    //openList.Add(nodes[i, j]);

                    nodesDone++;
                }
                if (nodesDone >= 2)
                {
                    i = nodeRows;
                    break;
                }
            }
        }

        while (openList.Count > 0)
        {
            int lowF = 50000; // random large number
            pNode current = new pNode();//node with lowest F
            foreach(pNode n in openList)
            {
                if (n.F < lowF)
                {
                    lowF = n.F;
                    current = n;
                }

                closedList.Add(current);
                openList.Remove(current);
                List<pNode> adj = GetWalkableNodes(current.X, current.Y);
                g++;
                for (int i = 0; i < adj.Count; i++)
                {
                    if (closedList.Contains(adj[i]))
                    {
                        continue;
                    }
                    else if (!openList.Contains(adj[i]))
                    {
                        adj[i].G = g;
                        adj[i].H = CalculateHScore(adj[i].X,
                        adj[i].Y, targetNode.X, targetNode.Y);
                        adj[i].F = adj[i].G + adj[i].H;
                        adj[i].parent = current;
                    }
                    else
                    {
                        // test if using the current G score makes the adjacent square's F score
                        // lower, if yes update the parent because it means it's a better path
                        if (g + adj[i].H < adj[i].F)
                        {
                            adj[i].G = g;
                            adj[i].H = CalculateHScore(adj[i].X,
                            adj[i].Y, targetNode.X, targetNode.Y);
                            adj[i].F = adj[i].G + adj[i].H;
                            adj[i].parent = current;
                        }
                    }

                }

                if (closedList[closedList.Count - 1].obj == targetNode.obj)
                {
                    List<Vector3> pathlist = new List<Vector3>();
                    foreach(pNode p in openList)
                    {
                        pathlist.Add(p.obj.transform.position);
                    }

                    return pathlist;
                }

            }
        }

        Debug.Log("Dont show this message");
        
        return null;
    }

    List<pNode> GetWalkableNodes(int x, int y)
    {
        List<pNode> proposedLocations = new List<pNode>()
        {
            nodes[x, y - 1],
            nodes[x - 1, y - 1],
            nodes[x, y + 1],
            nodes[x + 1, y + 1],
            nodes[x - 1, y],
            nodes[x - 1, y + 1],
            nodes[x + 1, y],
            nodes[x + 1, y - 1]
        };

        foreach(pNode n in proposedLocations)
        {
            if (n.obj == null)
            {
                proposedLocations.Remove(n);
            }
        }

        return proposedLocations;
     
    }

    static int CalculateHScore(int x, int y, int targetX, int targetY)
    {
        return Mathf.Abs(targetX - x) + Mathf.Abs(targetY - y);
    }
    
    public void DestroyGrid()
    {
        //Get rid of the grid
    }
    
}

