using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections;

namespace Graph
{
    class Graph
    {
        #region fields

        private bool IsWeighted { get; set; } // взвешанный или нет
        private bool IsOriented { get; set; } //ориентированный или нет

        public bool _isweighted;
        public virtual bool isweighted
        {
            get { return _isweighted; }
            set { _isweighted = value; }
        }
        public bool _isoriented { get { return IsOriented; } }

        private string Path;
        private bool fl;

        private Dictionary<Vertex, List<Dictionary<Vertex, Weight>>> G = new Dictionary<Vertex, List<Dictionary<Vertex, Weight>>>();

        private List<Edge> Edges = new List<Edge>();
        private List<Vertex> Vertices = new List<Vertex>();

        public List<Vertex> GetVertices()
        {
            return Vertices;
        }

        public List<Edge> GetEdges()
        {
            return Edges;
        }
        private List<Dictionary<Vertex, Weight>> V_list = new List<Dictionary<Vertex, Weight>>();
        private Dictionary<Vertex, Weight> tmp_value = new Dictionary<Vertex, Weight>();

        Vertex x = new Vertex();
        Weight w = new Weight();

        private Dictionary<Vertex, bool> Visited = new Dictionary<Vertex, bool>();
        private Dictionary<Vertex, bool> recVisited = new Dictionary<Vertex, bool>();
        private Dictionary<Vertex, int> SP = new Dictionary<Vertex, int>();


        #endregion

        #region constructors
        public Graph() { }


        public Graph(string path)
        {
            Path = path;
            using (FileStream Source = new FileStream(path, FileMode.Open))
            {
                using (StreamReader read = new StreamReader(Source))
                {
                    string s = read.ReadToEnd();
                    string[] line = s.Split("\n");

                    if (Int64.Parse(line[0]) == 0) IsWeighted = false; else IsWeighted = true;

                    if (Int64.Parse(line[1]) == 0) IsOriented = false; else IsOriented = true;

                    _isweighted = IsWeighted;
                    for (int i = 2; i < line.Length; i++)
                    {
                        string[] currentLine = line[i].Split(' ');
                        V_list = new List<Dictionary<Vertex, Weight>>();
                        int[] a;
                        if (!IsWeighted)
                        {
                            a = line[i].Split(' ', ':').
                                          Where(x => !string.IsNullOrWhiteSpace(x)).
                                          Select(x => int.Parse(x)).ToArray();
                            tmp_value = new Dictionary<Vertex, Weight>();
                            for (int j = 1; j < a.Length; j++)
                            {
                                x = new Vertex(a[j]);
                                tmp_value.Add(x, null);

                            }
                            V_list.Add(tmp_value);
                        }
                        else
                        {

                            a = line[i].Split(' ', ':', '(', ')', ',').
                                       Where(x => !string.IsNullOrWhiteSpace(x)).
                                       Select(x => int.Parse(x)).ToArray();

                            tmp_value = new Dictionary<Vertex, Weight>();
                            for (int j = 1; j < a.Length; j++)
                            {
                                x = new Vertex(a[j]);
                                w = new Weight(a[++j]);
                                tmp_value.Add(x, w);
                            }
                            V_list.Add(tmp_value);

                        }
                        G.Add(new Vertex(a[0]), V_list);
                    }

                }
            }
            Vertices.Clear();
            Edges.Clear();
            SetEdges();
            SetVertices();
            WriteToCopyFile();


        }

        #endregion

        #region Methods 

        public List<Edge> SetEdges()
        {

            foreach (var key in G.Keys)
            {
                foreach (var list_values in G[key])
                {
                    foreach (var value in list_values)
                    {
                        Edges.Add(new Edge(key, value.Key, value.Value));
                    }
                }
            }

            return Edges;
        }


        public List<Vertex> SetVertices()
        {

            foreach (var key in G.Keys)
                Vertices.Add(key);


            return Vertices;
        }

        public void AddVertex(Vertex v)
        {
            fl = true;
            foreach (var key in G.Keys)
                if (key.Equals(v))
                {
                    fl = false;
                    break;
                }
            if (fl)
            {
                V_list = new List<Dictionary<Vertex, Weight>>();
                G.Add(v, V_list);
            }
            Vertices.Clear();
            SetVertices();
        }

