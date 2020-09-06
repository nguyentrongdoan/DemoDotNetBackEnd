using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Data;

namespace Test.Controllers
{
    [Authorize]
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
        
        //GET api/customers/2
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _dataContext.Customers.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(customer);
        }
    }
}