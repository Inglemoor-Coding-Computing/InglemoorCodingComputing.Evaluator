namespace InglemoorCodingComputing.Evaluator.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for executing code.
/// </summary>
[Authorize]
[ApiController]
[Route("evaluation")]
[Produces("application/json")]
public class EvaluationController : ControllerBase
{
    private readonly ILogger<EvaluationController> _logger;
    private readonly IAsyncCodeExecutionService _codeExecutionService;
    private readonly IUserLimitService _userLimitService;
    private readonly IResultCheckingService _resultCheckingService;
    private readonly IExecutionLoggingService _executionLoggingService;

    /// <inheritdoc/>
    public EvaluationController(ILogger<EvaluationController> logger, IAsyncCodeExecutionService codeExecutionService, IUserLimitService userLimitService, IResultCheckingService resultCheckingService, IExecutionLoggingService executionLoggingService)
    {
        _logger = logger;
        _codeExecutionService = codeExecutionService;
        _userLimitService = userLimitService;
        _resultCheckingService = resultCheckingService;
        _executionLoggingService = executionLoggingService;
    }

    /// <summary>
    /// Executes code and returns the output.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>
    /// Output from code execution
    /// </returns>
    /// <response code="200">Code execution output</response>
    /// <response code="400">User already has an execution run in progres.</response>
    [HttpPost("result")]
    public async Task<ActionResult<EvaluationResultResponse>> ResultAsync([FromBody] EvaluationResultRequest request)
    {
        var appId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (appId is null || !Guid.TryParse(appId, out var applicationid))
            return Unauthorized("Token doesn't contain id.");

        // Create an id for this evaluation run.
        var id = Guid.NewGuid();
        var start = DateTime.UtcNow;
        try
        {
            // Don't let the end user have simultaneously code execution.
            if (!_userLimitService.TryLock(request.User))
                return BadRequest("Cannot run request for a user simultaneously.");
            // Execute.
            var res = await _codeExecutionService.ExecuteAsync(new()
            {
                Id = id,
                Language = request.Language,
                Content = request.Content,
                StandardIn = request.StandardInput
            });
            await _executionLoggingService.LogSuccessAsync(applicationid, request.User, res.Instance, id, DateTime.UtcNow - start);

            return new EvaluationResultResponse
            {
                Code = res.Code,
                Output = res.Result
            };
        }
        catch (Exception e)
        {
            await _executionLoggingService.LogFailureAsync(applicationid, request.User, null, id, DateTime.UtcNow - start, e.Message);
            throw;
        }
        finally
        {
            // Release lock on user.
            _userLimitService.Release(request.User);
        }
    }

    /// <summary>
    /// Executes code and compares results to expected outputs.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Batched result.</returns>
    /// <response code="200">Code execution verification output</response>
    /// <response code="400">User already has an execution run in progres.</response>
    [HttpPost("verify")]
    public async Task<ActionResult<EvaluationVerificationResponse>> VerifyAsync([FromBody] EvaluationVerificationRequest request)
    {
        var appId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (appId is null || !Guid.TryParse(appId, out var applicationid))
            return Unauthorized("Token doesn't contain id.");

        if (request.Inputs.Count != request.Outputs.Count)
            return BadRequest("Lengths of inputs and outputs cannot differ.");
        if (request.Inputs.Count is 0)
            return BadRequest("At least one input output set required.");

        // Create an id for this evaluation run.
        var id = Guid.NewGuid();
        var start = DateTime.UtcNow;
        try
        {
            // Don't let the end user have simultaneously code execution.
            if (!_userLimitService.TryLock(request.User))
                return BadRequest("Cannot run request for a user simultaneously.");

            var c = new int[request.Inputs.Count];
            var r = new bool[request.Inputs.Count];
            var o = new string[request.Inputs.Count];
            await Task.WhenAll(Enumerable.Range(0, request.Inputs.Count).Select(async i =>
            {
                var res = await _codeExecutionService.ExecuteAsync(new()
                {
                    Id = id,
                    Language = request.Language,
                    Content = request.Code,
                    StandardIn = request.Inputs[i]
                });
                lock (c)
                {
                    c[i] = res.Code;
                    r[i] = _resultCheckingService.Verify(res.Result, request.Outputs[i]);
                    o[i] = res.Result;
                }
            }));

            await _executionLoggingService.LogSuccessAsync(applicationid, request.User, null, id, DateTime.UtcNow - start);
            return new EvaluationVerificationResponse
            {
                Outputs = o,
                Results = r,
                StatusCodes = c
            };
        }
        catch (Exception e)
        {
            await _executionLoggingService.LogFailureAsync(applicationid, request.User, null, id, DateTime.UtcNow - start, e.Message);
            throw;
        }
        finally
        {
            // Release lock on user.
            _userLimitService.Release(request.User);
        }
    }
}