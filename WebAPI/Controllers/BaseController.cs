using Microsoft.AspNetCore.Mvc;

namespace GuessGame.WebAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IActionResult CreateResponse<T>(T data, int statusCode = 200, string errorMessage = null)
        {
            var response = new ResponseWrapper<T>
            {
                StatusCode = statusCode,
                ErrorMessage = errorMessage,
                Data = data
            };

            return StatusCode(statusCode, response);
        }

        protected IActionResult CreateErrorResponse(string errorMessage, int statusCode)
        {
            var response = new ResponseWrapper<object>
            {
                StatusCode = statusCode,
                ErrorMessage = errorMessage,
                Data = null
            };

            return StatusCode(statusCode, response);
        }
    }

}
