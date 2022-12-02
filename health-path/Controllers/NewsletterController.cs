using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace health_path.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsletterController : ControllerBase
{
    private readonly ILogger<NewsletterController> _logger;
    private readonly IDbConnection _connection;

    public NewsletterController(ILogger<NewsletterController> logger, IDbConnection connection)
    {
        _logger = logger;
        _connection = connection;
    }

    [HttpPost]
    public ActionResult Subscribe(string Email)
    {
        var inserted = _connection.Execute(@"
            
            INSERT INTO NewsletterSubscription (Email)
                        SELECT *
                        FROM ( VALUES (@Email) ) AS V(Email)
                        WHERE (SELECT COUNT(Email)
                                FROM NewsletterSubscription s
                                WHERE (s.Email LIKE @Email AND @Email NOT LIKE '%@gmail.com') 
                                    OR (REPLACE(s.Email, '.', '') LIKE REPLACE(@Email, '.', '') AND @Email LIKE '%@gmail.com')) = 0 
            
        ", new { Email = Email });

        return inserted == 0 ? Conflict("email is already subscribed") : Ok();
    }
}
