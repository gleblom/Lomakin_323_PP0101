
namespace DocumentManagementService.DTO
{
    //DTO узла графа для сериализации/десериализации JSON
    public class SerializableRouteNode
    {
        public string Id { get; set; }
        public int StepNumber { get; set; }
        public string Name { get; set; }
        public string? UserId { get; set; }
        public string Role {  get; set; }
    }
}
