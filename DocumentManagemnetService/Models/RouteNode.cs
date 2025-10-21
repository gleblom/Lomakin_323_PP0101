namespace DocumentManagementService.Models
{
    //Модель кастомного узла для графа
    public class RouteNode
    {
        public int StepNumber { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public string Display => $"{StepNumber}\n({Name})";

    }
}
