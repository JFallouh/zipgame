# ZipGame

ZipGame is a web-based puzzle game built using ASP.NET Core MVC. In this game, players must draw a single continuous path that visits every cell in the grid exactly once, while ensuring that the numbered cells are visited in ascending order. Walls are placed between cells to add extra complexity, and the board size (and difficulty) is randomized with each new game.

## Features

- **Dynamic Board Generation:**  
  The board size is randomly chosen between 5×5, 6×6, and 7×7. The difficulty is labeled as _Easy_, _Medium_, or _Hard_ based on the board size.
- **Unique Hamiltonian Path:**  
  A random Hamiltonian path is generated (using DFS with a fallback to a snake path) to create a unique valid solution for each puzzle.
- **Even Distribution of Numbered Cells:**  
  Exactly one numbered cell is placed in each row and each column so that any cell has at least one numbered cell in its row and its column. The numbered cells must be visited in ascending order.
- **Wall Obstacles:**  
  Walls are randomly generated between cells (with a probability that depends on board size) and are drawn in a distinct style (e.g., dark purple, dashed, and thicker) to clearly differentiate them from the grid lines.
- **Responsive Gameplay:**  
  Designed to work on both desktop and mobile devices using mouse and touch events.
- **Real-Time Validation:**  
  The drawn path is continuously validated. Once a valid Hamiltonian path is drawn (meeting the puzzle conditions), the game displays a congratulatory message along with the time taken to solve the puzzle.
- **Game Controls:**  
  - **Start Over:** Clears the current drawn path (without generating a new board).  
  - **New Game:** Generates a completely new puzzle with a different board size and solution.

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- A modern web browser

### Installation

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/yourusername/ZipGame.git
   cd ZipGame
   ```

2. **Restore NuGet Packages:**

   ```bash
   dotnet restore
   ```

3. **Build the Project:**

   ```bash
   dotnet build
   ```

4. **Run the Project:**

   ```bash
   dotnet run
   ```

5. **Open Your Browser:**

   Navigate to `https://localhost:7024` or `http://localhost:5261`.

## Project Structure

```
ZipGame/
├── Controllers/
│   └── GameController.cs        // MVC controller for game actions.
├── Models/
│   └── GameBoard.cs             // Logic for board generation, walls, and numbered cells.
├── Views/
│   ├── Game/
│   │   └── Index.cshtml         // Main view for the game.
│   └── Shared/
│       └── _Layout.cshtml       // Layout view including header/footer.
├── wwwroot/
│   ├── css/
│   │   └── site.css             // Styling for the game.
│   └── js/
│       └── game.js              // Client-side game logic.
├── Properties/
│   └── launchSettings.json      // Launch settings for debugging.
├── Program.cs                   // Application entry point.
└── zipgame.csproj               // Project file.
```

## How to Play

- **Draw a Path:**  
  Click (or touch) and drag on the grid to create a continuous path that covers every cell exactly once.
- **Numbered Cells Order:**  
  Ensure the numbered cells (one per row and one per column) are visited in ascending order.
- **Walls:**  
  Walls are obstacles that prevent movement between adjacent cells. You must navigate around them.
- **Game Completion:**  
  When the correct path is drawn, the game stops the timer and displays a "Great Job!" message with your completion time.
- **Controls:**  
  - **Start Over:** Clears your current drawn path without changing the puzzle.
  - **New Game:** Generates a new puzzle with different dimensions and layout.

## Credits

Created by James FALLOUH.

## License

This project is licensed under the MIT License.
