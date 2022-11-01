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
                              , @"D:\C#\graph\Graph\adjacency_list_4.txt"
                              , @"D:\C#\graph\Graph\test5_38.txt"
                              , @"D:\C#\graph\Graph\test5_38_2.txt"
                              , @"D:\C#\graph\Graph\frame.txt"
                              , @"D:\C#\graph\Graph\frame2.txt"};

            int action;
            int value;


            while (main_fl)
            {
                Console.WriteLine("Choose action: \n1 - Make one graph\n0 - Exit\n2 - Work with two graph");
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
                                "\n7 - Task1.N9 \n8 - Task1.N12 \n13 - Create MST");
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
                                case 8:
                                    List<Vertex> resList_1 = g.LoopInGraph();

                                    Console.Write($"Вершины в которых содержатся петли: ");
                                    foreach (var i in resList_1)
                                        Console.Write($"{i.X}, ");
                                    break;
                                case 9:
                                    Console.Write("Введите вершину:");

                                    List<Vertex> resList_2 = g.NotAdjacentVerties(new Vertex((Convert.ToInt32(Console.ReadLine()))));

                                    Console.Write($"Вершины не смежные с данной: ");
                                    foreach (var i in resList_2)
                                        Console.Write($"{i.X}, ");
                                    break;
                                case 11:

                                    if (g.isCyclic())
                                    {
                                        Console.Write("Граф имеет циклы");
                                    }
                                    else Console.Write("Граф ацикличен");
                                    break;

                                case 12:

                                    if (g._isoriented)
                                    {
                                        g.allShortestCylces();
                                     
                                    }
                                    else
                                    {
                                        Console.WriteLine("Граф неориентированный");

                                    }
                                    break;

                                case 13:
                                        g.MST();
                                        g.WriteToNewFile();

                                    break;
                                default:
                                    include_fl = false;
                                    break;
                            
                            }

                        }
                        break;
                    case 2:
                        Graph g_1 = new Graph(pathes[1]);
                        Graph g_2 = new Graph(pathes[2]);
                        g_1.SymmetricDifference(g_2);
                        g_1.WriteToNewFile();
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

