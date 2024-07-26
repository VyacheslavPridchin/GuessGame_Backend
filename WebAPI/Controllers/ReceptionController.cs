using GuessGame.Abstractions.Grains;
using GuessGame.Abstractions.States;
using Microsoft.AspNetCore.Mvc;

namespace GuessGame.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ReceptionController : BaseController
{
    private readonly IGrainFactory grainFactory;

    public ReceptionController(IGrainFactory grainFactory)
    {
        this.grainFactory = grainFactory;
    }

    [HttpPost("check_in")]
    public async Task<IActionResult> CheckIn([FromBody] Payload<string> nickname)
    {
        try
        {
            var receptionGrain = grainFactory.GetGrain<IReceptionGrain>(0);
            Guid id = await receptionGrain.CheckIn(nickname.Data);
            return CreateResponse(id);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message, 500);
        }
    }

    [HttpPost("check_out")]
    public async Task<IActionResult> CheckOut(Payload<Guid> playerId)
    {
        try
        {
            var receptionGrain = grainFactory.GetGrain<IReceptionGrain>(0);
            await receptionGrain.CheckOut(playerId.Data);
            return CreateResponse(true);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message, 500);
        }
    }

    [HttpGet("get_players")]
    public async Task<IActionResult> GetPlayers()
    {
        try
        {
            var queueGrain = grainFactory.GetGrain<IReceptionGrain>(0);
            var players = await queueGrain.GetPlayers();
            return CreateResponse(players);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message, 500);
        }
    }

    [HttpGet("get_score")]
    public async Task<IActionResult> GetScore()
    {
        try
        {
            var queueGrain = grainFactory.GetGrain<IReceptionGrain>(0);
            var score = await queueGrain.GetScore();
            return CreateResponse(score);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message, 500);
        }
    }

    [HttpGet("get_player")]
    public async Task<IActionResult> GetPlayer(Guid playerId)
    {
        try
        {
            var queueGrain = grainFactory.GetGrain<IReceptionGrain>(0);
            PlayerState player = await queueGrain.GetPlayer(playerId);
            return CreateResponse(player);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message, 500);
        }
    }
}