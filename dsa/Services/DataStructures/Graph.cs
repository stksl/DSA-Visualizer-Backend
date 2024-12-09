
using System.Collections;
using System.Collections.Immutable;

namespace Dsa.DataStructures;
public class Graph<T> : IDataStructure, IEnumerable<GraphVertex<T>>
    where T : unmanaged
{
    internal virtual HashSet<GraphEdge<T>> _edges { get; set; } = new HashSet<GraphEdge<T>>();
    internal virtual HashSet<GraphVertex<T>> _vertices { get; set; } = new HashSet<GraphVertex<T>>();

    public int Count => _vertices.Count;
    protected int counter = 0; // used with increment to always allocate unique vertex key
    /// <summary>
    /// Adds a vertex and returns its unique key
    /// </summary>
    /// <param name="vertexVal"></param>
    /// <returns></returns>
    public virtual GraphVertex<T> AddVertex(T val, bool unique)
    {
        if (unique) 
        {
            var v = TryGetVertex((int)(object)val);
            if (v != null)
            return v;
        }
        GraphVertex<T> vertex = new GraphVertex<T>(counter++, val);
        _vertices.Add(vertex);

        return vertex;
    }

    /// <summary>
    /// Adds unique <paramref name="edge"/> 
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public virtual GraphEdge<T>? AddEdge(GraphVertex<T> from, GraphVertex<T> to, int weight)
    {
        var edge = new GraphEdge<T>() { From = from, To = to, Weight = weight };
        from.EdgesFrom.Add(edge);
        to.EdgesTo.Add(edge);

        if (_edges.TryGetValue(edge, out GraphEdge<T>? existing))
            return existing;
        
        _edges.Add(edge);
        return edge;
    }

    public virtual bool ContainsEdge(GraphEdge<T> edge)
    {
        return _edges.Contains(edge);
    }
    public virtual bool ContainsVertex(GraphVertex<T> v) => _vertices.Contains(v);
    public virtual bool RemoveEdge(GraphEdge<T> edge)
    {
        if (!ContainsEdge(edge)) return false;

        if (ContainsVertex(edge.From)) 
            edge.From.EdgesFrom.Remove(edge);
        if (ContainsVertex(edge.To))
            edge.To.EdgesTo.Remove(edge);

        return _edges.Remove(edge);
    }
    public virtual bool RemoveVertex(GraphVertex<T> v)
    {
        if (!ContainsVertex(v)) return false;

        foreach(var edgeFrom in v.EdgesFrom.ToImmutableArray()) RemoveEdge(edgeFrom);
        foreach(var edgeTo in v.EdgesTo.ToImmutableArray()) RemoveEdge(edgeTo);
        
        return _vertices.Remove(v);
    }

    public GraphVertex<T>? TryGetVertex(int key) 
    {
        _vertices.TryGetValue(new GraphVertex<T>(key, default), out GraphVertex<T>? actual);
        return actual;
    }
    public IEnumerator<GraphVertex<T>> GetEnumerator() => _vertices.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class GraphVertex<T>
    where T : unmanaged
{
    public readonly int Key;
    public readonly T Value;
    /// <summary>
    /// All edges starting from the vertex
    /// </summary>
    public IList<GraphEdge<T>> EdgesFrom { get; internal set; } = new List<GraphEdge<T>>();
    /// <summary>
    /// All edges ending in the vertex
    /// </summary>
    public IList<GraphEdge<T>> EdgesTo { get; internal set; } = new List<GraphEdge<T>>();
    public GraphVertex(int key, T val)
    {
        Value = val;
        Key = key;
    }
    public override bool Equals(object? obj)
    {
        return obj is GraphVertex<T> v && v.GetHashCode() == GetHashCode();
    }
    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }
}
public sealed class GraphEdge<T> where T : unmanaged
{
    public required GraphVertex<T> From { get; init; }
    public required GraphVertex<T> To { get; init; }
    public int Weight { get; init; }

    public override int GetHashCode()
    {
        return HashCode.Combine(From.GetHashCode(), To.GetHashCode(), Weight);
    }
    public override bool Equals(object? obj)
    {
        return obj is GraphEdge<T> e && e.GetHashCode() == GetHashCode();
    }
}