        public void RemoveVertex(Vertex v)
        {
            foreach (var key in G.Keys)
            {
                if (key.Equals(v))
                {
                    G.Remove(key);
                }
                else
                {
                    V_list = new List<Dictionary<Vertex, Weight>>(G[key]);
                    foreach (var list_values in V_list)
                        foreach (var value in list_values)
                            if (value.Key.Equals(v))
                                list_values.Remove(value.Key);

                }
            }
            Vertices.Clear();
            SetVertices();
        }


        private bool EdgeCheck(Edge edge)
        {
            foreach (var i in Edges)
            {
                if (edge.X.Equals(i.X) && edge.Y.Equals(i.Y))
                    return false;
            }
            return true;

        }
        public void AddEdges(Edge edge)
        {
            if (exist(edge.X, edge.Y) && EdgeCheck(edge))
            {
                if (IsOriented)
                {
                    foreach (var key in G.Keys)
                    {
                        if (edge.X.Equals(key))
                        {
                            tmp_value = new Dictionary<Vertex, Weight>();
                            if (IsWeighted)
                                tmp_value.Add(edge.Y, edge.W);
                            else
                                tmp_value.Add(edge.Y, null);
                            G[key].Add(tmp_value);
                        }
                    }
                }
                else
                {
                    foreach (var key in G.Keys)
                    {
                        if (edge.X.Equals(key))
                        {
                            tmp_value = new Dictionary<Vertex, Weight>();
                            if (IsWeighted)
                            {
                                tmp_value.Add(edge.Y, edge.W);
                                //tmp_value.Add(edge.X, edge.W);
                            }
                            else
                            {
                                tmp_value.Add(edge.Y, null);
                                //tmp_value.Add(edge.X, null);

                            }
                            G[key].Add(tmp_value);
                        }

                        if (edge.Y.Equals(key))
                        {
                            tmp_value = new Dictionary<Vertex, Weight>();
                            if (IsWeighted)
                            {
                                tmp_value.Add(edge.X, edge.W);
                            }
                            else
                            {
                                tmp_value.Add(edge.X, null);

                            }
                            G[key].Add(tmp_value);
                        }
                    }
                }
                Edges.Clear();
                SetEdges();
            }
        }



        public void RemoveEdge(Vertex v1, Vertex v2)
        {
            if (exist(v1, v2))
            {
                foreach (var key in G.Keys)
                {
                    if (key.X == v1.X)
                    {
                        foreach (var list_values in G[key])
                        {
                            foreach (var value in list_values.Keys)
                                if (value.X == v2.X)
                                    list_values.Remove(value);
                        }
                    }
                    if (!IsOriented)
                    {
                        if (key.X == v2.X)
                        {
                            foreach (var list_values in G[key])
                            {
                                foreach (var value in list_values.Keys)
                                    if (value.X == v1.X)
                                        list_values.Remove(value);
                            }
                        }
                    }

                }
                Edges.Clear();
                SetEdges();
            }
        }

        private bool exist(Vertex v1, Vertex v2)
        {
            bool[] fl = new bool[2];
            int i = 0;
            foreach (var key in G.Keys)
                if (v1.X == key.X || v2.X == key.X)
                    fl[i++] = true;

            return fl[0] && fl[1];

        }

        public void WriteToNewFile()
        {
            string path = @"D:\C#\graph\Graph\result.txt";
            using (FileStream file = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter write = new StreamWriter(file))
                {
                    write.WriteLine((Convert.ToInt32(IsWeighted)).ToString());
                    write.WriteLine((Convert.ToInt32(IsOriented)).ToString());

                    foreach (var key in G.Keys)
                    {
                        write.Write($"{key.ToString()}: ");
                        foreach (var list_values in G[key])
                        {
                            if (list_values != null)
                            {
                                foreach (var value in list_values)
                                    if (IsWeighted)
                                        write.Write($"({value.Key.ToString()},{value.Value.ToString()}) ");
                                    else write.Write($"{value.Key.ToString()} ");
                            }

                        }
                        write.WriteLine();
                    }

                }
            }
        }

