using HHParser.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace HHParser.WEB.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ParserController : ControllerBase
    {
        private readonly HtmlLoaderService _loader;

        public ParserController(HtmlLoaderService loader)
        {
            _loader = loader;
        }

        [HttpGet]
        public void GetDate()
        {
            _loader.HtmlLoadAndParse();
        }
    }
}