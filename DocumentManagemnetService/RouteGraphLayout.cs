using DocumentManagementService.Models;
using GraphSharp.Controls;
using QuickGraph;


namespace DocumentManagementService
{
    public class RouteGraphLayout : GraphLayout<RouteNode, RouteEdge, BidirectionalGraph<RouteNode, RouteEdge>> { }
}
