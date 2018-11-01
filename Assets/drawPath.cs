using System.Collections;
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

//using QuickGraph;
public class drawPath : MonoBehaviour
{
    // begin sepUp graph


    private DijkstraShortestPathAlgorithm<node, Edge<node>> dijkstra;
    public List<string> path;
    private VertexPredecessorRecorderObserver<node, Edge<node>> predecessorObserver;
    private AdjacencyGraph<node, Edge<node>> myGraph;
    private Dictionary<Edge<node>, float> edgeCost;
    private List<node> myNode;
    private List<Edge<node>> listEdgeShortestGraph;
    private List<Edge<node>> listEdgeNotInShortestGraph;
    public int[] exitnode = { 50, 101, 107 };
    private List<Road> myRoad;
    public int soCanh = 0;
    public void SetRoad()
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
                soCanh += 1;
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


    //end setUp graph


    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainner;
    

    
    private void Awake()
    {
        SetRoad();
        setGraph();
        setUplist();
        Debug.Log("tuan thai ba");

        foreach (node mnode in myNode)
        {
            Debug.Log(mnode.xPos + "  " + mnode.yPos);
        }
   

        foreach (Edge<node> edge in listEdgeNotInShortestGraph)
        {
            node n1 = edge.Source;
            node n2 = edge.Target;
        }
    }


    private GameObject createCircle(Vector2 anchoredVector)
    {
        GameObject gameObject = new GameObject("redCircle", typeof(Image));
        gameObject.transform.SetParent(graphContainner, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredVector;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    private void ShowMyGraph(List<node> valueList)
    {
        float graphHeight = graphContainner.sizeDelta.y;
        float yMaximum = 100f;
        float xSize = 50f;

        foreach (node node in valueList)
        {
            Debug.Log(node.xPos + "  " + node.yPos);
            float xPosition = node.xPos;
            float yPosition = node.yPos;
            GameObject circleGameObject = createCircle(new Vector2(xPosition, yPosition));
        }



        foreach (Edge<node> edge in listEdgeShortestGraph)
        {
            node n1 = edge.Source;
            node n2 = edge.Target;

            CreateDotConnection(new Vector2(n1.xPos, n1.yPos), new Vector2(n2.xPos, n2.yPos), new Color(237, 41, 57));
        }
    }

    private void ShowGraph(List<int> valueList)
    {
        float graphHeight = graphContainner.sizeDelta.y;
        float yMaximum = 100f;
        float xSize = 50f;

        GameObject lastCircleGameObject = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;
            GameObject circleGameObject = createCircle(new Vector2(xPosition, yPosition));
            if (lastCircleGameObject != null)
            {
                //CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircleGameObject = circleGameObject;
        }
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, Color color)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainner, false);
        gameObject.GetComponent<Image>().color = new Color(247, 47, 19, 1);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
    }
}

