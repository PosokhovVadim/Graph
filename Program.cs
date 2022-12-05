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
                              , @"D:\C#\graph\Graph\frame2.txt"
                              , @"D:\C#\graph\Graph\MaxFlow1.txt"
                              , @"D:\C#\graph\Graph\MaxFlow2.txt"
                              , @"D:\C#\graph\Graph\MaxFlow3.txt"
                              , @"D:\C#\graph\Graph\dijkstra1.txt"
                              , @"D:\C#\graph\Graph\dijkstra2.txt"
                              , @"D:\C#\graph\Graph\floyd1.txt"
                              , @"D:\C#\graph\Graph\floyd2.txt"
                              , @"D:\C#\graph\Graph\Centre.txt"};

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
                                "\n7 - Task1.N9 \n8 - Task1.N12 \n9 - Task1.N15 \n10 - Task2.N17 \n11 - Task2.N38 \n12 - Create MST " +
                                "\n13 - Task4.12(Dijkstra's algorithm) \n16 - Find Max Flow");
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
                                case 10:
                                    if (g._isoriented)
                                    {

                                        if (g.isCyclic())
                                        {
                                            Console.WriteLine("Граф имеет циклы");
                                        }
                                        else Console.WriteLine("Граф ацикличен");
                                    }
                                    else Console.WriteLine("Граф не ориентированный");
                                    break;

                                case 11:

                                    if (g._isoriented)
                                    {
                                        g.allShortestCylces();
                                     
                                    }
                                    else
                                    {
                                        Console.WriteLine("Граф неориентированный");
                                    }
                                    break;

                                case 12:
                                    if (g._isweighted && !g._isoriented)
                                    {
                                        g.MST();
                                        g.WriteToNewFile();
                                    }
                                
                                    break;
                                case 13:
                                    //
                                    if (g.isweighted && g._isoriented)
                                    {
                                        Console.Write("Список вершину: ");
                                        foreach (var vertex in g.GetVertices())
                                        {
                                            Console.Write($"{vertex.X}, ");
                                        }
                                        Console.WriteLine();
                                        Console.Write("First vertex: ");
                                        x = new Vertex((Convert.ToInt32(Console.ReadLine())));
                                        Console.Write("Second vertex: ");
                                        y = new Vertex((Convert.ToInt32(Console.ReadLine())));
                                        List<Vertex> res = new List<Vertex>();
                                        res = g.ShosrtestPath_Dijkstra(x, y);
                                        Console.WriteLine("Кратчайший путь: ");
                                        foreach (var vertex in res) { Console.Write($"{vertex.X}, "); }
                                    }
                                    else Console.WriteLine("Граф неориентирован и/или не взвешан");

                                    break;
                                case 14:
                                    if (g.isweighted && g._isoriented)
                                    {
                                        
                                        List<Vertex> centre =  g.CentreOfGraph();
                                        Console.WriteLine("Центр графа: ");
                                        foreach (var vertex in centre) { Console.Write($"{vertex.X}, "); }
                                    }
                                    else Console.WriteLine("Граф неориентирован и/или не взвешан");
                                    break;
                                case 15:
                                    break;
                                case 16:
                                    Console.WriteLine(g.Max_Flow());
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

