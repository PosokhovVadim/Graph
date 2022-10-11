using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Graph;

namespace Graph
{
    class Program
    {
        static void Main(string[] args)
        {
            //Dictionary<Vertex, List<Vertex>> G = new Dictionary<Vertex, List<Vertex>>();
            //List<Vertex> list = new List<Vertex>();
            Vertex x = new Vertex();
            Vertex y = new Vertex();
            Weight w = new Weight();
            //Graph[] graphs;
            bool main_fl = true;
            bool include_fl = true;
            string[] pathes = { @"D:\C#\graph\Graph\adjacency_list_1.txt"
                              , @"D:\C#\graph\Graph\adjacency_list_2.txt"
                              , @"D:\C#\graph\Graph\adjacency_list_3.txt"
                              , @"D:\C#\graph\Graph\adjacency_list_4.txt" };

            int action;
            int value;


            while (main_fl)
            {
                Console.WriteLine("Choose action: \n1 - Make graph\n0 - Exit");
                action = Convert.ToInt32(Console.ReadLine());
                switch (action)
                {
                    case 1:
                        Console.WriteLine("Choose path : ");

                        for (int i = 0; i < pathes.Length; i++)
                            Console.WriteLine($"{i}: {pathes[i]}");

                        int it = Convert.ToInt32(Console.ReadLine());
                        Graph g = new Graph(pathes[it]);
                        include_fl = true;

                        while (include_fl)
                        {
                            Console.WriteLine("Choose action: \n0 - Add vertex,\n1 - Remove vertex,\n2 - Add edge" +
                                "\n3 - Remove Edge\n4 - Write to result file\n5 - Write to current file\n6 - exit" +
                                "\n7 - Task1.N9");
                            action = Convert.ToInt32(Console.ReadLine());
                            switch (action)
                            {
                                case 0:
                                    Console.Write("Vertex: ");
                                    g.AddVertex(new Vertex(Convert.ToInt32(Console.ReadLine())));
                                    break;

                                case 1:
                                    Console.Write("Vertex: ");
                                    g.RemoveVertex(new Vertex(Convert.ToInt32(Console.ReadLine())));
                                    break;
                                case 2:
                                    Console.Write("First vertex:");
                                    x = new Vertex((Convert.ToInt32(Console.ReadLine())));
                                    Console.Write("Second vertex:");
                                    y = new Vertex((Convert.ToInt32(Console.ReadLine())));
                                    if (g.isweighted)
                                    {
                                        w = new Weight((Convert.ToInt32(Console.ReadLine())));
                                        g.AddEdges(new Edge(x, y, w));
                                    }
                                    else g.AddEdges(new Edge(x, y, null));

                                    break;
                                case 3:
                                    Console.Write("First vertex: ");
                                    x = new Vertex((Convert.ToInt32(Console.ReadLine())));
                                    Console.Write("Second vertex: ");
                                    y = new Vertex((Convert.ToInt32(Console.ReadLine())));
                                    g.RemoveEdge(x, y);
                                    break;
                                case 4:
                                    g.WriteToNewFile();
                                    break;
                                case 5:
                                    g.WriteToCopyFile();
                                    break;
                                case 6:
                                    include_fl = false;
                                    break;
                                case 7:
                                    Console.Write("Enter Vertex: ");
                                    x = new Vertex((Convert.ToInt32(Console.ReadLine())));
                                    List<Vertex> resList = g.HSofExodus(x);
                                    Console.Write($"Вершины, полустепень исхода которых больше, чем у вершины{x.X}: ");

                                    foreach (var i in resList)
                                        Console.Write($"{i.X}, ");
                                    break;
                                default:
                                    include_fl = false;
                                    break;

                            }

                        }
                        break;
                    case 0:
                        main_fl = false;
                        break;

                    default:
                        main_fl = false;
                        break;
                }
            }
              
        }
    }
}

