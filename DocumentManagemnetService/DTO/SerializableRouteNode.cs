using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagementService.DTO
{
    public class SerializableRouteNode
    {
        public string Id { get; set; }
        public int StepNumber { get; set; }
        public string Name { get; set; }
        public string? UserId { get; set; }
    }
}
