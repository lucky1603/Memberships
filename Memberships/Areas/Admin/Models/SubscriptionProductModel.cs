﻿using Memberships.Entities;
using System.Collections.Generic;
using System.ComponentModel;

namespace Memberships.Areas.Admin.Models
{
    public class SubscriptionProductModel
    {
        [DisplayName("Product Id")]
        public int ProductId { get; set; }
        [DisplayName("Subscription Id")]
        public int SubscriptionId { get; set; }
        [DisplayName("Product")]
        public string ProductTitle { get; set; }
        [DisplayName("Subscription")]
        public string SubscriptionTitle { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
    }
}