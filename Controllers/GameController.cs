using Microsoft.AspNetCore.Mvc;
using zipgame.Models;

namespace zipgame.Controllers
{
    public class GameController : Controller
    {
        public IActionResult Index()
        {
            // Create a new randomized 5x5 game board with numbered cells.
            var board = GameBoard.CreateDefaultBoard();
            return View(board);
        }

        [HttpGet]
        public IActionResult Hint()
        {
            // For demonstration purposes, return a static hint path.
            var hintPath = new[]
            {
                new { row = 0, col = 0 },
                new { row = 0, col = 1 },
                new { row = 1, col = 1 }
            };
            return Json(hintPath);
        }

        [HttpPost]
        public IActionResult CheckSolution([FromBody] CheckSolutionRequest request)
        {
            // Validate the user's solution using both the board configuration and the user path.
            bool isValid = GameBoard.ValidateSolution(request.BoardCells, request.UserPath);
            return Json(new { valid = isValid });
        }
    }
}
