using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Memberships.Models
{
    public class UserSubscriptionModel
    {        
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [MaxLength(2048)]
        public string Description { get; set; }

        [MaxLength(20)]
        [DisplayName("Registration Code")]
        public string RegistrationCode { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}