﻿namespace CellPhoneB_Store.Models
{
	public class OrderModel
	{
		public int Id { get; set; }
		public string OrderCode { get; set; }
		public string UserName { get; set; }
		public decimal ShippingCost { get; set; }
        public string CouponCode { get; set; }
        public DateTime CreateDate { get; set; }

        public int status { get; set; }
	}
}
