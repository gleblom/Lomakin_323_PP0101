using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagementService.DTO
{
    public class RouteGraph
    {
        public List<SerializableRouteNode> Nodes { get; set; }
        public List<SerializableRouteEdge> Edges { get; set; }
    }
}
