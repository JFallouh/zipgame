document.addEventListener('DOMContentLoaded', function () {
    const canvas = document.getElementById('gameCanvas');
    const ctx = canvas.getContext('2d');
    const rows = boardConfig.rows;
    const cols = boardConfig.cols;
    const cellWidth = canvas.width / cols;
    const cellHeight = canvas.height / rows;
    let drawing = false;
    let path = []; // Stores sequence of {row, col} objects

    // Draw the grid and the numbered cells.
    function drawBoard() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        // Draw grid lines.
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

        // Draw numbered cells.
        for (let i = 0; i < rows; i++) {
            for (let j = 0; j < cols; j++) {
                const num = boardConfig.cells[i][j];
                if (num > 0) {
                    ctx.fillStyle = '#ddd';
                    ctx.fillRect(j * cellWidth, i * cellHeight, cellWidth, cellHeight);
                    ctx.fillStyle = '#000';
                    ctx.font = '20px Arial';
                    ctx.textAlign = 'center';
                    ctx.textBaseline = 'middle';
                    ctx.fillText(num, j * cellWidth + cellWidth / 2, i * cellHeight + cellHeight / 2);
                }
            }
        }
    }

    // Draw the user's path.
    function drawPath() {
        if (path.length === 0) return;
        ctx.beginPath();
        ctx.lineWidth = 3;
        ctx.strokeStyle = 'red';
        const start = cellCenter(path[0]);
        ctx.moveTo(start.x, start.y);
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

    // Convert event coordinates into board cell indices.
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

    // Event handlers for drawing the path.
    function startDrawing(e) {
        e.preventDefault();
        drawing = true;
        path = [];
        const cell = getCellFromEvent(e);
        path.push(cell);
        redraw();
    }

    function continueDrawing(e) {
        if (!drawing) return;
        e.preventDefault();
        const cell = getCellFromEvent(e);
        const last = path[path.length - 1];
        if (cell.row !== last.row || cell.col !== last.col) {
            path.push(cell);
            redraw();
        }
    }

    function endDrawing(e) {
        if (!drawing) return;
        e.preventDefault();
        drawing = false;
        redraw();
    }

    function redraw() {
        drawBoard();
        drawPath();
    }

    // Attach mouse and touch event listeners.
    canvas.addEventListener('mousedown', startDrawing);
    canvas.addEventListener('mousemove', continueDrawing);
    canvas.addEventListener('mouseup', endDrawing);
    canvas.addEventListener('mouseleave', endDrawing);

    canvas.addEventListener('touchstart', startDrawing);
    canvas.addEventListener('touchmove', continueDrawing);
    canvas.addEventListener('touchend', endDrawing);
    canvas.addEventListener('touchcancel', endDrawing);

    // Hint button: fetch a static hint path.
    document.getElementById('hintButton').addEventListener('click', function () {
        fetch('/Game/Hint')
            .then(response => response.json())
            .then(data => {
                alert('Hint path: ' + JSON.stringify(data));
            });
    });

    // Check Solution button: send both the drawn path and the board configuration.
    document.getElementById('checkButton').addEventListener('click', function () {
        fetch('/Game/CheckSolution', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                userPath: path.map(cell => [cell.row, cell.col]),
                boardCells: boardConfig.cells
            })
        })
            .then(response => response.json())
            .then(data => {
                const resultDiv = document.getElementById('result');
                resultDiv.innerText = data.valid ? 'Solution is valid!' : 'Solution is invalid. Try again.';
            });
    });

    // Initial drawing.
    redraw();
});
