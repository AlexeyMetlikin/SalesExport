using SalesReport.Models.Abstract;
using SalesReport.Models.Entities;
using System.Linq;

namespace SalesReport.Models.Context
{
    public class EFSalesContainer : ISalesContainer
    {
        private EFDbContext _context = new EFDbContext();

        public IQueryable<Product> Products
        {
            get { return _context.Products; }
        }

        public IQueryable<Order> Orders
        {
            get { return _context.Orders; }
        }

        public IQueryable<OrderDetail> OrderDetails
        {
            get { return _context.OrderDetails; }
        }
    }
}