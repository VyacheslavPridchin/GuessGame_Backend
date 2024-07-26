using GuessGame.Abstractions.Grains;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GuessGame.WebAPI.Controllers;

[ApiController]
[Route("[controller]/{roomId:guid}")]
public class GameRoomController : BaseController
{
    [Serializable]
    public class Guess
    {
        public Guess(Guid playerId, int number)
        {
            PlayerId = playerId;
            Number = number;
        }

        public Guid PlayerId { get; private set; }
        public int Number { get; private set; }
    }

    private readonly IGrainFactory grainFactory;

    public GameRoomController(IGrainFactory grainFactory)
    {
        this.grainFactory = grainFactory;
    }

    [HttpPost("join")]
    public async Task<IActionResult> Join([FromBody] Payload<Guid> playerId, Guid roomId)
    {
        try
        {
            var roomGrain = grainFactory.GetGrain<IGameRoomGrain>(roomId);
            await roomGrain.Join(playerId.Data);
            return CreateResponse(true);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message, 500);
        }
    }

    [HttpPost("guess_number")]
    public async Task<IActionResult> GuessNumber([FromBody] Payload<Guess> guess, Guid roomId)
    {
        try
        {
            var roomGrain = grainFactory.GetGrain<IGameRoomGrain>(roomId);
            await roomGrain.GuessNumber(guess.Data.PlayerId, guess.Data.Number);
            return CreateResponse(true);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message, 500);
        }
    }

    [HttpGet("get_players")]
    public async Task<IActionResult> GetPlayers(Guid roomId)
    {
        try
        {
            var roomGrain = grainFactory.GetGrain<IGameRoomGrain>(roomId);
            var players = await roomGrain.GetPlayers();
            return CreateResponse(players);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message, 500);
        }
    }

    [HttpGet("get_answers")]
    public async Task<IActionResult> GetAnswers(Guid roomId)
    {
        try
        {
            var roomGrain = grainFactory.GetGrain<IGameRoomGrain>(roomId);
            var results = await roomGrain.GetAnswers();
            return CreateResponse(results);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message, 500);
        }
    }

    [HttpGet("get_current_action")]
    public async Task<IActionResult> GetCurrentAction(Guid roomId)
    {
        try
        {
            var roomGrain = grainFactory.GetGrain<IGameRoomGrain>(roomId);
            var action = await roomGrain.GetCurrentAction();
            return CreateResponse(action);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message, 500);
        }
    }

    [HttpGet("get_winners")]
    public async Task<IActionResult> GetWinners(Guid roomId)
    {
        try
        {
            var roomGrain = grainFactory.GetGrain<IGameRoomGrain>(roomId);
            var winners = await roomGrain.GetWinners();
            return CreateResponse(winners);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message, 500);
        }
    }

    [HttpGet("get_true_number")]
    public async Task<IActionResult> GetTrueNumber(Guid roomId)
    {
        try
        {
            var roomGrain = grainFactory.GetGrain<IGameRoomGrain>(roomId);
            var number = await roomGrain.GetTrueNumber();
            return CreateResponse(number);
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(ex.Message, 500);
        }
    }
}
