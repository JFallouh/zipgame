using Microsoft.AspNetCore.Mvc;
using zipgame.Models;

namespace zipgame.Controllers
{
    public class GameController : Controller
    {
        // GET: /Game/Index
        public IActionResult Index()
        {
            // Create a new puzzle board using a random Hamiltonian path.
            var board = GameBoard.CreateDefaultBoard();
            return View(board);
        }
    }
}
