using Authorization.Context;
using Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        protected readonly CustomerData _context;
        
        public CustomerRepository() {
            _context = new CustomerData();
        }

        // Returns all Customers
        public List<Customer> GetCustomers()
        {
            return _context.Customers;
        }

        // Returns customer by username
        public Customer GetCustomer(string username)
        {
            return _context.Customers.FirstOrDefault(c =>
                c.Username == username
            );
        }

        // Returns customer details by username
        public CustomerDetail GetCustomerDetail(string username)
        {
            Customer customer = GetCustomer(username);
            CustomerDetail customerDetail = null;

            if (customer != null)
            {
                customerDetail = new CustomerDetail()
                {
                    Username = customer.Username,
                    PortfolioId = customer.PortfolioId
                };
            }

            return customerDetail;
        }
    }
}
