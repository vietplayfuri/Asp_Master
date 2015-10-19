namespace GoEat.Web.Models
{
    public class AdminViewModels
    {
    }

    public class OrderViewModel
    {
        public int RestaurentId { get; set; }
        public int UserId { get; set; }
        public int DiscountId { get; set; }
        public string qrUrl { get; set; }
        public string UserName { get; set; }
        public float TotalAmount { get; set; }

    }
}