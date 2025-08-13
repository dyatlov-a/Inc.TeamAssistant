using Inc.TeamAssistant.Survey.Model.Commands.FinishSurvey;
using Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;
using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Bff;

[ApiController]
[Route("survey")]
[Authorize]
public sealed class SurveyController : ControllerBase
{
    private readonly ISurveyService _surveyService;

    public SurveyController(ISurveyService surveyService)
    {
        _surveyService = surveyService ?? throw new ArgumentNullException(nameof(surveyService));
    }
    
    [HttpGet("{roomId:guid}/state")]
    public async Task<IActionResult> GetPersonSurveys(Guid roomId, CancellationToken token)
    {
        return Ok(await _surveyService.GetSurveyState(roomId, token));
    }
    
    [HttpGet("{roomId:guid}/summary/{limit:int}")]
    public async Task<IActionResult> GetPersonSurveys(Guid roomId, int limit, CancellationToken token)
    {
        return Ok(await _surveyService.GetSurveySummary(roomId, limit, token));
    }
    
    [HttpPost]
    public async Task<IActionResult> StartSurvey([FromBody]StartSurveyCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _surveyService.Start(command);
        
        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> FinishSurvey([FromBody]FinishSurveyCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _surveyService.Finish(command);
        
        return Ok();
    }
}