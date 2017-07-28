using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesReport.Models.Entities
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        public int ID { get; set; }

        [Display(Name = "Номер заказа")]
        public int? OrderID { get; set; }

        [Display(Name = "Артикул товара")]
        public int? ProductID { get; set; }

        [Display(Name = "Цена реализации за единицу продукции")]
        [Column(TypeName = "money")]
        public decimal? UnitPrice { get; set; }

        [Display(Name = "Количество реализованных единиц товара")]
        public short? Quantity { get; set; }

        [Display(Name = "Скидка")]
        public float? Discount { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowTimeStamp { get; set; }

        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }
    }
}