        public void WriteToCopyFile()
        {
            string path = @"D:\C#\graph\Graph\copy.txt";
            using (FileStream file = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter write = new StreamWriter(file))
                {
                    write.WriteLine((Convert.ToInt32(IsWeighted)).ToString());
                    write.WriteLine((Convert.ToInt32(IsOriented)).ToString());

                    foreach (var key in G.Keys)
                    {
                        write.Write($"{key.ToString()}: ");
                        foreach (var list_values in G[key])
                        {
                            if (list_values != null)
                            {
                                foreach (var value in list_values)
                                    if (IsWeighted)
                                        write.Write($"({value.Key.ToString()},{value.Value.ToString()}) ");
                                    else write.Write($"{value.Key.ToString()} ");
                            }

                        }
                        write.WriteLine();
                    }

                }
            }

        }

        #endregion

        private int currVertexExodus = default;
        public List<Vertex> HSofExodus(Vertex v)
        {
            Dictionary<Vertex, int> CountHSofExodus = new Dictionary<Vertex, int>();
            List<Vertex> res_vertices = new List<Vertex>();
            foreach (var i in G.Keys)
            {
                foreach (var j in G[i])
                {
                    CountHSofExodus.Add(i, j.Count);

                    if (i.Equals(v))
                    {
                        currVertexExodus = j.Count;
                    }
                }

            }

            foreach (var i in CountHSofExodus)
            {
                if (!i.Key.Equals(v) && i.Value > currVertexExodus)
                {
                    res_vertices.Add(i.Key);

                }
            }
            currVertexExodus = default;
            return res_vertices;
        }



        public List<Vertex> LoopInGraph()
        {
            List<Vertex> loop = new List<Vertex>();

            foreach (var i in Edges)
                if (i.X.Equals(i.Y))
                {
                    loop.Add(i.X);
                }

            return loop.Distinct().ToList<Vertex>();
        }

        public List<Vertex> NotAdjacentVerties(Vertex v)
        {
            List<Vertex> res = new List<Vertex>();
            List<Vertex> tmp = new List<Vertex>();

            foreach (var i in Edges)
            {

                if (i.X.Equals(v))
                {
                    tmp.Add(i.Y);
                }
                else if (i.Y.Equals(v))
                {
                    tmp.Add(i.X);
                }
            }

            bool fl = true;
            foreach (var i in Vertices)
            {
                if (!i.Equals(v))
                {
                    foreach (var j in tmp)
                    {
                        if (i.Equals(j))
                        {
                            fl = false;
                        }
                    }
                    if (fl)
                    {
                        res.Add(i);
                    }
                    else
                    {
                        fl = true;
                    }
                }
            }
            return res.Distinct().ToList<Vertex>();
        }


        //task2

        public void SymmetricDifference(Graph graph)
        {

            List<Vertex> tmp_v = Vertices;
            List<Edge> tmp_e = Edges;

            foreach (var i in graph.GetVertices())
            {
                if (!ContaintsVertex(i))
                    AddVertex(i);
            }

            Vertices.Distinct().ToList();

            foreach (var i in graph.GetEdges())
            {
                if (ContaintsEdge(i))
                {
                    RemoveEdge(i.X, i.Y);
                }
                else
                {
                    AddEdges(i);
                }
            }
            Edges.Distinct().ToList();
        }

        private bool ContaintsVertex(Vertex v)
        {
            foreach (var i in G.Keys)
            {
                if (i.Equals(v))
                    return true;
            }
            return false;
        }

        private bool ContaintsEdge(Edge e)
        {

            foreach (var i in Edges)
            {
                if (i.Equals(e))
                    return true;
            }

            return false;
        }


        private bool isCyclicHellper(Vertex u)
        {

            if (recVisited[u])
                return true;

            if (Visited[u])
                return false;

            Visited[u] = true;
            recVisited[u] = true;
            List<Dictionary<Vertex, Weight>> adj = G[u];
            //Console.Write($"{u.X} ");
            foreach (var i in adj)
            {
                foreach (var j in i.Keys)
                {
                    int iter = 0;
                    foreach (var k in G.Keys)
                    {
                        if (k.Equals(j))
                            break;
                        iter++;
                    }
                    if (isCyclicHellper(Visited.ElementAt(iter).Key))
                        return true;

                }
            }

            recVisited[u] = false;

            return false;

        }
        //17
        public bool isCyclic()
        {
            foreach (var i in G.Keys) { Visited.Add(i, false); }

            foreach (var i in G.Keys) { recVisited.Add(i, false); }

            foreach (var i in G.Keys)
            {
                if (isCyclicHellper(i))
                {
                    foreach (var j in G.Keys) { Visited.Remove(j); }

                    foreach (var j in G.Keys) { recVisited.Remove(j); }

                    return true;
                }
            }

            return false;
        }

        //38

        public void AllPathes(Vertex v, int depth, int _depth, List<Vertex> Path, List<List<Vertex>> pathes)
        {
            List<Dictionary<Vertex, Weight>> adj = new List<Dictionary<Vertex, Weight>>();
            foreach (var key in G.Keys)
            {
                if (key.Equals(v))
                {
                    adj = G[key];
                    break;

                }
            }
            if (depth == _depth || adj[0].Count == 0)
            {
                _depth -= 1;
                var tmp = new List<Vertex>();
                foreach (var item in Path)
                {
                    var i = new Vertex();
                    i.X = item.X;
                    tmp.Add(i);
                }
                pathes.Add(tmp);
                Path.Remove(Path.Last());

            }
            else
            {
                foreach (var i in adj)
                {

                    foreach (var vertex in i)
                    {
                        var tmpList = new List<Vertex>();
                        Path.Add(vertex.Key);
                        AllPathes(vertex.Key, depth, _depth + 1, Path, pathes);
                    }
                    if (Path.Count != 0)
                    {
                        Path.Remove(Path.Last());

                    }

                }
            }
        }
        public List<List<Vertex>> AllShortestCycles = new List<List<Vertex>>();
        public void BFS(Vertex v)
        {
            var cyclesVertex = new List<Vertex>();

            Queue<Vertex> q = new Queue<Vertex>();
            Stack<Vertex> ShortestPAth = new Stack<Vertex>();
            foreach (var i in G.Keys) { Visited.Add(i, false); }
            SP.Add(v, 0);
            q.Enqueue(v);
            Visited[v] = true;
            ///......
            int CountPath = int.MaxValue;
            Vertex shv = new Vertex();
            while (q.Count != 0)
            {
                var ver = q.Dequeue();
                foreach (var edge in Edges)
                {

                    if (edge.X.Equals(ver))
                    {
                        var r = new KeyValuePair<Vertex, bool>();
                        foreach (var adj in Visited)
                        {
                            if (adj.Key.Equals(edge.Y) && adj.Value == false)
                            {
                                r = adj;
                                q.Enqueue(edge.Y);
                                SP[edge.Y] = SP[ver] + 1;

                            }

                            if (adj.Key.Equals(edge.Y) && v.Equals(edge.Y) && adj.Value == true && SP[ver] <= CountPath)
                            {
                                shv = ver;
                                cyclesVertex.Add(ver);
                                CountPath = SP[ver];
                            }

                        }
                        if (r.Key != null)
                        {
                            Visited[r.Key] = true;

                        }
                    }
                }
            }

            ///......
            foreach (var j in G.Keys) { Visited.Remove(j); }
            if (cyclesVertex.Count != 0)
            {
                int depth = SP[shv];
                var Path = new List<Vertex>();
                var pathes = new List<List<Vertex>>();

                AllPathes(v, depth, 0, Path, pathes);
                for (int i = 0; i < pathes.Count; i++)
                {
                    pathes[i].Insert(0, v);
                }


                for (int i = pathes.Count - 1; i >= 0; i--)
                {
                    var path = pathes[i];
                    bool fl = false;
                    foreach (var j in cyclesVertex)
                    {
                        if (path.Last().Equals(j))
                        {
                            fl = true;
                            break;
                        }
                    }
                    if (!fl)
                    {
                        pathes.Remove(path);
                    }
                }

                foreach (var res in pathes)
                {
                    AllShortestCycles.Add(res);
                }
            }
        }

        public void allShortestCylces()
        {
            foreach (var vertex in Vertices)
            {
                BFS(vertex);
            }
            int min = int.MaxValue;
            foreach (var cycle in AllShortestCycles)
            {
                if (cycle.Count < min)
                {
                    min = cycle.Count;
                }
            }

            foreach (var cycle in AllShortestCycles)
            {
                if (cycle.Count == min)
                {
                    foreach (var vertex in cycle)
                    {
                        Console.Write($"{vertex.X}, ");
                    }
                    Console.WriteLine();
                }
            }
        }
        //каркас прима
        private void FixDirection()
        {
            var tmp_edges = Edges;
            for (int i = tmp_edges.Count; i > 0; --i)
            {
                var edge = tmp_edges[i - 1];
                RemoveEdge(edge.X, edge.Y);
                AddEdges(new Edge(edge.Y, edge.X, edge.W));
            }

            foreach (var i in resMST.Keys)
            {
                if (resMST[i].Count == 0)
                    resMST.Remove(i);
            }

        }
        private Dictionary<Vertex, List<Dictionary<Vertex, Weight>>> resMST = new Dictionary<Vertex, List<Dictionary<Vertex, Weight>>>();
        private bool MSTContainsHellper(Vertex v)
        {
            foreach (var i in resMST)
            {
                if (i.Key.Equals(v))
                    return true;
            }
            return false;
        }

        public void MST()
        {

            List<Edge> EdgesFrame = new List<Edge>();
            IDictionary res = resMST;
            var adj = new Dictionary<Vertex, Weight>();
            resMST.Add(G.First().Key, new List<Dictionary<Vertex, Weight>>());
            while (resMST.Count != G.Count)
            {
                var tmp = new Dictionary<Vertex, Weight>();
                var curV = new Vertex();

                foreach (var vertex in resMST.Keys)
                {

                    foreach (var edge in Edges)
                    {

                        if (vertex.Equals(edge.X) && !MSTContainsHellper(edge.Y))
                        {

                            if (tmp.Count == 0)
                            {
                                curV = vertex;
                                tmp.Add(edge.Y, edge.W);
                            }
                            else if (edge.W.W < tmp.First().Value.W)
                            {
                                curV = vertex;
                                tmp.Remove(tmp.First().Key);
                                tmp.Add(edge.Y, edge.W);
                            }

                        }


                    }


                }

                resMST[curV].Add(tmp);
                resMST.Add(tmp.First().Key, new List<Dictionary<Vertex, Weight>>());





            }

            G = resMST;
            Edges.Clear();
            Vertices.Clear();
            SetEdges();
            SetVertices();
            FixDirection();
        }

        private List<Vertex> CH = new List<Vertex>();

        private bool ContainsHellper(Vertex v)
        {
            foreach (var i in CH)
            {
                if (i.Equals(v))
                    return true;
            }
            return false;
        }

        private int ChangeBandwidth(List<Edge> e)
        {
            if (e != null)
            {
                //находим минимум
                int min = int.MaxValue;
                foreach (var edge in e)
                {
                    if (edge.W.W < min)
                        min = edge.W.W;
                }

                //меняем сеть 

                foreach (var key in G.Keys)
                {
                    foreach (var value in G[key])
                    {
                        for (int j = value.Count - 1; j >= 0; j--)
                        {
                            foreach (var edge in e)
                            {
                                var keyInDict = value.ElementAt(j).Key;
                                if (key.Equals(edge.X) && keyInDict.Equals(edge.Y))
                                {
                                    value[keyInDict].W -= min;
                                    value[keyInDict].W2 += min;
                                    break;
                                }

                            }

                        }
                    }
                }



                return min;
            }
            else return 0;
        }
        private int Iteration(Vertex f, Vertex t, Vertex curV, List<Vertex> prev, bool fl, List<Edge> e)
        {

            while (!curV.Equals(t) && fl)
            {
                List<Dictionary<Vertex, Weight>> adj = new List<Dictionary<Vertex, Weight>>();
                foreach (var key in G.Keys)
                {
                    if (key.Equals(curV))
                    {
                        adj = G[key];
                        break;
                    }
                }
                var tmp = new Dictionary<Vertex, Weight>();
                var tmpCurV = new Vertex(curV.X);
                //1 - пропускная способность > 0, 2 - она максимальна, 3- вершина не входит в множество

                foreach (var i in adj)
                {
                    foreach (var j in i.Keys)
                    {
                        var tmpValue = i[j];
                        if (tmp.Count == 0)
                        {
                            if (!ContainsHellper(j) && tmpValue.W > 0)
                            {
                                curV = j;
                                tmp.Add(j, tmpValue);
                            }
                            else continue;
                        }
                        else if (tmpValue.W > tmp.First().Value.W && tmpValue.W > 0 && !ContainsHellper(j))
                        {
                            curV = j;
                            tmp.Remove(tmp.First().Key);
                            tmp.Add(j, tmpValue);
                        }


                    }
                }

                if (tmpCurV.Equals(curV))
                {
                    if (prev.Count() != 0)
                    {
                        curV = prev.Last();
                        prev.Remove(prev.Last());
                    }
                    else
                        fl = false;
                    if (e.Count() != 0)
                        e.Remove(e.Last());
                    //Iteration(f, t, curV, prev, fl, e);
                }
                else
                {
                    CH.Add(curV);
                    e.Add(new Edge(tmpCurV, tmp.First().Key, tmp.First().Value));
                    prev.Add(tmpCurV);

                }


            }

            //изменяем остаточную сеть, и находим максимальный поток по данному пути
            if (fl)
                return ChangeBandwidth(e);
            else
                return -1;
        }
        public int Max_Flow()
        {
            //
            int maxFlow = 0;
            /// Добавить для каждых ребер дополнительное знчение веса (для обратного прохода) - complited
            /// Далее создать метод принадлежности к множеству
            /// Прописать отдельный метод изменения пропуской способности
            /// Идти по значениям будет по типу MST только будем брать не минимальное а максимальное 
            /// 

            //Для всех ребер положим остаточную пропускую способность равную первым весам ребер
            Dictionary<Vertex, List<Dictionary<Vertex, Weight>>> FlowGraph = new Dictionary<Vertex, List<Dictionary<Vertex, Weight>>>();

            foreach (var values_List in G.Values)
            {
                foreach (var item in values_List)
                {
                    for (int i = item.Count - 1; i > 0; i--)
                    {
                        var key = item.ElementAt(i).Key;
                        var new_Weight = new Weight(item[key].W, 0);
                        item[key] = new_Weight;
                    }

                }
            }

            Edges.Clear();
            Vertices.Clear();
            SetEdges();
            SetVertices();

            bool fl = true;
            Vertex f = G.Keys.ElementAt(0);
            Vertex t = G.Keys.ElementAt(G.Keys.Count - 1);

            while (fl)
            {
                var curV = f;
                CH.Add(curV);
                List<Edge> e = new List<Edge>();
                int curFlow = Iteration(f, t, curV, new List<Vertex>(), fl, e);
                if (curFlow != -1)
                {
                    maxFlow += curFlow;
                }
                else
                    fl = false;

                CH.Clear();
            }


            return maxFlow;
        }


        public List<Vertex> ShosrtestPath_Dijkstra(Vertex u, Vertex v)
        {
            //
            int from = u.X;
            int to = v.X;
            int n = Vertices.Count() + 1;
            int[] D = new int[n];
            int[] P = new int[n];

            int[,] C = new int[n, n];

            bool[] visited = new bool[n];
            foreach (var edge in Edges)
            {
                C[edge.X.X, edge.Y.X] = edge.W.W;
            }

            for (int i = 0; i < D.Length; i++)
            {
                if (C[from, i] == 0) D[i] = int.MaxValue; else D[i] = C[from, i];

                visited[i] = false;
            }

            for (int i = 0; i < n - 1; i++)
            {

                int CurMin = int.MaxValue;
                int w = int.MaxValue;
                int tmp = w;
                for (int j = 0; j < n; j++)
                {
                    if (visited[j] == false && D[j] <= CurMin)
                    {
                        CurMin = D[j];
                        w = j;
                    }
                }
                visited[w] = true;

                if (visited[to])
                {
                    break;
                }

                for (int j = 0; j < n; j++)
                {
                    if (visited[j] == false && C[w, j] != 0 /*&& D[j] != int.MaxValue*/)
                    {
                        if (D[w] + C[w, j] < D[j])
                        {
                            D[j] = D[w] + C[w, j];
                            P[j] = w;
                        }

                    }
                }


            }

            List<Vertex> ShortPath = new List<Vertex>();
            while (P[to] != 0)
            {
                ShortPath.Add(new Vertex(P[to]));
                to = P[to];
            }

            if (D[to] != int.MaxValue)
            {
                ShortPath.Add(u);
                ShortPath.Reverse();
                ShortPath.Add(v);
            }
            return ShortPath;
        }

        public int[,] Floyd()
        {

            int n = Vertices.Count();

            int[,] C = new int[n, n];

            foreach (var edge in Edges)
            {
                int i = edge.X.X;
                int j = edge.Y.X;
                C[i - 1, j - 1] = edge.W.W;
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {

                    if (C[i, j] == 0) { C[i, j] = int.MaxValue; }
                }
            }
            
            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                        for (int j = 0; j < n; j++)
                        {
                            if (C[i,j] > C[k, j] + C[i,k] && C[i,k] != int.MaxValue && C[k,j] != int.MaxValue )
                            {
                                C[i, j] = C[k, j] + C[i, k];
                            }
                        }
                }
            }

            return C;
        }
        public List<Vertex> CentreOfGraph()
        {
            int[,] C = Floyd();
            Dictionary<Vertex, int> eccentricity = new Dictionary<Vertex, int>();
            foreach (var vertex in Vertices)
            {
                int currV = vertex.X - 1;
                int max = int.MinValue;
                for (int i = 0; i < Vertices.Count; i++)
                {
                    for (int j = 0; j < Vertices.Count; j++)
                    {
                        if (j == currV && C[i, j] > max && i != j && C[i, j] != int.MaxValue)
                        {
                            max = C[i, j];
                        }
                    }
                }

                eccentricity.Add(vertex, max);

            }

            var rad = int.MaxValue;
            foreach (var ver in eccentricity.Values)
            {
                if (ver < rad && ver != int.MinValue)
                    rad = ver;
            }
            List<Vertex> Centre = new List<Vertex>();
            foreach (var vertex in eccentricity)
            {
                if (vertex.Value == rad)
                {
                    Centre.Add(vertex.Key);
                }
            }

            return Centre;

        }

        //Belman-Ford Algorithm
        
        public int[] Belman_Ford(Vertex u1)
        {
            int from = u1.X - 1;
            int n = Vertices.Count();
            int[] D = new int[n];
            int[] P = new int[n];

            for (int i = 0; i < D.Length; i++)
            {
                if (from == i) D[i] = 0; else D[i] = int.MaxValue;
            }


            for (int i = 1; i < n - 1; i++)
            {
                foreach (var edge in Edges)
                {
                    int u = edge.X.X - 1;
                    int v = edge.Y.X - 1;
                    int w = edge.W.W;

                    var tmpDist = D[u] + w;

                    if (tmpDist < D[v] && D[u] != int.MaxValue)
                    {
                        D[v] = tmpDist;
                    }
                }
            }

            //проверка на отрицательный цикл 
            bool fl= true;

                foreach (var edge in Edges)
                {
                    int u = edge.X.X - 1;
                    int v = edge.Y.X - 1;
                    int w = edge.W.W;

                    var tmpDist = D[u] + w;

                    if (tmpDist < D[v] && D[u] != int.MaxValue)
                    {
                        fl = false;

                    }
                }
    
            if (!fl)
            {
                Console.WriteLine("В графе есть циклы отрицательного веса");
            }
            return D;
        }

        public List<Vertex> DistLessN(int n, Vertex v)
        {
            List<Vertex> res = new List<Vertex>();
            foreach (var vertex in Vertices)
            {
                int[] D = Belman_Ford(vertex);
                if (D[v.X - 1] <= n)
                {
                    res.Add(vertex);
                }
            }
            return res;
        }
    }



    class Vertex
    {

        public int X { get; set; }

        public Vertex() { }
        ~Vertex() { }
        public Vertex(int x)
        {
            X = x;
        }

        public override string ToString()
        {
            return $"{X}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Vertex);
        }

        private bool Equals(Vertex other)
        {
            if (other == null) { return false; };
            return X == other.X;
        }

    }


    class Weight
    {
        public int W { get; set; }
        public int W2 { get; set; }
        public Weight() { }
        public Weight(int w)
        {
            W = w;
        }

        public Weight(int w, int w2)
        {
            W = w;
            W2 = w2;
        }

        public override string ToString()
        {
            return $"{W}";
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as Weight);
        }

        private bool Equals(Weight other)
        {
            if (other == null) { return false; };
            return W == other.W;
        }


    }

    class Edge
    {
        public Vertex X { get; set; }
        public Vertex Y { get; set; }
        public Weight W { get; set; }

        public Edge() { }
        public Edge(Vertex x, Vertex y, Weight w)
        {
            X = x;
            Y = y;
            W = w;
        }

        public override string ToString()
        {
            return $"({X}, {Y}) - {W}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Edge);
        }

        public bool Equals(Edge other)
        {
            if (other == null) { return false; };
            return X.Equals(other.X) && X.Equals(other.X) && W.Equals(other.W);

        }
    }


}