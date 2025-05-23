namespace RailwaySections.Persistence.Settings;

public sealed class Neo4JOptions
{
    public const string SectionName = "Neo4JSettings";
    
    public Uri Neo4JConnection { get; set; }

    public string Neo4JUser { get; set; }

    public string Neo4JPassword { get; set; }

    public string Neo4JDatabase { get; set; }
}
