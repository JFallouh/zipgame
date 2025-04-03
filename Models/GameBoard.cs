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

        // Walls: vertical walls between columns (dimensions: [Rows, Cols+1])
        public bool[,] VerticalWalls { get; set; } = default!;
        // Walls: horizontal walls between rows (dimensions: [Rows+1, Cols])
        public bool[,] HorizontalWalls { get; set; } = default!;

        private static Random rand = new Random();

        public static GameBoard CreateDefaultBoard()
        {
            // To balance performance and complexity, choose a board size randomly between 5 and 7.
            int size = rand.Next(5, 8); // 5,6,or 7.
            int rows = size, cols = size;
            int total = rows * cols;

            // Generate a random Hamiltonian path using DFS.
            var path = GenerateHamiltonianPath(rows, cols);
            if (path == null || path.Count != total)
            {
                // Fallback: use snake-ordered path.
                path = GenerateSnakePath(rows, cols);
            }

            // Create cells array.
            int[,] cells = new int[rows, cols];

            // Choose numbered cells count equal to board size.
            int numberedCount = size;
            var numberedPositions = SelectNumberedCells(path, numberedCount);
            if (numberedPositions == null)
            {
                // Fallback: choose evenly spaced indices.
                int[] indices = new int[numberedCount];
                for (int i = 0; i < numberedCount; i++)
                {
                    indices[i] = i * total / numberedCount;
                }
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

            // Determine wall probability based on board size.
            double wallProb = size switch
            {
                5 => 0.3,
                6 => 0.25,
                _ => 0.2,
            };

            // Generate walls.
            // VerticalWalls: dimensions [rows, cols+1]
            bool[,] verticalWalls = new bool[rows, cols + 1];
            for (int r = 0; r < rows; r++)
            {
                verticalWalls[r, 0] = true;
                verticalWalls[r, cols] = true;
                for (int c = 1; c < cols; c++)
                {
                    verticalWalls[r, c] = (rand.NextDouble() < wallProb);
                }
            }
            // HorizontalWalls: dimensions [rows+1, cols]
            bool[,] horizontalWalls = new bool[rows + 1, cols];
            for (int c = 0; c < cols; c++)
            {
                horizontalWalls[0, c] = true;
                horizontalWalls[rows, c] = true;
                for (int r = 1; r < rows; r++)
                {
                    horizontalWalls[r, c] = (rand.NextDouble() < wallProb);
                }
            }
            // For every consecutive pair in the Hamiltonian path, force open the wall between them.
            for (int i = 0; i < path.Count - 1; i++)
            {
                var p = path[i];
                var q = path[i + 1];
                if (p.row == q.row && Math.Abs(p.col - q.col) == 1)
                {
                    int r = p.row;
                    int c = Math.Min(p.col, q.col) + 1;
                    verticalWalls[r, c] = false;
                }
                else if (p.col == q.col && Math.Abs(p.row - q.row) == 1)
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

        // Select 'count' cells along the path that are not immediately adjacent (horizontally or vertically).
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
