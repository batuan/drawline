using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Use this for initialization
    public void Start()
    {
        SetRoad();
        foreach(Road road in myRoad) {
            drawRoad(road);
            //Debug.Log(road.node1 + " (" + myNode[road.node1-1].xPos + ", "+ myNode[road.node1 - 1].yPos +") "+ road.node2 + "( " + myNode[road.node2 - 1].xPos + ", " + myNode[road.node2 - 1].yPos+")");
        }

        foreach (node node1 in myNode)
        {
            GameObject nodeGo = Instantiate(node);
            Vector3 position = new Vector3((float)(node1.xPos) , (float)node1.yPos, 0);
            nodeGo.transform.SetPositionAndRotation(position,new Quaternion(0,0,0,0));
        }
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
            node mNode = new node(x, y);
            myNode.Add(mNode);
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
