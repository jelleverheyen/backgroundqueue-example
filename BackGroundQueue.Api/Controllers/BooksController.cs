using System.Threading.Tasks;
using BackGroundQueue.Api.Background;
using BackGroundQueue.Api.Persistence.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BackGroundQueue.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BooksController : ControllerBase
    {
        private readonly IBackgroundQueue<Book> _queue;

        public BooksController(IBackgroundQueue<Book> queue)
        {
            _queue = queue;
        }
        
        [HttpPost]
        public async Task<IActionResult> Publish([FromBody] Book book)
        {
            _queue.Enqueue(book);
            
            return Accepted();
        }
    }
}