using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Road
   
    {
        public int node1;
        public int node2;

        public int width;
        public int length;

        public float trust;
        public Road(int n1, int n2, int leng, int wid, float tru)
        {
            node1 = n1;
            node2 = n2;
            width = wid;
            trust = tru;
            length = leng;
        }

        public void print()
        {
            System.Console.WriteLine(node1 + " " + node2 + " " + width + " " + length + " " + trust);
        }

    }

    
