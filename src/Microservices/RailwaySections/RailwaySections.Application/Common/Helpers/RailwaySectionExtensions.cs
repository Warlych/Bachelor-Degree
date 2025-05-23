using RailwaySections.Application.Common.Helpers.Graphs;
using RailwaySections.Domain.RailwaySections;

namespace RailwaySections.Application.Common.Helpers;

public static class RailwaySectionExtensions
{
    public static RailwaySectionGraph ToGraph(this IEnumerable<RailwaySection> sections)
    {
        var graph = new RailwaySectionGraph();

        foreach (var section in sections)
        {
            foreach (var transition in section.Transitions)
            {
                graph.AddEdge(section.Id, transition.To, transition.Length);
            }
        }

        return graph;
    }
}
