using System.Collections.Generic;
using PROG6212_POE.Models.Entities;

namespace PROG6212_POE.Models
{
    public class DashboardViewModel
    {
        public UserType UserRole { get; set; }
        public int TotalClaims { get; set; }
        public int PendingApproval { get; set; }
        public int Approved { get; set; }
        public List<PROG6212_POE.Models.Entities.Claim> RecentClaims { get; set; }
    }
}