using System.Collections.Immutable;

namespace Dsa.DataStructures;
public static class GraphExtension 
{
    public static async Task RunDijkstra<T>(this Graph<T> g, GraphVertex<T> from, 
    Func<ImmutableArray<int>, Task>? onChangeAsync) 
        where T : unmanaged 
    {
        int[] paths = new int[g.Count];
        for(int i = 0; i < paths.Length; i++) paths[i] = i == from.Key ? 0 : int.MaxValue;

        PriorityQueue<GraphVertex<T>, int> q = new PriorityQueue<GraphVertex<T>, int>();
        q.Enqueue(from, 0);
        while (q.Count > 0) 
        {
            q.TryDequeue(out GraphVertex<T>? cur, out int vertexPath);
            
            for(int i = 0; i < cur!.EdgesFrom.Count; i++) 
            {
                int currentPath = vertexPath + cur!.EdgesFrom[i].Weight;
                if (paths[cur!.EdgesFrom[i].To.Key] > currentPath) 
                {
                    paths[cur!.EdgesFrom[i].To.Key] = currentPath;
                    q.Enqueue(cur!.EdgesFrom[i].To, currentPath);
                }
            }
        }
        if (onChangeAsync != null)
            await onChangeAsync.Invoke(paths.ToImmutableArray());

    }
}