using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace PerformanceTestLibrary
{
    public class Util
    {
        public static Dictionary<int, Site> CreateSites()
        {
            int sitesCount = 1000;
            int zonesCount = 100;

            Console.WriteLine($"Creating {sitesCount} sites with {zonesCount} in each");
            var siteCache = new Dictionary<int, Site>();

            for (var site = 1; site < sitesCount; site++)
            {
                var zones = new List<Zone>();

                for (var zone = 1; zone < zonesCount; zone++)
                {
                    var rectangle = new Rectangle(site * zone * 10, site * zone * 10, 100, 100);
                    var polyGon = new PolyGon(DrawPolygon(rectangle));
                    zones.Add(new Zone() { Rectangle = rectangle, PolyGon = polyGon });
                }

                siteCache[site] = new Site() { ID = site, Zones = zones }; ;
            }

            return siteCache;
        }

        public static List<PloyPoint> DrawPolygon(Rectangle r)
        {

            var point1 = new PloyPoint(r.X + 0.0F, r.Y + 0.0F);
            var point2 = new PloyPoint(r.X + 100.0F, r.Y + 25.0F);
            var point3 = new PloyPoint(r.X + 200.0F, r.Y + 5.0F);
            var point4 = new PloyPoint(r.X + 250.0F, r.Y + 50.0F);
            var point5 = new PloyPoint(r.X + 300.0F, r.Y + 100.0F);
            var point6 = new PloyPoint(r.X + 350.0F, r.Y + 200.0F);
            var point7 = new PloyPoint(r.X + 250.0F, r.Y + 250.0F);

            PloyPoint[] ployPoints =
                     {
                 point1,
                 point2,
                 point3,
                 point4,
                 point5,
                 point6,
                 point7,
             };

            return ployPoints.ToList();
        }

        static long GetObjectSize(object obj)
        {
            long size = 0;
            using (Stream s = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, obj);
                size = s.Length;
            }

            return size;
        }
    }
}
