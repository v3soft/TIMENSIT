using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Timensit_API.Classes;

[ApiController]
public class ErrorController : ControllerBase
{
    [Route("error")]
    protected IActionResult Error()
    {

        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        string noidung = context.Error.Message;
        AutoSendMail.SendErrorReport(noidung);

        return Problem();
    }
}
