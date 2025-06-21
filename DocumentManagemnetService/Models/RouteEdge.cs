using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagementService.Models
{
    public class RouteEdge : QuickGraph.Edge<RouteNode>
    {
        public RouteEdge(RouteNode source, RouteNode target) : base(source, target) { }
    }
}
