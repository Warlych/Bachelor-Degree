using System.Linq.Expressions;
using Neo4j.Driver;
using RailwaySections.Domain.RailwaySections;
using RailwaySections.Domain.RailwaySections.Enums;
using RailwaySections.Domain.RailwaySections.Repositories;
using RailwaySections.Domain.RailwaySections.ValueObjects.RailwaySections;
using RailwaySections.Persistence.Abstractions;
using RailwaySections.Persistence.ExpressionBuilders;

namespace RailwaySections.Persistence.Repositories;

public class RailwaySectionRepository : IRailwaySectionRepository
{
    private readonly IRailwaySectionDatabaseContext _context;

    public RailwaySectionRepository(IRailwaySectionDatabaseContext context)
    {
        _context = context;
    }

    public async Task<RailwaySection?> GetAsync(Expression<Func<RailwaySection, bool>> predicate,
                                                CancellationToken cancellationToken = default)
    {
        await using var session = _context.AsyncSession();

        var (where, parameters) = new Neo4JExpressionBuilder().Build(predicate.Body);
        
        return await session.ExecuteReadAsync(async x =>
        {
            var query = $@"
                        MATCH (r:RailwaySection)
                        WHERE {where}
                        OPTIONAL MATCH (r)-[t:TRANSITION]->(to:RailwaySection)
                        WITH r, collect(CASE 
                            WHEN to IS NOT NULL THEN {{ to: to.id, length: t.length }} 
                            ELSE null 
                        END) AS rawTransitions
                        RETURN r, [t IN rawTransitions WHERE t IS NOT NULL] AS transitions
                        LIMIT 1";
            
            var reader = await x.RunAsync(query, parameters);

            var fetchResult = await reader.FetchAsync();

            if (!fetchResult)
            {
                return null;
            }

            var node = reader.Current["r"].As<INode>();
            var transitionList = reader.Current["transitions"].As<List<object>>();

            List<RailwaySectionTransition> transitions = [];
            if (transitionList.Any())
            {
                transitions = transitionList
                              .Select(x =>
                              {
                                  var map = (IDictionary<string, object>)x;

                                  return new RailwaySectionTransition(
                                      new RailwaySectionId(Guid.Parse(map["to"].ToString()!)),
                                      Convert.ToInt32(map["length"])
                                  );
                              })
                              .ToList();
            }

            return new RailwaySection(
                new RailwaySectionId(Guid.Parse(node["id"].As<string>())),
                (RailwaySectionTypes)node["type"].As<int>(),
                new RailwaySectionTitle(
                    node["fullName"].As<string>(),
                    node["name"].As<string>(),
                    node["mnemonic"].As<string?>()
                ),
                new RailwaySectionParameters(
                    node["railwayCode"].As<string>(),
                    node["unifiedNetworkMarking"].As<string>()
                ),
                transitions
            );
        });
    }

    public async Task<IEnumerable<RailwaySection>> GetAsync(Expression<Func<RailwaySection, bool>> predicate,
                                                            int pageNumber,
                                                            int pageSize,
                                                            CancellationToken cancellationToken = default)
    {
        await using var session = _context.AsyncSession();

        var (where, parameters) = new Neo4JExpressionBuilder().Build(predicate?.Body);

        var skip = (pageNumber - 1) * pageSize;

        parameters["skip"] = skip;
        parameters["limit"] = pageSize;

        return await session.ExecuteReadAsync(async x =>
        {
            var query = $@"
                    MATCH (r:RailwaySection)
                    WHERE {where}
                    OPTIONAL MATCH (r)-[t:TRANSITION]->(to:RailwaySection)
                    WITH r, collect(CASE 
                        WHEN to IS NOT NULL THEN {{ to: to.id, length: t.length }} 
                        ELSE null 
                    END) AS rawTransitions
                    RETURN r, [t IN rawTransitions WHERE t IS NOT NULL] AS transitions
                    ORDER BY r.id
                    SKIP $skip
                    LIMIT $limit";

            var reader = await x.RunAsync(query, parameters);
            var results = new List<RailwaySection>();

            while (await reader.FetchAsync())
            {
                var node = reader.Current["r"].As<INode>();
                var transitionList = reader.Current["transitions"].As<List<object>>();

                List<RailwaySectionTransition> transitions = [];

                if (transitionList.Any())
                {
                    transitions = transitionList
                                  .Select(x =>
                                  {
                                      var map = (IDictionary<string, object>)x;

                                      return new RailwaySectionTransition(
                                          new RailwaySectionId(Guid.Parse(map["to"].ToString()!)),
                                          Convert.ToInt32(map["length"])
                                      );
                                  })
                                  .ToList();
                }

                var railwaySection = new RailwaySection(
                    new RailwaySectionId(Guid.Parse(node["id"].As<string>())),
                    (RailwaySectionTypes)node["type"].As<int>(),
                    new RailwaySectionTitle(
                        node["fullName"].As<string>(),
                        node["name"].As<string>(),
                        node["mnemonic"].As<string?>()
                    ),
                    new RailwaySectionParameters(
                        node["railwayCode"].As<string>(),
                        node["unifiedNetworkMarking"].As<string>()
                    ),
                    transitions
                );

                results.Add(railwaySection);
            }

            return results;
        });
    }

