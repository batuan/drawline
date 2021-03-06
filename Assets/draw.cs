﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Threading;

using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.ShortestPath;
using QuickGraph.Serialization;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Collections;
using QuickGraph.Algorithms.Search;
public class draw : MonoBehaviour {
    
    public GameObject line1;
    public GameObject node;
    public GameObject line2;
    public GameObject exit_node;
    line activeLine;
    line myLine1;
    line myLine2;
    List<node> myNode;
    List<Road> myRoad;
    int index = 0;

    private DijkstraShortestPathAlgorithm<node, Edge<node>> dijkstra;
    public List<string> path;
    private VertexPredecessorRecorderObserver<node, Edge<node>> predecessorObserver;
    private AdjacencyGraph<node, Edge<node>> myGraph;
    private Dictionary<Edge<node>, float> edgeCost;
    private List<Edge<node>> listEdgeShortestGraph;
    private List<Edge<node>> listEdgeNotInShortestGraph;
    public int[] exitnode = { 50, 101, 107 };
    public int soCanh = 0;
    IEnumerable<Edge<node>> edges;


    // Use this for initialization
    public void Start()
    {
        SetRoad();
        setGraph();
        List<IEnumerable<Edge<node>>> resultList = new List<IEnumerable<Edge<node>>>();
        foreach (node mnode in myNode)
        {
            if (mnode.thuTu == 50 || mnode.thuTu == 101 || mnode.thuTu == 169) continue;
            IEnumerable<Edge<node>> x = MinExitPath(mnode);
            if (x == null) continue;
            resultList.Add(MinExitPath(mnode));
        }

        setUplist();


        foreach (Road road in myRoad) {
            //drawRoad(road);
            //Debug.Log(road.node1 + " (" + myNode[road.node1-1].xPos + ", "+ myNode[road.node1 - 1].yPos +") // "+ road.node2 + "( " + myNode[road.node2 - 1].xPos + ", " + myNode[road.node2 - 1].yPos+")");
        }

        foreach (node node1 in myNode)
        {
            if(node1.thuTu == 50|| node1.thuTu == 101|| node1.thuTu == 107)
            {
                GameObject nodeGo1 = Instantiate(exit_node);
                Vector3 position1 = new Vector3((float)(node1.xPos), (float)node1.yPos, 0);
                nodeGo1.transform.SetPositionAndRotation(position1, new Quaternion(0, 0, 0, 0));
                continue;
            }
            GameObject nodeGo = Instantiate(node);
            Vector3 position = new Vector3((float)(node1.xPos) , (float)node1.yPos, 0);
            nodeGo.transform.SetPositionAndRotation(position,new Quaternion(0,0,0,0));
        }


        foreach (Edge<node> edge in listEdgeShortestGraph)
        {
            Debug.Log("thu tu:" + edge.Source.thuTu);
            node n1 = edge.Source;
            node n2 = edge.Target;
            Road road = new Road(n1.thuTu, n2.thuTu, 0, 4, 1);
            drawRoad(road);
        }

        edges = MinExitPath(myNode[0]);

        foreach(Edge<node> edge in edges)
        {
           // Debug.Log(edge.Target.thuTu + "  " + edge.Source.thuTu );

        }

        drawSeriesLine(edges);

    }


    void SetRoad()
    {
        myNode = new List<node>();
        myRoad = new List<Road>();
        string[] lines = System.IO.File.ReadAllLines("C:\\Users\\ThaiTuan\\Documents\\simulation\\Assets\\graph.txt");
        foreach (string line in lines)
        {
            string[] words = line.Split(';');
            int thutu = int.Parse(words[0]);
            int x = int.Parse(words[1]);
            int y = int.Parse(words[2]);
            int index = 3;
            while (index < words.Length - 1)
            {
                int dinhKe = int.Parse(words[index].Split('#')[1]);
                int leng = int.Parse(words[index + 1]);
                int wid = int.Parse(words[index + 2]);
                float tr = float.Parse(words[index + 3]);
                Road mRoad = new Road(thutu, dinhKe, leng, wid, tr);
                myRoad.Add(mRoad);
                index = index + 4;
            }
            node mNode = new node(x, y, thutu);
            myNode.Add(mNode);
        }
    }


    public void setGraph()
    {
        listEdgeNotInShortestGraph = new List<Edge<node>>();
        myGraph = new AdjacencyGraph<node, Edge<node>>();
        edgeCost = new Dictionary<Edge<node>, float>(myRoad.Count);
        foreach (node node in myNode)
        {
            myGraph.AddVertex(node);
        }
        foreach (Road road in myRoad)
        {
            node n1 = myNode[road.node1 - 1];
            node n2 = myNode[road.node2 - 1];
            Edge<node> edge = new Edge<node>(n1, n2);
            myGraph.AddEdge(edge);
            edgeCost.Add(edge, road.length);
            if (contains(listEdgeNotInShortestGraph, edge)[0] != 1)
            {
                listEdgeNotInShortestGraph.Add(edge);
            }

        }

        listEdgeShortestGraph = new List<Edge<node>>();
    }

    private void Dijkstra(node mNode)

    {
        dijkstra = new DijkstraShortestPathAlgorithm<node, Edge<node>>(myGraph, e => edgeCost[e]);
        predecessorObserver = new VertexPredecessorRecorderObserver<node, Edge<node>>();
        using (predecessorObserver.Attach(dijkstra))
            dijkstra.Compute(mNode);
    }

