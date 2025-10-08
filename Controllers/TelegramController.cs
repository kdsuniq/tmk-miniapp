using Microsoft.AspNetCore.Mvc;

namespace TelegramBotApi.Controllers
{
    [ApiController]
    [Route("api/telegram")]
    public class TelegramController : ControllerBase
    {
        [HttpPost("validate")]
        public IActionResult ValidateInitData([FromBody] string initData)
        {
            // Здесь будет логика валидации initData от Telegram
            return Ok(new { isValid = true });
        }
        
        [HttpGet("user")]
        public IActionResult GetUserData()
        {
            // Получение данных пользователя из initData
            return Ok(new { user = "test_user" });
        }
    }
}