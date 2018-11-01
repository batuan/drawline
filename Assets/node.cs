using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class node
{
    public int thuTu;
    public int xPos;
    public int yPos;

    public node(int p1, int p2, int tt)
    {
        thuTu = tt;
        xPos = p1;
        yPos = p2;
    }

    public void toString()
    {
        Console.WriteLine(xPos + " " + yPos + " " + thuTu);

    }

}

