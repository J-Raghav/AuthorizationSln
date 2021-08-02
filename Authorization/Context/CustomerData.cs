using Authorization.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Context
{
    public class CustomerData 
    {

        public List<Customer> Customers = new List<Customer>() {
                new Customer(){ Username = "t1", Password = "123", PortfolioId=1 },
                new Customer(){ Username = "t2", Password = "456", PortfolioId=2 },
                new Customer(){ Username = "t3", Password = "789", PortfolioId=3 },
                new Customer(){ Username = "t4", Password = "123", PortfolioId=4 },
                new Customer(){ Username = "t5", Password = "456", PortfolioId=5 },
            };

        public Customer GetByUsername(string username)
        {
            return Customers.FirstOrDefault(c =>
                c.Username == username
            );
        }

        public CustomerDetail GetDetailByUsername(string username)
        {
            Customer customer = GetByUsername(username);

            return new CustomerDetail()
            {
                Username = customer.Username,
                PortfolioId = customer.PortfolioId
            };
        }
    }
}
