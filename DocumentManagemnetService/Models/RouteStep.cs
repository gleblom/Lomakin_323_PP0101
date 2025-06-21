using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagementService.Models
{
    public class RouteStep
    {
        public string Name { get; set; }
        public string Position { get; set; }

        public string Display => $"{Position} - {Name}";
    }

}
