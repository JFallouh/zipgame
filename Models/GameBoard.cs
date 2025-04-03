using System;
using System.Collections.Generic;

namespace zipgame.Models
{
    public class GameBoard
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        // 0 means empty; a positive number indicates a required numbered cell.
        public int[,] Cells { get; set; } = default!;

        private static Random rand = new Random();

        public static GameBoard CreateDefaultBoard()
        {
            int rows = 5, cols = 5;
            int total = rows * cols;
            // Generate a random Hamiltonian path using DFS.
            var path = GenerateHamiltonianPath(rows, cols);
            // Fallback to a snake path if DFS fails (should not happen for 5x5).
            if (path == null || path.Count != total)
            {
                path = GenerateSnakePath(rows, cols);
            }

            // Create an empty board.
            int[,] cells = new int[rows, cols];

            // Choose 5 indices (approximately equally spaced) along the path for numbering.
            int[] indices = new int[] { 0, total / 5, (2 * total) / 5, (3 * total) / 5, total - 1 };
            for (int i = 0; i < indices.Length; i++)
            {
                var pos = path[indices[i]];
                cells[pos.row, pos.col] = i + 1;
            }

            return new GameBoard { Rows = rows, Cols = cols, Cells = cells };
        }

        // Generate a Hamiltonian path using DFS.
        private static List<(int row, int col)>? GenerateHamiltonianPath(int rows, int cols)
        {
            var path = new List<(int row, int col)>();
            var visited = new bool[rows, cols];
            int startRow = rand.Next(rows);
            int startCol = rand.Next(cols);
            if (DFS(startRow, startCol, rows, cols, path, visited))
                return path;
            return null;
        }

        private static bool DFS(int row, int col, int rows, int cols, List<(int row, int col)> path, bool[,] visited)
        {
            path.Add((row, col));
            visited[row, col] = true;
            if (path.Count == rows * cols)
                return true;
            // Get neighbors in random order.
            var directions = new List<(int dr, int dc)> { (1, 0), (-1, 0), (0, 1), (0, -1) };
            Shuffle(directions);
            foreach (var (dr, dc) in directions)
            {
                int newRow = row + dr, newCol = col + dc;
                if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols && !visited[newRow, newCol])
                {
                    if (DFS(newRow, newCol, rows, cols, path, visited))
                        return true;
                }
            }
            // Backtrack.
            path.RemoveAt(path.Count - 1);
            visited[row, col] = false;
            return false;
        }

        // Fisher–Yates shuffle.
        private static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        // Fallback: generate a snake-ordered path.
        private static List<(int row, int col)> GenerateSnakePath(int rows, int cols)
        {
            var path = new List<(int row, int col)>();
            for (int r = 0; r < rows; r++)
            {
                if (r % 2 == 0)
                {
                    for (int c = 0; c < cols; c++)
                        path.Add((r, c));
                }
                else
                {
                    for (int c = cols - 1; c >= 0; c--)
                        path.Add((r, c));
                }
            }
            return path;
        }
    }
}
