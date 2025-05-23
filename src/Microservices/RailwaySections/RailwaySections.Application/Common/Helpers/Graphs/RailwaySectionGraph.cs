using RailwaySections.Domain.RailwaySections.ValueObjects.RailwaySections;

namespace RailwaySections.Application.Common.Helpers.Graphs;

public sealed class RailwaySectionGraph
{
    private readonly Dictionary<RailwaySectionId, List<(RailwaySectionId To, int Length)>> _adjacency = [];

    public void AddEdge(RailwaySectionId from, RailwaySectionId to, int length)
    {
        if (!_adjacency.ContainsKey(from))
        {
            _adjacency[from] = new();
        }

        _adjacency[from].Add((to, length));
    }

    public int? CalculateDistance(RailwaySectionId from, RailwaySectionId to)
    {
        var distances = new Dictionary<RailwaySectionId, int>();
        var visited = new HashSet<RailwaySectionId>();
        var queue = new PriorityQueue<RailwaySectionId, int>();

        foreach (var node in _adjacency.Keys)
        {
            distances[node] = Int32.MaxValue;
        }

        distances[from] = 0;
        queue.Enqueue(from, 0);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.Equals(to))
            {
                return distances[current];
            }

            if (!visited.Add(current) || !_adjacency.TryGetValue(current, out var neighbors))
            {
                continue;
            }

            foreach (var (neighbor, length) in neighbors)
            {
                var newDist = distances[current] + length;
                
                if (newDist < distances.GetValueOrDefault(neighbor, Int32.MaxValue))
                {
                    distances[neighbor] = newDist;
                    queue.Enqueue(neighbor, newDist);
                }
            }
        }

        return null;
    }
}
