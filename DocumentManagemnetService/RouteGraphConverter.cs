using DocumentManagementService.DTO;
using DocumentManagementService.Models;
using QuickGraph;

namespace DocumentManagementService
{
    public static class RouteGraphConverter
    {
        public static RouteGraph ToDto(BidirectionalGraph<RouteNode, RouteEdge> graph)
        {
            var dto = new RouteGraph();
            var nodeMap = new Dictionary<RouteNode, string>();
            int index = 0;

            foreach (var node in graph.Vertices)
            {
                var id = $"n{index++}";
                nodeMap[node] = id;

                dto.Nodes.Add(new SerializableRouteNode
                {
                    Id = id,
                    StepNumber = node.StepNumber,
                    Name = node.Name
                });
            }

            foreach (var edge in graph.Edges)
            {
                dto.Edges.Add(new SerializableRouteEdge
                {
                    SourceId = nodeMap[edge.Source],
                    TargetId = nodeMap[edge.Target]
                });
            }

            return dto;
        }

        public static BidirectionalGraph<RouteNode, RouteEdge> FromDto(RouteGraph dto)
        {
            var graph = new BidirectionalGraph<RouteNode, RouteEdge>();
            var idToNode = new Dictionary<string, RouteNode>();

            foreach (var nodeDto in dto.Nodes)
            {
                var node = new RouteNode
                {
                    StepNumber = nodeDto.StepNumber,
                    Name = nodeDto.Name
                };
                idToNode[nodeDto.Id] = node;
                graph.AddVertex(node);
            }

            foreach (var edgeDto in dto.Edges)
            {
                if (idToNode.TryGetValue(edgeDto.SourceId, out var source) &&
                    idToNode.TryGetValue(edgeDto.TargetId, out var target))
                {
                    graph.AddEdge(new RouteEdge(source, target));
                }
            }

            return graph;
        }
    }

}
