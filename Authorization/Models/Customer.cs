using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Models
{
    public class Customer
    {

        [Key]
        public string Username { get; set; }

        [Required]
        public int PortfolioId { get; set; }
        
        [Required]
        public string Password { get; set; }

        public string Email { get; set; }

    }
}
