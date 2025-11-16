using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;

namespace ASP_KT8.Controllers
{
    public class ToolsController : Controller
    {
        private static readonly HttpClient _http = new HttpClient();

        public async Task<IActionResult> Solve(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return BadRequest("URL required!");
            
            var html = await _http.GetStringAsync(url);
            var title = Regex.Match(html, @"<title>(.*?)</title>", RegexOptions.Singleline)
                .Groups[1]
                .Value
                .Trim();
            var questions = Regex.Match(html, @"<div[^>]*class=""prob_maindiv[^""]*""[^>]*>(.*?)</div>", RegexOptions.Singleline);
            var questionsRow = questions.Success ? questions.Groups[1].Value : "";
            var questionsText = Regex.Replace(questionsRow, "<.*?>", " ")
                .Replace("&nbsp", " ")
                .Trim();

            return Json(new { Title = title, FirstQuestion = questionsText });
        }
    }
}
