namespace SmE_CommerceModels.ResponseDtos.Dashboard;

public class AdminDashboardResDto
{
    public decimal TotalRevenue { get; set; }

    public double TrendingRevenueRate { get; set; }

    public int TotalOrders { get; set; }

    public double TrendingOrdersRate { get; set; }

    public int TotalCustomers { get; set; }

    public double TrendingCustomersRate { get; set; }

    public int TotalProducts { get; set; }

    public double TrendingProductsRate { get; set; }
}
