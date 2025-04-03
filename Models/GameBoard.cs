using System;
using System.Collections.Generic;

namespace zipgame.Models
{
    public class GameBoard
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        // Cells: 0 means empty; a positive number indicates a numbered cell.
        public int[,] Cells { get; set; } = default!;

        // Walls are represented as two Boolean matrices.
        // VerticalWalls[r, c] is true if there's a wall between cell (r, c-1) and (r, c).
        // Dimensions: [Rows, Cols+1]
        public bool[,] VerticalWalls { get; set; } = default!;
        // HorizontalWalls[r, c] is true if there's a wall between cell (r-1, c) and (r, c).
        // Dimensions: [Rows+1, Cols]
        public bool[,] HorizontalWalls { get; set; } = default!;

        private static Random rand = new Random();

        public static GameBoard CreateDefaultBoard()
        {
            // Increase complexity: use an 8x8 grid.
            int rows = 8, cols = 8;
            int total = rows * cols;

            // Generate a random Hamiltonian path using DFS.
            var path = GenerateHamiltonianPath(rows, cols);
            // Fallback to a snake-ordered path if DFS fails.
            if (path == null || path.Count != total)
            {
                path = GenerateSnakePath(rows, cols);
            }

            // Create cells array.
            int[,] cells = new int[rows, cols];

            // Try to select 5 numbered cells along the path that are not immediately adjacent (horizontally or vertically).
            var numberedPositions = SelectNumberedCells(path, 5);
            // If selection fails, fallback to candidate indices.
            if (numberedPositions == null)
            {
                int[] indices = new int[] { 0, total / 5, (2 * total) / 5, (3 * total) / 5, total - 1 };
                numberedPositions = new List<(int row, int col)>();
                foreach (int idx in indices)
                    numberedPositions.Add(path[idx]);
            }
            // Mark the numbered cells.
            for (int i = 0; i < numberedPositions.Count; i++)
            {
                var pos = numberedPositions[i];
                cells[pos.row, pos.col] = i + 1;
            }

            // Generate walls.
            // VerticalWalls: dimensions [rows, cols+1]
            bool[,] verticalWalls = new bool[rows, cols + 1];
            // Set boundary walls.
            for (int r = 0; r < rows; r++)
            {
                verticalWalls[r, 0] = true;
                verticalWalls[r, cols] = true;
            }
            // HorizontalWalls: dimensions [rows+1, cols]
            bool[,] horizontalWalls = new bool[rows + 1, cols];
            for (int c = 0; c < cols; c++)
            {
                horizontalWalls[0, c] = true;
                horizontalWalls[rows, c] = true;
            }
            // Initialize internal walls randomly (30% chance), except for edges used in the Hamiltonian path.
            // First, mark all internal walls randomly.
            for (int r = 0; r < rows; r++)
            {
                for (int c = 1; c < cols; c++)
                {
                    verticalWalls[r, c] = (rand.NextDouble() < 0.3);
                }
            }
            for (int r = 1; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    horizontalWalls[r, c] = (rand.NextDouble() < 0.3);
                }
            }
            // Then, for every consecutive pair in the Hamiltonian path, ensure the wall is open.
            for (int i = 0; i < path.Count - 1; i++)
            {
                var p = path[i];
                var q = path[i + 1];
                // Horizontal move.
                if (p.row == q.row && Math.Abs(p.col - q.col) == 1)
                {
                    int r = p.row;
                    int c = Math.Min(p.col, q.col) + 1;
                    verticalWalls[r, c] = false;
                }
                // Vertical move.
                if (p.col == q.col && Math.Abs(p.row - q.row) == 1)
                {
                    int c = p.col;
                    int r = Math.Min(p.row, q.row) + 1;
                    horizontalWalls[r, c] = false;
                }
            }

            return new GameBoard
            {
                Rows = rows,
                Cols = cols,
                Cells = cells,
                VerticalWalls = verticalWalls,
                HorizontalWalls = horizontalWalls
            };
        }

        // Try to greedily select 'count' cells along the path that are not immediately adjacent horizontally or vertically.
        private static List<(int row, int col)>? SelectNumberedCells(List<(int row, int col)> path, int count)
        {
            var selected = new List<(int row, int col)>();
            foreach (var pos in path)
            {
                bool valid = true;
                foreach (var s in selected)
                {
                    if ((s.row == pos.row && Math.Abs(s.col - pos.col) == 1) ||
                        (s.col == pos.col && Math.Abs(s.row - pos.row) == 1))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    selected.Add(pos);
                    if (selected.Count == count)
                        return selected;
                }
            }
            return null;
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
