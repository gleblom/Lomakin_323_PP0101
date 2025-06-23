using DocumentManagementService.Models;
using GraphSharp.Controls;
using QuickGraph;


namespace DocumentManagementService
{
    //Граф, который позволяет использовать кастомные узлы и ребра 
    public class RouteGraphLayout : GraphLayout<RouteNode, RouteEdge, BidirectionalGraph<RouteNode, RouteEdge>> { }
}
