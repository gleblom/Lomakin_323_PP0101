namespace DocumentManagementService.Models
{
    //Модель кастомного ребра для графа
    public class RouteEdge : QuickGraph.Edge<RouteNode>
    {
        public RouteEdge(RouteNode source, RouteNode target) : base(source, target) { }
    }
}
