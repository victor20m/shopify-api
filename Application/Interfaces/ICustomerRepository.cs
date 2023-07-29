using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    internal interface ICustomerRepository
    {
        public CustomerEntity Get(string customerId);

        public void Create(string customerId);

        public void Update(CustomerEntity customer);

    }
}
