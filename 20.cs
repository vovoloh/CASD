using System;
using System.Collections.Generic;

public class Graph
{
    public int VertexCount;
    public List<int>[] List;
    public List<int>[] RevList;

    public Graph(int n)
    {
        VertexCount = n;
        List = new List<int>[n];
        RevList = new List<int>[n];
        for (int i = 0; i < n; i++)
        {
            List[i] = new List<int>();
            RevList[i] = new List<int>();
        }
    }
    public void AddEdge(int from, int to)
    {
        List[from].Add(to);
        RevList[to].Add(from);
    }
}
public class FlowGraph
{
    public int VertexCount;
    public int[,] Capacity;
    public FlowGraph(int n)
    {
        VertexCount = n;
        Capacity = new int[n, n];
    }
    public void AddEdge(int from, int to, int cap)
    {
        Capacity[from, to] += cap;
    }
}

public class Malgrange
{
    private Graph graph;
    private bool[] vis;
    private List<List<int>> comp;

    public Malgrange(Graph g)
    {
        graph = g;
    }
    private void DfsForward(int v, bool[] vis, List<int> comp)
    {
        vis[v] = true;
        comp.Add(v);

        foreach (int u in graph.List[v])
        {
            if (!vis[u])
            {
                DfsForward(u, vis, comp);
            }
        }
    }
    private void DfsBackward(int v, bool[] vis, List<int> comp)
    {
        vis[v] = true;
        comp.Add(v);

        foreach (int u in graph.RevList[v])
        {
            if (!vis[u])
            {
                DfsBackward(u, vis, comp);
            }
        }
    }

    public void AMalgrange()
    {
        comp = new List<List<int>>();
        bool[] visited = new bool[graph.VertexCount];

        for (int i = 0; i < graph.VertexCount; i++)
        {
            if (visited[i])
            {
                continue;
            }

            bool[] forwardVis = new bool[graph.VertexCount];
            List<int> forwardReach = new List<int>();
            DfsForward(i, forwardVis, forwardReach);

            bool[] backwardVis = new bool[graph.VertexCount];
            List<int> backwardReach = new List<int>();
            DfsBackward(i, backwardVis, backwardReach);

            List<int> comp = new List<int>();
            foreach (int v in forwardReach)
            {
                if (backwardVis[v])
                {
                    comp.Add(v);
                }
            }

            foreach (int v in comp)
            {
                visited[v] = true;
            }

            this.comp.Add(comp);
        }

        Console.WriteLine("\n Задача 3: Алгоритм Мальгранжа");
        Console.WriteLine("Компоненты сильной связности:");


        for (int i = 0; i < comp.Count; i++)
        {
            Console.Write("Компонента " + (i + 1) + ": { ");
            foreach (int v in comp[i])
            {
                Console.Write(v + " ");
            }
            Console.WriteLine("}");
        }
    }
   
}
public class PushRelabel
{
    private int n;
    private int[,] capacity;
    private int[,] flow;
    private int[] height;
    private int[] excess;
    private List<int>[] adjList;

    public PushRelabel(FlowGraph g)
    {
        n = g.VertexCount;
        capacity = (int[,])g.Capacity.Clone();
        flow = new int[n, n];
        height = new int[n];
        excess = new int[n];
        adjList = new List<int>[n];
        for (int i = 0; i < n; i++)
        {
            adjList[i] = new List<int>();
        }

        for (int u = 0; u < n; u++)
        {
            for (int v = 0; v < n; v++)
            {
                if (capacity[u, v] > 0)
                {
                    if (!adjList[u].Contains(v))
                    {
                        adjList[u].Add(v);
                    }
                    if (!adjList[v].Contains(u))
                    {
                        adjList[v].Add(u);
                    }
                }
            }
        }
    }
    private void Push(int u, int v)
    {
        int delta = Math.Min(excess[u], capacity[u, v] - flow[u, v]);

        if (delta <= 0)
        {
            return;
        }

        flow[u, v] += delta;
        flow[v, u] -= delta;
        excess[u] -= delta;
        excess[v] += delta;
    }
    private void Relabel(int u)
    {
        int minHeight = int.MaxValue;

        foreach (int v in adjList[u])
        {
            if (capacity[u, v] - flow[u, v] > 0)
            {
                if (height[v] < minHeight)
                {
                    minHeight = height[v];
                }
            }
        }

        height[u] = minHeight + 1;
    }
    private void Initialize(int source)
    {
        height[source] = n;

        foreach (int v in adjList[source])
        {
            int cap = capacity[source, v];
            if (cap > 0)
            {
                flow[source, v] = cap;
                flow[v, source] = -cap;
                excess[v] += cap;
                excess[source] -= cap;
            }
        }
    }

    public int MaxFlow(int source, int sink)
    {
        Initialize(source);

        bool pushed = true;

        while (pushed)
        {
            pushed = false;

            for (int u = 0; u < n; u++)
            {
                if (u == source || u == sink)
                {
                    continue;
                }

                if (excess[u] <= 0)
                {
                    continue;
                }

                bool foundPush = false;

                foreach (int v in adjList[u])
                {
                    if (capacity[u, v] - flow[u, v] > 0 && height[u] == height[v] + 1)
                    {
                        Push(u, v);
                        pushed = true;
                        foundPush = true;

                        if (excess[u] == 0)
                        {
                            break;
                        }
                    }
                }

                if (!foundPush && excess[u] > 0)
                {
                    Relabel(u);
                    pushed = true;
                }
            }
        }

        int maxFlow = 0;
        foreach (int v in adjList[source])
        {
            maxFlow += flow[source, v];
        }

        return maxFlow;
    }

