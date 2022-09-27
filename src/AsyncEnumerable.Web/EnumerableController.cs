using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace AsyncEnumerable.Web;

[ApiController]
[Route("enumerators")]
[Produces(System.Net.Mime.MediaTypeNames.Application.Json)]
public class EnumerableController : ControllerBase
{
    private const int LIMIT = 5;

    [HttpGet]
    [Route("enumerate")]
    public async Task<IActionResult> EnumerateAsync([FromQuery] int page = 0)
    {
        await Task.Delay(2000);

        var startPoint = page * LIMIT;

        var points = Enumerable
            .Range(startPoint, LIMIT)
            .Select(x => new Response
            {
                Value = x
            });

        return Ok(points);
    }

    [HttpGet]
    [Route("enumerate-async")]
    public IAsyncEnumerable<int> AsyncEnumerateAsync([FromQuery] int page = 0)
    {
        var startPoint = page * LIMIT;

        return EnumerateAsync(startPoint, startPoint + LIMIT);
    }

    [HttpGet]
    [Route("cancel-enumerate-async")]
    public IAsyncEnumerable<int> CancelAsyncEnumerateAsync([FromQuery] int page = 0, CancellationToken cancellationToken = default)
    {
        var startPoint = page * LIMIT;

        return EnumerateAsync(startPoint, startPoint + LIMIT, cancellationToken);
    }

    private static async IAsyncEnumerable<int> EnumerateAsync(
        int startPoint,
        int iterations)
    {
        for (int i = startPoint; i <= iterations; i++)
        {
            await Task.Delay(2000);

            yield return i;
        }
    }

    private static async IAsyncEnumerable<int> EnumerateAsync(
        int startPoint,
        int iterations,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            for (int i = startPoint; i <= iterations; i++)
            {
                await Task.Delay(2000, cancellationToken);

                yield return i;
            }
        }
    }
}

public class Response
{
    [JsonPropertyName("value")]
    public int Value { get; set; }
}
