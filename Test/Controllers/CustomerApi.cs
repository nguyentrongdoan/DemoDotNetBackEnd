﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Data;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerApi: ControllerBase
    {
        private readonly DataContext _dataContext;

        public CustomerApi(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        //GET api/customers
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _dataContext.Customers.ToListAsync();
            return Ok(customers);
        }
    }
}