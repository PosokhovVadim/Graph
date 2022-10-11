using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Graph
{
    class Graph
    {
        #region fields

        private bool IsWeighted { get; set; } // взвешанный или нет
        private bool IsOriented { get; set; } //ориентированный или нет

        public bool _isweighted;
        public virtual bool isweighted { 
            get { return _isweighted; }
            set { _isweighted = value; }
        }
        private string Path;
        private bool fl;
        //Vertex V = new Vertex();
        
        //List<Edge> E = new List<Edge>();


        private Dictionary<Vertex, List<Dictionary<Vertex, Weight>>> G = new Dictionary<Vertex, List<Dictionary<Vertex, Weight>>>();

        private List<Edge> Edges = new List<Edge>();
        private List<Vertex> Vertices = new List<Vertex>();

        private List<Dictionary<Vertex, Weight>> V_list = new List<Dictionary<Vertex, Weight>>();
        private Dictionary<Vertex, Weight> tmp_value = new Dictionary<Vertex, Weight>();

        Vertex x = new Vertex();
        Weight w = new Weight();

        
        #endregion

        #region constructors
        public Graph() { }

        
        public Graph(string path) {
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
            GetEdges();
            WriteToCopyFile();
        }

        #endregion

        #region Methods 

        protected List<Edge> GetEdges() {

            foreach (var key in G.Keys)
            {
                foreach (var list_values in G[key])
                {
                    foreach (var value in list_values) {
                        Edges.Add( new Edge(key, value.Key, value.Value));
                    }
                }
            }

            return Edges;
        }


        protected List<Vertex> GetVertices()
        {

            foreach (var key in G.Keys)
                Vertices.Add(key);
           

            return Vertices;
        }

        public void AddVertex(Vertex v) {
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
            GetVertices();
        }

        public void RemoveVertex(Vertex v) {
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
            GetVertices();
        }


        private bool EdgeCheck(Edge edge) {
            foreach (var i in Edges) {
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
                GetEdges();
            }
        }



        public void RemoveEdge(Vertex v1, Vertex v2) {
            if (exist(v1,v2)) {
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
                GetEdges();
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

        public void WriteToNewFile() {
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

        public void WriteToCopyFile() {
            string path = @"D:\C#\graph\Graph\copy.txt";
            using (FileStream file = new FileStream(path, FileMode.Create)) {
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


        

    }

    class Vertex{

        public int X { get; set; }

        public Vertex() { }
        ~Vertex() { }
        public Vertex(int x) {
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


    class Weight {
        public int W { get; set; }

        public Weight() { }
        public Weight(int w)
        {
            W = w;
        }

        public override string ToString()
        {
            return $"{W}";
        }


    }
       
    class Edge
    {
        public Vertex X { get; set; } 
        public Vertex Y { get; set; } 
        public Weight W { get; set; }

        public Edge(Vertex x, Vertex y, Weight w) {
            X = x;
            Y = y;
            W = w;
        }

        public override string ToString() {
            return $"({X}, {Y}) - {W}";
        }
    }


}
