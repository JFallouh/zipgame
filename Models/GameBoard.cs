using System;
using System.Collections.Generic;

namespace zipgame.Models
{
    public class GameBoard
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        // 0 means empty; a positive number indicates a required cell to be visited in order.
        public int[,] Cells { get; set; } = default!;

        public static GameBoard CreateDefaultBoard()
        {
            var board = new GameBoard
            {
                Rows = 5,
                Cols = 5,
                Cells = new int[5, 5]
            };

            // Randomly place numbered cells 1 through 5.
            Random rand = new Random();
            int numbersToPlace = 5;
            var usedPositions = new HashSet<(int row, int col)>();

            for (int num = 1; num <= numbersToPlace; num++)
            {
                int row, col;
                do
                {
                    row = rand.Next(0, board.Rows);
                    col = rand.Next(0, board.Cols);
                }
                while (!usedPositions.Add((row, col))); // repeat until a new position is found
                board.Cells[row, col] = num;
            }
            return board;
        }

        // Validates that the user path covers all cells and that the numbered cells
        // (extracted from boardCells) appear in increasing order in the drawn path.
        public static bool ValidateSolution(int[][] boardCells, int[][] userPath)
        {
            int rows = boardCells.Length;
            int cols = boardCells[0].Length;
            if (userPath.Length != rows * cols)
                return false;

            var requiredOrder = new List<(int number, int row, int col)>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (boardCells[i][j] > 0)
                        requiredOrder.Add((boardCells[i][j], i, j));
                }
            }
            // Sort the required cells by the number.
            requiredOrder.Sort((a, b) => a.number.CompareTo(b.number));

            int requiredIndex = 0;
            foreach (var pos in userPath)
            {
                if (requiredIndex < requiredOrder.Count &&
                    pos[0] == requiredOrder[requiredIndex].row &&
                    pos[1] == requiredOrder[requiredIndex].col)
                {
                    requiredIndex++;
                }
            }
            return requiredIndex == requiredOrder.Count;
        }
    }
}
