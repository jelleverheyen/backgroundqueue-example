using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BackGroundQueue.Api.Persistence.Domain;
using Microsoft.Extensions.Logging;

namespace BackGroundQueue.Api.Persistence
{
    public interface IBookPublisher
    {
        Task Publish(Book book, CancellationToken cancellationToken = default);
    }
    
    public class BookPublisher : IBookPublisher
    {
        private readonly ILogger<BookPublisher> _logger;

        public BookPublisher(ILogger<BookPublisher> logger)
        {
            _logger = logger;
        }
        
        public async Task Publish(Book book, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Doing heavy publishing logic ...");
            
            await Task.Delay(2500, cancellationToken);
            
            _logger.LogInformation("\"{Name} by {Author}\" has been published!", book.Name, book.Author);
        }
    }
}