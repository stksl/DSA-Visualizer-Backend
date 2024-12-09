using System.Text;
using Dsa.DataStructures;
using Microsoft.AspNetCore.SignalR;

public sealed class DsaHub : Hub {
    /// <summary>
    /// runs dijkstra's algorithm on a received graph represented via an adjacency list
    /// </summary>
    /// <param name="adjList"></param>
    /// <returns></returns>
    public async Task RunDijkstra(IList<IList<KeyValuePair<int, int>>> adjList, int fromKey) 
    {
        Graph<int> g = await Task.Run(() => 
        {
            Graph<int> graph = new Graph<int>();
            for(int i = 0; i < adjList.Count; i++) 
            {
                GraphVertex<int> v = graph.AddVertex(i, unique: true);
                for(int j = 0; j < adjList[i].Count; j++) 
                {
                    GraphVertex<int> adjV = graph.AddVertex(adjList[i][j].Key, unique: true);
                    graph.AddEdge(v, adjV, adjList[i][j].Value);
                }
            }
            return graph;
        });
        GraphVertex<int>? from = g.TryGetVertex(fromKey);
        if (from == null) 
        {
            await Clients.Caller.SendAsync("err", $"{fromKey} vertex does not exist");
            return;
        }
        await g.RunDijkstra(from, async (paths) => 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Paths:\n");
            for(int i = 0; i < paths.Length; i++) 
            {
                sb.Append($"\tTo Vertex {i}: {paths[i]}\n");
            }
            await Clients.Caller.SendAsync("recv", sb.ToString());
        });
    }
}