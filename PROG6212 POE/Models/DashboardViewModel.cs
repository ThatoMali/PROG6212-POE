using System.Collections.Generic;

namespace ContractMonthlyClaimSystem.Models
{
    public class DashboardViewModel
    {
        public UserRole UserRole { get; set; }
        public int TotalClaims { get; set; }
        public int PendingApproval { get; set; }
        public int Approved { get; set; }
        public List<ClaimViewModel> RecentClaims { get; set; }
    }
}