    IEnumerable<Edge<node>> MinExitPath(node mNode)
    {
        Dijkstra(mNode);

        List<node> list = new List<node>();
        foreach (int index in exitnode)
        {
            list.Add(myNode[index - 1]);
        }

        double min = double.MaxValue;
        node mexitNode = new node(0, 0, 0);
        foreach (node node in list)
        {
            double mMin = 0;
            dijkstra.TryGetDistance(node, out mMin);
            if (mMin < min)
            {
                min = mMin;
                mexitNode = node;
            }
        }

        IEnumerable<Edge<node>> result;
        predecessorObserver.VertexPredecessors.TryGetPath(mexitNode, out result);

        if (result == null) return null;

        foreach (Edge<node> tuan in result)
        {

            if (contains(listEdgeShortestGraph, tuan)[0] == 1) continue;
            listEdgeShortestGraph.Add(tuan);

        }


        return result;
    }

    int[] contains(List<Edge<node>> list, Edge<node> edge)
    {
        int index = 0;
        foreach (Edge<node> edgeList in list)
        {
            if ((edgeList.Source.thuTu == edge.Source.thuTu) && edgeList.GetOtherVertex(edgeList.Source).thuTu == edge.GetOtherVertex(edge.Source).thuTu)
            {
                int[] b = { 1, index };
                return b;
            }
            if ((edgeList.Source.thuTu == edge.GetOtherVertex(edge.Source).thuTu) && edgeList.GetOtherVertex(edgeList.Source).thuTu == edge.Source.thuTu)
            {
                int[] b = { 1, index };
                return b;
            }
            index += 1;
        }
        int[] a = { 0, 0 };
        return a;
    }

    void remove(List<Edge<node>> list, Edge<node> edge)
    {
        list.Remove(edge);
        Edge<node> newEdge = new Edge<node>(edge.GetOtherVertex(edge.Source), edge.Source);
        list.Remove(newEdge);
    }

    void setUplist()
    {
        foreach (Edge<node> edge in listEdgeShortestGraph)
        {
            int[] result = contains(listEdgeNotInShortestGraph, edge);
            if (result[0] == 1)
            {
                listEdgeNotInShortestGraph.RemoveAt(result[1]);
            }
        }

        foreach (Edge<node> edge in listEdgeShortestGraph)
        {
            if (contains(listEdgeNotInShortestGraph, edge)[0] == 1)
            {

            }
        }
    }

    void drawLine(node node1, node node2)
    {
         GameObject road1 = Instantiate(line1);
         myLine1 = road1.GetComponent<line>();
         LineRenderer lineRenderer = myLine1.GetComponent<LineRenderer>();
         lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
         lineRenderer.SetColors(Color.green, Color.green);   
         myLine1.DrawLine(new Vector3(node1.xPos, node1.yPos, 0), new Vector3(node2.xPos, node2.yPos));
   }


    void drawSeriesLine(IEnumerable<Edge<node>> list)
    {
        foreach(Edge<node> edge in list)
        {
            drawLine(edge.Source, edge.Target);
        }
    }


    void drawRoad(Road road)
    {
        int width = road.width;
        node node1 = myNode[road.node1 -1];
        node node2 = myNode[road.node2 - 1];
        if(node1.xPos == node2.xPos)
        {
            GameObject road1 = Instantiate(line1);
            GameObject road2 = Instantiate(line2);
            myLine1 = road1.GetComponent<line>();
            myLine2 = road2.GetComponent<line>();
            Vector3[] forNode1 = NodeToVec(node1, true, (float)width);
            Vector3[] forNode2 = NodeToVec(node2, true, (float)width);
            myLine1.DrawLine(forNode1[0], forNode2[0]);
            myLine2.DrawLine(forNode1[1], forNode2[1]);
        }

        else
        {
            GameObject road1 = Instantiate(line1);
            GameObject road2 = Instantiate(line2);
            myLine1 = road1.GetComponent<line>();
            myLine2 = road2.GetComponent<line>();
            Vector3[] forNode1 = NodeToVec(node1, false, (float)width);
            Vector3[] forNode2 = NodeToVec(node2, false, (float)width);
            //printVec(forNode1);
            //printVec(forNode2);
            myLine1.DrawLine(forNode1[0], forNode2[0]);
            myLine2.DrawLine(forNode1[1], forNode2[1]);
        }

        
    }

    Vector3[] NodeToVec(node node, bool xAxis, float width)
    {
        if (xAxis)
        {
            Vector3 a = new Vector3(((float)node.xPos ) + width, ((float)node.yPos), 0);
            Vector3 b = new Vector3(((float)node.xPos ) - width, (float)node.yPos, 0);
            Vector3[] list = { a, b };
            return list;
        }
        else
        {
            Vector3 a = new Vector3((float)node.xPos, ((float)node.yPos) + width, 0);
            Vector3 b = new Vector3((float)node.xPos, ((float)node.yPos) - width, 0);
            Vector3[] list = { a, b };
            return list;
        }
        
    }

    void printVec(Vector3[] y)
    {
        foreach(Vector3 x in y)
        {
            Debug.Log(x.x + " " + x.y + " " + x.z+ " " + index);
            index += 1;
        }
    }
}
