using System;
using System.Collections.Generic;
using System.Drawing;

namespace PerformanceTestLibrary
{
    [Serializable]
    public class Zone
    {
        public Rectangle Rectangle { get; set; }
        public PolyGon PolyGon { get; set; }
    }

    [Serializable]
    public class Site
    {
        public List<Zone> Zones { get; set; }
        public int SiteID { get; set; }
        public string Name { get; set; }
        public string id { get; set; }
    }

    [Serializable]
    public class PloyPoint
    {
        public double X;
        public double Y;
        public PloyPoint() { }
        public PloyPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

}