    public async Task AddAsync(RailwaySection aggregate, CancellationToken cancellationToken = default)
    {
        await using var session = _context.AsyncSession();
        
        await session.ExecuteWriteAsync(async x =>
        {
            const string createQuery = """
                                       MERGE (r:RailwaySection { id: $id })
                                       SET r.type = $type
                                           r.name = $name,
                                           r.fullName = $fullName,
                                           r.mnemonic = $mnemonic,
                                           r.railwayCode = $railwayCode,
                                           r.unifiedNetworkMarking = $unifiedNetworkMarking
                                       """;
            
            await x.RunAsync(createQuery, new
            {
                id = aggregate.Id.ToString(),
                type = (int)aggregate.Type,
                name = aggregate.Title.Name,
                fullName = aggregate.Title.FullName,
                mnemonic = aggregate.Title.Mnemonic,
                railwayCode = aggregate.Parameters.RailwayCode,
                unifiedNetworkMarking = aggregate.Parameters.UnifiedNetworkMarking
            });
            
            foreach (var transition in aggregate.Transitions)
            {
                const string createRelationQuery = """
                                                   MATCH (from:RailwaySection { id: $fromId })
                                                   MATCH (to:RailwaySection { id: $toId })
                                                   MERGE (from)-[r:CONNECTED_TO]->(to)
                                                   SET r.length = $length
                                                   """;

                await x.RunAsync(createRelationQuery, new
                {
                    fromId = aggregate.Id.ToString(),
                    toId = transition.To.ToString(),
                    length = transition.Length
                });
            }
        });
    }


    public async Task DeleteAsync(RailwaySection aggregate, CancellationToken cancellationToken = default)
    {
        await using var session = _context.AsyncSession();

        await session.ExecuteWriteAsync(async x =>
        {
            const string query = """
                                 MATCH (r:RailwaySection { id: $id })
                                 DETACH DELETE r
                                 """;


            await x.RunAsync(query, new { id = aggregate.Id.ToString() });
        });
    }

    public async Task<(int Length, IEnumerable<RailwaySection> RailwaySections)> GetRailwaySectionLengthAsync(RailwaySection from,
                                                                                                              RailwaySection to,
                                                                                                              CancellationToken cancellationToken)
    {
        await using var session = _context.AsyncSession();

        return await session.ExecuteReadAsync(async x =>
        {
            const string query = """
                                 MATCH (start:RailwaySection {id: $fromId}), (end:RailwaySection {id: $toId})
                                 CALL gds.shortestPath.dijkstra.stream('railwayGraph', {
                                   sourceNode: start,
                                   targetNode: end,
                                   relationshipWeightProperty: 'length'
                                 })
                                 YIELD totalCost, nodeIds
                                 RETURN totalCost AS length, [nodeId IN nodeIds | gds.util.asNode(nodeId)] AS path
                                 """;

            var reader = await x.RunAsync(query, new
            {
                fromId = from.Id.ToString(),
                toId = to.Id.ToString()
            });

            var fetchResult = await reader.FetchAsync();
            
            if (!fetchResult)
            {
                throw new ArgumentNullException(nameof(RailwaySection));
            }

            var length = (double)reader.Current["length"];
            var pathNodes = ((IEnumerable<object>)reader.Current["path"]).Cast<INode>();

            var railwaySections = pathNodes.Select(node => new RailwaySection(
                                                       new RailwaySectionId(Guid.Parse(node["id"].As<string>())),
                                                       (RailwaySectionTypes)node["type"].As<int>(),
                                                       new RailwaySectionTitle(
                                                           node["fullName"].As<string>(),
                                                           node["name"].As<string>(),
                                                           node["mnemonic"].As<string?>()
                                                       ),
                                                       new RailwaySectionParameters(
                                                           node["railwayCode"].As<string>(),
                                                           node["unifiedNetworkMarking"].As<string>()
                                                       ),
                                                       Enumerable.Empty<RailwaySectionTransition>()
                                                   ));

            return ((int)length, railwaySections);
        });
    }

    public async Task BuildGraphAsync(CancellationToken cancellationToken = default)
    {
        await using var session = _context.AsyncSession();

        await session.ExecuteWriteAsync(async x =>
        {
            const string query = """
                                 CALL gds.graph.project(
                                     'railwayGraph',
                                     'RailwaySection',
                                     {
                                         CONNECTED_TO: {
                                             type: 'CONNECTED_TO',
                                             orientation: 'UNDIRECTED',
                                             properties: 'length'
                                         }
                                     }
                                 )
                                 """;

            await x.RunAsync(query);
        });
    }

    public async Task DropGraphIfExistsAsync(CancellationToken cancellationToken = default)
    {
        await using var session = _context.AsyncSession();

        var exists = await session.ExecuteReadAsync(async x =>
        {
            const string query = """CALL gds.graph.exists('railwayGraph') YIELD exists RETURN exists""";

            var result = await x.RunAsync(query);
            var record = await result.SingleAsync();

            return record["exists"].As<bool>();
        });

        if (exists)
        {
            const string query = """CALL gds.graph.drop('railwayGraph', false)""";

            await session.ExecuteWriteAsync(async x =>
            {
                await x.RunAsync(query);
            });
        }
    }
}
