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

    [HttpGet("templates")]
    public async Task<IActionResult> GetTemplates(CancellationToken token)
    {
        return Ok(await _surveyService.GetSurveyTemplates(token));
    }
    
    [HttpPost]
    public async Task<IActionResult> StartSurvey([FromBody]StartSurveyCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _surveyService.StartSurvey(command);
        
        return Ok();
    }
}