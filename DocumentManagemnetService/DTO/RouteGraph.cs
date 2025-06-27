namespace DocumentManagementService.DTO
{
    //DTO графа для сериализации/десериализации JSON
    public class RouteGraph
    {
        public List<SerializableRouteNode> Nodes { get; set; }
        public List<SerializableRouteEdge> Edges { get; set; }
    }
}
