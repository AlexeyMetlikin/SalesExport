using SalesReport.Models.Entities;
using System.Linq;

namespace SalesReport.Models.Abstract
{
    public interface ISalesContainer
    {
        IQueryable<Product> Products { get; }

        IQueryable<Order> Orders { get; }

        IQueryable<OrderDetail> OrderDetails { get; }
    }
}