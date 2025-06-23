using DocumentManagementService.Models;
using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DocumentManagementService
{
    public class ApprovalRouteService
    {
        private readonly Client client;

        public ApprovalRouteService(Client client)
        {
            this.client = client;
        }
        public async Task<List<ApprovalRoute>> GetRoutesAsync()
        {
            var response = await client.From<ApprovalRoute>().Get();
            return response.Models;
        }
        public async Task<bool> SaveRouteAsync(string name, object graphData)
        {
            var userId = client.Auth.CurrentUser?.Id;
            if (userId == null)
            {
                return false;
            }
            var route = new ApprovalRoute()
            {
                Id = Guid.NewGuid(),
                Name = name,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                GraphJson = JsonSerializer.Serialize(graphData)
            };
            var response = await client.From<ApprovalRoute>().Insert(route);
            return response.Models.Count == 1;
        }
    }
}
