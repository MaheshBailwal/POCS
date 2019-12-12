// Written By: Raju Bhupathi
// On : 2 Feb 2010
// Refernce for finding a point inside a polygon "http://alienryderflex.com/polygon/"

using ConsoleApp2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RouteGeoFence
{

    [Serializable]
    public class PolyGon
    {
        public List<PloyPoint> ployPoints = new List<PloyPoint>();
        public PolyGon()
        {
        }
        public PolyGon(List<PloyPoint> points)
        {
            foreach (PloyPoint p in points)
            {
                this.ployPoints.Add(p);
            }
        }
        public void Add(PloyPoint p)
        {
            this.ployPoints.Add(p);
        }
        public int Count()
        {
            return ployPoints.Count;
        }

      
        public bool FindPoint(double X, double Y)
        {
            int sides = this.Count() - 1;
            int j = sides - 1;
            bool pointStatus = false;
            for (int i = 0; i < sides; i++)
            {
                if (ployPoints[i].Y < Y && ployPoints[j].Y >= Y || ployPoints[j].Y < Y && ployPoints[i].Y >= Y)
                {
                    if (ployPoints[i].X + (Y - ployPoints[i].Y) / (ployPoints[j].Y - ployPoints[i].Y) * (ployPoints[j].X - ployPoints[i].X) < X)
                    {
                        pointStatus = !pointStatus;
                    }
                }
                j = i;
            }
            return pointStatus;
        }
    }

    
}
