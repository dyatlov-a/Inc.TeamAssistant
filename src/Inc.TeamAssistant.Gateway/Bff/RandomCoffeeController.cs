using Inc.TeamAssistant.WebUI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inc.TeamAssistant.Gateway.Bff;

[ApiController]
[Route("random-coffee")]
[Authorize]
public sealed class RandomCoffeeController : ControllerBase
{
    private readonly IRandomCoffeeService _randomCoffeeService;

    public RandomCoffeeController(IRandomCoffeeService randomCoffeeService)
    {
        _randomCoffeeService = randomCoffeeService ?? throw new ArgumentNullException(nameof(randomCoffeeService));
    }

    [HttpGet("chats/{botId:Guid}")]
    public async Task<IActionResult> GetChats(Guid botId, CancellationToken token)
    {
        var getChatsByBotResult = await _randomCoffeeService.GetChatsByBot(botId, token);

        return Ok(getChatsByBotResult);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(Guid botId, long chatId, int depth, CancellationToken token)
    {
        var getHistoryResult = await _randomCoffeeService.GetHistory(botId, chatId, depth, token);

        return Ok(getHistoryResult);
    }
}