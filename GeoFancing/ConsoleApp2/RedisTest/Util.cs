using RouteGeoFence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace RedisTest
{
    public class Util
    {
        static void Main12(string[] args)
        {
            Console.WriteLine("Creating sites");
            var sites = CreateSites();
            Console.WriteLine("Creating sites done");

            var size = GetObjectSize(sites);

            for (var count = 1; count < 10; count++)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var site = sites[count];

                var x = (count * 1 * 10) + 100;
                var y = (count * 1 * 10) + 55;

                //check corodinates exist in rectangle
                var zone = site.Zones.FirstOrDefault(z => z.Rectangle.Contains(x, y));

                //if yes then then find whether corodiante exist in polygon
                if (zone != null)
                {
                    var found = zone.PolyGon.FindPoint(x, y);
                    if (found)
                    {
                        Console.WriteLine("Inside the polygon");
                    }
                }

                stopwatch.Stop();
                Console.WriteLine("Time to serach in  milliseconds " + stopwatch.ElapsedMilliseconds);
            }

            Console.ReadLine();
        }

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

                siteCache[site] = new Site() { SiteID = site, Zones = zones }; ;
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

        public static class ConnectionString
        {
            private static string cName = "Server=tcp:wencodbserver.database.windows.net,1433;Initial Catalog=wencodb;Persist Security Info=False;User ID=wencoadmin;Password=P@ssword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            public static string CName
            {
                get => cName;
            }
        }
    }
}
