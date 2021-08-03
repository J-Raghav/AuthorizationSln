using Authorization.Context;
using NUnit.Framework;
using Authorization.Repository;
using Authorization.Models;
using System.Collections.Generic;


namespace AuthorizationTest
{
    class RepositoryTests
    {

        CustomerData context;
        ICustomerRepository customerRepository;

        [SetUp]
        public void Setup()
        {
            context = new CustomerData();
            customerRepository = new CustomerRepository();            
        }


        #region Customer Repository Tests
        [Test]
        public void GetCustomers_ReturnsCustomerList()
        {
            List<Customer> customers = customerRepository.GetCustomers();

            Assert.IsNotNull(customers);
            Assert.IsInstanceOf<List<Customer>>(customers);
        }

        [Test]
        public void GetCustomer_ValidUsername_ReturnsCustomer() {
            Customer customer = customerRepository.GetCustomer("t1");

            Assert.IsNotNull(customer);
            Assert.AreEqual(customer.Username, "t1");
        }

        [Test]
        public void GetCustomer_InvalidUsername_ReturnsCustomer()
        {
            Customer customer = customerRepository.GetCustomer("Ram");

            Assert.IsNull(customer);
        }

        [Test]
        public void GetCustomerDetail_ValidUsername_ReturnsCustomerDetail()
        {
            CustomerDetail customerDetail = customerRepository.GetCustomerDetail("t1");

            Assert.IsNotNull(customerDetail);
            Assert.AreEqual(customerDetail.Username, "t1");
        }

        [Test]
        public void GetCustomerDetail_InvalidUsername_ReturnsCustomerDetail()
        {
            CustomerDetail customerDetail = customerRepository.GetCustomerDetail("Ram");

            Assert.IsNull(customerDetail);
        }
        #endregion
    }
}
