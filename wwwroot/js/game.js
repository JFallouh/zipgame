document.addEventListener('DOMContentLoaded', function () {
    const canvas = document.getElementById('gameCanvas');
    const ctx = canvas.getContext('2d');

    const rows = boardConfig.rows;
    const cols = boardConfig.cols;
    const cellWidth = canvas.width / cols;
    const cellHeight = canvas.height / rows;
    const totalCells = rows * cols;

    let drawing = false;
    let path = []; // Array of { row, col } for the drawn path.
    let gameSolved = false;

    // Start timer.
    let startTime = Date.now();
    let timerInterval = setInterval(updateTimer, 1000);

    function updateTimer() {
        if (gameSolved) return;
        const elapsedSeconds = Math.floor((Date.now() - startTime) / 1000);
        document.getElementById('timer').innerText = "Time: " + elapsedSeconds + "s";
    }

    // --- VALIDATION: Accept any Hamiltonian path that meets the conditions ---
    // Check that:
    // 1. The drawn path covers every cell exactly once.
    // 2. For every cell that has a number (in boardConfig.cells), its order
    //    in the drawn path is in ascending order.
    function checkSolution() {
        if (path.length !== totalCells) return false;
        // Check uniqueness.
        const seen = new Set();
        for (const cell of path) {
            seen.add(cell.row + "_" + cell.col);
        }
        if (seen.size !== totalCells) return false;

        // Gather the numbered cells from boardConfig.
        let numbered = [];
        for (let r = 0; r < rows; r++) {
            for (let c = 0; c < cols; c++) {
                const num = boardConfig.cells[r][c];
                if (num > 0) {
                    const idx = path.findIndex(cell => cell.row === r && cell.col === c);
                    if (idx === -1) return false;
                    numbered.push({ num: num, index: idx });
                }
            }
        }
        // Sort by the number value.
        numbered.sort((a, b) => a.num - b.num);
        // Check that indices are strictly increasing.
        for (let i = 0; i < numbered.length - 1; i++) {
            if (numbered[i].index >= numbered[i + 1].index) {
                return false;
            }
        }
        return true;
    }
    // --- END VALIDATION ---

    // Draw grid and numbered cells.
    function drawBoard() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        // Draw grid lines (thin).
        ctx.strokeStyle = 'black';
        ctx.lineWidth = 1;
        for (let i = 0; i <= rows; i++) {
            ctx.beginPath();
            ctx.moveTo(0, i * cellHeight);
            ctx.lineTo(canvas.width, i * cellHeight);
            ctx.stroke();
        }
        for (let j = 0; j <= cols; j++) {
            ctx.beginPath();
            ctx.moveTo(j * cellWidth, 0);
            ctx.lineTo(j * cellWidth, canvas.height);
            ctx.stroke();
        }

        // Draw numbered cells (orange background with black text).
        for (let r = 0; r < rows; r++) {
            for (let c = 0; c < cols; c++) {
                const num = boardConfig.cells[r][c];
                if (num > 0) {
                    ctx.fillStyle = 'orange';
                    ctx.fillRect(c * cellWidth, r * cellHeight, cellWidth, cellHeight);

                    ctx.fillStyle = 'black';
                    ctx.font = '20px Arial';
                    ctx.textAlign = 'center';
                    ctx.textBaseline = 'middle';
                    ctx.fillText(num, c * cellWidth + cellWidth / 2, r * cellHeight + cellHeight / 2);
                }
            }
        }
    }

    // Draw the user's drawn path with a thick blue line.
    function drawPath() {
        if (path.length === 0) return;
        ctx.beginPath();
        ctx.lineWidth = 30; // Thick line.
        ctx.strokeStyle = 'blue';
        const startPos = cellCenter(path[0]);
        ctx.moveTo(startPos.x, startPos.y);
        for (let i = 1; i < path.length; i++) {
            const center = cellCenter(path[i]);
            ctx.lineTo(center.x, center.y);
        }
        ctx.stroke();
    }

    function cellCenter(cell) {
        return {
            x: cell.col * cellWidth + cellWidth / 2,
            y: cell.row * cellHeight + cellHeight / 2
        };
    }

    function sameCell(a, b) {
        return a.row === b.row && a.col === b.col;
    }

    function manhattanDistance(a, b) {
        return Math.abs(a.row - b.row) + Math.abs(a.col - b.col);
    }

    function redraw() {
        drawBoard();
        drawPath();
    }

    // Convert mouse/touch event coordinates to a grid cell.
    function getCellFromEvent(e) {
        const rect = canvas.getBoundingClientRect();
        let clientX, clientY;
        if (e.touches && e.touches.length > 0) {
            clientX = e.touches[0].clientX;
            clientY = e.touches[0].clientY;
        } else {
            clientX = e.clientX;
            clientY = e.clientY;
        }
        const x = clientX - rect.left;
        const y = clientY - rect.top;
        const col = Math.floor(x / cellWidth);
        const row = Math.floor(y / cellHeight);
        return { row, col };
    }

    // Event handlers for drawing.
    function startDrawing(e) {
        if (gameSolved) return;
        e.preventDefault();
        drawing = true;
        if (path.length === 0) {
            path.push(getCellFromEvent(e));
            redraw();
        }
    }

    function continueDrawing(e) {
        if (!drawing || gameSolved) return;
        e.preventDefault();
        const cell = getCellFromEvent(e);
        const last = path[path.length - 1];
        // Allow only moves to adjacent cells.
        if (manhattanDistance(cell, last) !== 1) return;
        if (sameCell(cell, last)) return;

        // Check if cell is already in path.
        const idx = path.findIndex(c => sameCell(c, cell));
        if (idx !== -1) {
            // Allow backtracking if cell is immediately before the last.
            if (idx === path.length - 2) {
                path.pop();
                redraw();
                return;
            } else {
                return;
            }
        }
        path.push(cell);
        redraw();

        // If all cells have been used, validate.
        if (path.length === totalCells && checkSolution()) {
            gameSolved = true;
            clearInterval(timerInterval);
            const timeTaken = Math.floor((Date.now() - startTime) / 1000);
            document.getElementById('result').innerText =
                "Great Job! You finished in " + timeTaken + " seconds.";
        }
    }

    function endDrawing(e) {
        if (!drawing || gameSolved) return;
        e.preventDefault();
        drawing = false;
    }

    // Attach mouse and touch events.
    canvas.addEventListener('mousedown', startDrawing);
    canvas.addEventListener('mousemove', continueDrawing);
    canvas.addEventListener('mouseup', endDrawing);
    canvas.addEventListener('mouseleave', endDrawing);
    canvas.addEventListener('touchstart', startDrawing);
    canvas.addEventListener('touchmove', continueDrawing);
    canvas.addEventListener('touchend', endDrawing);
    canvas.addEventListener('touchcancel', endDrawing);

    // "Start Over" button reloads the page.
    document.getElementById('startOverButton').addEventListener('click', function() {
        location.reload();
    });

    // Initial draw.
    redraw();
});
