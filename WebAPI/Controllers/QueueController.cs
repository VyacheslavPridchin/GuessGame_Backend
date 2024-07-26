using GuessGame.Abstractions.Grains;
using Microsoft.AspNetCore.Mvc;

namespace GuessGame.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class QueueController : BaseController
{
    private readonly IGrainFactory grainFactory;

    public QueueController(IGrainFactory grainFactory)
    {
        this.grainFactory = grainFactory;
    }

    [HttpPost("join")]
    public async Task<IActionResult> Join(Payload<Guid> playerId)
    {
        try
        {
            var queueGrain = grainFactory.GetGrain<IQueueGrain>(0);
            await queueGrain.Join(playerId.Data);
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
            var queueGrain = grainFactory.GetGrain<IQueueGrain>(0);
            var players = await queueGrain.GetQueue();
            return CreateResponse(players);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message, 500);
        }
    }

    [HttpGet("get_invitation")]
    public async Task<IActionResult> GetInvitation(Guid playerId)
    {
        try
        {
            var queueGrain = grainFactory.GetGrain<IQueueGrain>(0);
            Guid roomID = await queueGrain.GetInvitation(playerId);
            return CreateResponse(roomID);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message, 500);
        }
    }

    [HttpPost("leave")]
    public async Task<IActionResult> Leave(Payload<Guid> playerId)
    {
        try
        {
            var queueGrain = grainFactory.GetGrain<IQueueGrain>(0);
            await queueGrain.Leave(playerId.Data);
            return CreateResponse(true);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message, 500);
        }
    }
}
