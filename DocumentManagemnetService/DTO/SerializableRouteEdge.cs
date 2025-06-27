namespace DocumentManagementService.DTO
{
    //DTO ребра графа для сериализации/десериализации JSON
    public class SerializableRouteEdge
    {
        public string SourceId { get; set; }
        public string TargetId { get; set; }
    }
}