    public void Print(int source, int sink)
    {
        Console.WriteLine("\nЗадача 12: Алгоритм проталкивания предпотока");
        Console.WriteLine("Максимальный поток из " + source + " в " + sink + ": " + MaxFlow(source, sink));

        Console.WriteLine("Поток по рёбрам:");
        for (int u = 0; u < n; u++)
        {
            for (int v = 0; v < n; v++)
            {
                if (capacity[u, v] > 0 && flow[u, v] > 0)
                {
                    Console.WriteLine("  " + u + " -> " + v + " : " + flow[u, v] + " / " + capacity[u, v]);
                }
            }
        }
    }
}
public class BronKerbosch
{
    private int n;
    private bool[,] adjMatrix;
    private List<int> maxClique;

    public BronKerbosch(int n)
    {
        this.n = n;
        adjMatrix = new bool[n, n];
        maxClique = new List<int>();
    }

    public void AddEdge(int u, int v)
    {
        adjMatrix[u, v] = true;
        adjMatrix[v, u] = true;
    }

    private List<int> Intersect(List<int> a, List<int> b)
    {
        List<int> result = new List<int>();

        foreach (int v in a)
        {
            if (b.Contains(v))
            {
                result.Add(v);
            }
        }

        return result;
    }

    private List<int> Neighbors(int v, List<int> candidates)
    {
        List<int> result = new List<int>();

        foreach (int u in candidates)
        {
            if (adjMatrix[v, u])
            {
                result.Add(u);
            }
        }

        return result;
    }

    private void BronKerboschRecursive(List<int> R, List<int> P, List<int> X)
    {


        if (P.Count == 0 && X.Count == 0)
        {
            if (R.Count > maxClique.Count)
            {
                maxClique = new List<int>(R);
            }
            return;
        }

        List<int> candidates = new List<int>(P);

        foreach (int v in candidates)
        {
            List<int> newR = new List<int>(R);
            newR.Add(v);

            List<int> newP = Intersect(P, Neighbors(v, P));
            List<int> newX = Intersect(X, Neighbors(v, X));

            BronKerboschRecursive(newR, newP, newX);

            P.Remove(v);
            X.Add(v);
        }
    }

    public List<int> FindMaxClique()
    {
        List<int> R = new List<int>();
        List<int> P = new List<int>();
        List<int> X = new List<int>();

        for (int i = 0; i < n; i++)
        {
            P.Add(i);
        }

        BronKerboschRecursive(R, P, X);

        return maxClique;
    }

    public void Print()
    {
        Console.WriteLine("\nЗадача 15: Алгоритм Брона-Кербоша");

        List<int> clique = FindMaxClique();

        Console.Write("Максимальная клика: { ");
        foreach (int v in clique)
        {
            Console.Write(v + " ");
        }
        Console.WriteLine("}");
        Console.WriteLine("Размер клики: " + clique.Count);
    }
}
public class Program
{
    public static void Main()
    {
        Console.WriteLine("N=3");
        Console.WriteLine("Задачи: 3, 12, 15");
        Console.WriteLine("\nГраф для задачи 3");
        Console.WriteLine("Вершины: 0,1,2,3,4,5");
        Console.WriteLine("Рёбра: 0->1, 1->2, 2->0, 3->4, 4->3, 1->3, 2->5");
        Graph g1 = new Graph(6);
        g1.AddEdge(0, 1);
        g1.AddEdge(1, 2);
        g1.AddEdge(2, 0);
        g1.AddEdge(3, 4);
        g1.AddEdge(4, 3);
        g1.AddEdge(1, 3);
        g1.AddEdge(2, 5);
        Malgrange malgrange = new Malgrange(g1);
        malgrange.AMalgrange();


        Console.WriteLine("\nГраф для задачи 12");
        Console.WriteLine("Вершины: 0(источник),1,2,3,4,5(сток)");
        Console.WriteLine("Рёбра с пропускными способностями:");
        Console.WriteLine("  0->1:16, 0->2:13, 1->2:10, 1->3:12");
        Console.WriteLine("  2->1:4,  2->4:14, 3->2:9,  3->5:20");
        Console.WriteLine("  4->3:7,  4->5:4");
        FlowGraph fg = new FlowGraph(6);
        fg.AddEdge(0, 1, 16);
        fg.AddEdge(0, 2, 13);
        fg.AddEdge(1, 2, 10);
        fg.AddEdge(1, 3, 12);
        fg.AddEdge(2, 1, 4);
        fg.AddEdge(2, 4, 14);
        fg.AddEdge(3, 2, 9);
        fg.AddEdge(3, 5, 20);
        fg.AddEdge(4, 3, 7);
        fg.AddEdge(4, 5, 4);
        PushRelabel pushRelabel = new PushRelabel(fg);
        pushRelabel.Print(0, 5);

        Console.WriteLine("\nГраф для задачи 15");
        Console.WriteLine("Вершины: 0,1,2,3,4,5");
        Console.WriteLine("Рёбра: 0-1, 0-2, 0-3, 1-2, 1-3, 2-3, 3-4, 4-5");

        BronKerbosch bk = new BronKerbosch(6);
        bk.AddEdge(0, 1);
        bk.AddEdge(0, 2);
        bk.AddEdge(0, 3);
        bk.AddEdge(1, 2);
        bk.AddEdge(1, 3);
        bk.AddEdge(2, 3);
        bk.AddEdge(3, 4);
        bk.AddEdge(4, 5);
        bk.Print();
    }
}
