using Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authorization.Repository
{
    public interface ICustomerRepository
    {
        public Customer GetCustomer(string username);

        public CustomerDetail GetCustomerDetail(string username);

        public List<Customer> GetCustomers();

    }
}
