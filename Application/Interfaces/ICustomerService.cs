using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    internal interface ICustomerService
    {
        public CustomerEntity Get(string customerId);

        public CustomerEntity Create(string customerId);

        public CustomerEntity Update(CustomerEntity customer);
    }
}
