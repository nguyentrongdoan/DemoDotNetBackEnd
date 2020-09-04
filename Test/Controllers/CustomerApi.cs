using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Data;

namespace Test.Controllers
{
    [ApiController]
    [Route("api/customers")]
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