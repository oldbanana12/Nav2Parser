using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Nav2Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1 || Path.GetExtension(args[0]) != ".nav2")
            {
                Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} <path to .nav2 file>");
                Environment.Exit(1);
            }

            Nav2 nav2 = new Nav2();
            nav2.Read(args[0]);

            bool exit = false;

            string prefix = Path.GetFileNameWithoutExtension(args[0]);

            // The fact we use segmentChunks here is irrelevant, all of the blocks have the same group IDs
            foreach (var group in nav2.segmentChunks.Keys)
            {
                ExtractNavmesh(prefix, nav2, group);
                ExtractSegmentGraph(prefix, nav2, group);
                ExtractSection2(prefix, nav2, group);
                ExtractNavworld(prefix, nav2, group);
                ExtractNavworldBitmap(prefix, nav2, group);
            }

            while (!exit)
            {
                Console.WriteLine("Select an option:");
                Console.WriteLine("1 - Header Info");
                Console.WriteLine("2 - Manifest Info");
                Console.WriteLine("3 - NavWorld Info");
                Console.WriteLine("4 - Section 2 Info");
                Console.WriteLine("0 - Exit");

                switch (Console.ReadLine())
                {
                    case "0":
                        exit = true;
                        break;
                    case "1":
                        nav2.DisplayHeaderInfo();
                        break;
                    case "2":
                        nav2.DisplayManifestInfo();
                        break;
                    case "3":
                        nav2.DisplayNavWorldInfo();
                        break;
                    case "4":
                        nav2.DisplaySection2Info();
                        break;
                    default:
                        Console.WriteLine("\nInvalid option!\n");
                        break;
                } //switch
            } //while
        } //Main

        private static void ExtractNavmesh(string prefix, Nav2 nav2, byte group)
        {
            using (StreamWriter file = new StreamWriter(String.Format("{0}_group{1}.obj", prefix, group)))
            {
                var navmesh = nav2.navmeshChunks[group];
                foreach (var vertex in navmesh.navmeshChunkSubsection1Entries)
                {
                    var f1 = (float)vertex.x / (float)nav2.header.xDivisor;
                    var f2 = (float)vertex.y / (float)nav2.header.yDivisor;
                    var f3 = (float)vertex.z / (float)nav2.header.zDivisor;

                    file.WriteLine("v {0} {1} {2}", f1, f2, f3);
                }

                int c = 0;
                foreach (var chunk in nav2.segmentChunks[group].segmentChunkSubsection2Entries)
                {
                    file.WriteLine("g chunk_{0}", c);
                    for (int i = 0; i < chunk.faces; i++)
                    {
                        var navmeshChunk = navmesh.navmeshChunkSubsection3Entries[chunk.navmeshChunkSubsection2EntryIndex + i];
                        var f1 = navmeshChunk.vertex1 + chunk.vertexIndexOffset + 1;
                        var f2 = navmeshChunk.vertex2 + chunk.vertexIndexOffset + 1;
                        var f3 = navmeshChunk.vertex3 + chunk.vertexIndexOffset + 1;

                        file.WriteLine("f {0} {1} {2}", f1, f2, f3);
                    }

                    c++;
                }
            }
        }

        private static void ExtractSegmentGraph (string prefix, Nav2 nav2, byte group)
        {
            using (StreamWriter file = new StreamWriter(String.Format("{0}_group{1}_segmentGraph.obj", prefix, group)))
            {
                int v = 1;

                foreach (var node in nav2.segmentGraphs[group].navworldSegmentGraphSubsection1Entries)
                {
                    float x = (float)node.x / (float)nav2.header.xDivisor;
                    float y = (float)node.y / (float)nav2.header.yDivisor;
                    float z = (float)node.z / (float)nav2.header.zDivisor;

                    file.WriteLine("v {0} {1} {2}", x - 0.5, y + 0.5, z - 0.5);
                    file.WriteLine("v {0} {1} {2}", x - 0.5, y + 0.5, z + 0.5);
                    file.WriteLine("v {0} {1} {2}", x + 0.5, y + 0.5, z - 0.5);
                    file.WriteLine("v {0} {1} {2}", x + 0.5, y + 0.5, z + 0.5);

                    file.WriteLine("f {0} {1} {2} {3}", v, v + 1, v + 2, v + 3);
                    v += 4;
                }
            }
        }

        private static void ExtractSection2(string prefix, Nav2 nav2, byte group)
        {
            if (nav2.header.section2EntryCount > 0)
            {
                using (StreamWriter file = new StreamWriter(String.Format("{0}_group{1}_section2.obj", prefix, group)))
                {
                    foreach (var section2_entry in nav2.section2Entries[0].subsection1Entries)
                    {
                        float x = (float)section2_entry.x / (float)nav2.header.xDivisor;
                        float y = (float)section2_entry.y / (float)nav2.header.yDivisor;
                        float z = (float)section2_entry.z / (float)nav2.header.zDivisor;

                        file.WriteLine("v {0} {1} {2}", x, y, z);
                    }

                    for (int i = 0; i < nav2.section2Entries[0].subsection1Entries.Length; i++)
                    {
                        for (int j = 0; j < nav2.section2Entries[0].subsection2Entries[i].adjacentNodes.Length; j++)
                        {
                            var adjacentEdge = nav2.section2Entries[0].subsection2Entries[i].adjacentNodes[j];
                            file.WriteLine("l {0} {1}", i + 1, adjacentEdge + 1);
                        }
                    }
                }
            }
        }

        private static void ExtractNavworld(string prefix, Nav2 nav2, byte group)
        {
            using (StreamWriter file = new StreamWriter(String.Format("{0}_group{1}_navworld.obj", prefix, group)))
            {
                for (int i = 0; i < nav2.navWorlds[group].navWorldPoints.Length; i++)
                {
                    var point = nav2.navWorlds[group].navWorldPoints[i];
                    float x = (float)point.x / (float)nav2.header.xDivisor;
                    float y = (float)point.y / (float)nav2.header.yDivisor;
                    float z = (float)point.z / (float)nav2.header.zDivisor;

                    file.WriteLine("v {0} {1} {2}", x, y, z);
                }

                for (int i = 0; i < nav2.navWorlds[group].navWorldPoints.Length; i++)
                {
                    for (int j = 0; j < nav2.navWorlds[group].navWorldSubsection3Entries[i].adjacentNodeIndices.Length; j++)
                    {
                        var adjacentEdge = nav2.navWorlds[group].navWorldSubsection3Entries[i].adjacentNodeIndices[j];
                        file.WriteLine("l {0} {1}", i + 1, adjacentEdge + 1);
                    }
                }
            }
        }

        private static void ExtractNavworldBitmap(string prefix, Nav2 nav2, byte group)
        {
            Bitmap bmp = new Bitmap(2010, 2010);

            var red_line_pen = new Pen(Color.Red, 2.5f);
            uint numEdges = 0;

            uint max_x = 0;
            uint max_z = 0;
            uint min_x = 0;
            uint min_z = 0;

            for (int i = 0; i < nav2.navWorlds[group].navWorldPoints.Length; i++)
            {
                var point = nav2.navWorlds[group].navWorldPoints[i];
                if (point.x > max_x)
                    max_x = point.x;

                if (point.x < min_x)
                    min_x = point.x;

                if (point.z > max_z)
                    max_z = point.z;

                if (point.z < min_z)
                    min_z = point.z;
            }

            using (Graphics g = Graphics.FromImage(bmp))
            {
                var length = nav2.navWorlds[group].navWorldPoints.Length;
                for (int i = 0; i < length; i++)
                {
                    var point = nav2.navWorlds[group].navWorldPoints[i];

                    var x = 2005.0f * (point.x - min_x) / (max_x - min_x);
                    var z = 2005.0f * (point.z - min_z) / (max_z - min_z);

                    g.FillEllipse(Brushes.Blue, x - 2.5f, z - 2.5f, 5.0f, 5.0f);

                    var n_edges = nav2.navWorlds[group].navWorldSubsection3Entries[i].adjacentNodeIndices.Length;
                    for (int j = 0; j < n_edges; j++)
                    {
                        var adjacent_index = nav2.navWorlds[group].navWorldSubsection3Entries[i].adjacentNodeIndices[j];
                        var adjacent = nav2.navWorlds[group].navWorldPoints[adjacent_index];

                        var adjacent_x = 2005.0f * (adjacent.x - min_x) / (max_x - min_x);
                        var adjacent_z = 2005.0f * (adjacent.z - min_z) / (max_z - min_z);

                        var edge_index = nav2.navWorlds[group].navWorldSubsection3Entries[i].edgeIndices[j];
                        var edge = nav2.navWorlds[group].navWorldEdges[edge_index];

                        g.DrawLine(red_line_pen, x, z, adjacent_x, adjacent_z);

                        g.DrawString(edge.weight.ToString(), SystemFonts.DefaultFont, Brushes.Black, (x + adjacent_x) / 2, (z + adjacent_z) / 2);
                        numEdges++;

                    }
                }
            }

            bmp.Save(string.Format("{0}_navworld_graph{1}.png", prefix, group), ImageFormat.Png);
        }
    } //class
} //namespace
