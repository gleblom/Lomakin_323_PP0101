using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagementService.Models
{
    public class RouteNode
    {
        public string Title { get; set; }
        public string Role { get; set; }
        public string Display => $"{Role}\n({Title})";
    }
}
