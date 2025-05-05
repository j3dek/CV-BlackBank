let number = 1;
let intervalID = null;
let iks = 'x';
let balance = 1000;
let betamount = 0;
let gameState = "ready"; // Możliwe stany: "ready", "running", "crashed", "observing"
let gamecolor = "lightgreen"; // Możliwe kolory: darkred i lightgreen
let glowingcolor = "rgba(0, 255, 0, 0.8)";
const canvas = document.getElementById("myCanvas");
const ctx = canvas.getContext("2d");
let x = 0;
let speed = 1000;
let displayNumber = 1; // Zmienna przechowująca wartość wyświetlaną w liczbaDisplay

function updateBalanceDisplay() {
    const balanceDisplay = document.getElementById("balanceDisplay");
    balanceDisplay.textContent = `$${balance.toFixed(2)}`;
}

function StartFunc() {
    const przycisk = document.getElementById('przycisk-gray');

    if (betamount === 0) {
        alert("You cannot enter the crash if your bet amount is equal to 0");
        return;
    }

    // Obsługa kliknięcia w zależności od stanu gry
    if (gameState === "running") {
        handleCashout(przycisk);
        return;
    }

    if (gameState === "crashed") {
        resetGame(przycisk);
        return;
    }

    startNewGame(przycisk);
}

function handleCashout(przycisk) {
    const wygrana = betamount * number;
    balance += wygrana; // Dodaj wygraną do balansu
    updateBalanceDisplay(); // Zaktualizuj wyświetlany balans

    przycisk.classList.remove('przycisk-green');
    przycisk.classList.add('przycisk-gray');
    przycisk.textContent = `You cashed out: $${wygrana.toFixed(2)}`;

    gameState = "observing";
}

function resetGame(przycisk) {
    przycisk.classList.remove('przycisk-red');
    przycisk.classList.add('przycisk-gray');
    przycisk.textContent = "Start";
    gamecolor = "lightgreen";
    number = 1;
    gameState = "ready";
    updateCanvasGlow(); // Zaktualizuj glowienie canvasu
}

function startNewGame(przycisk) {
    if (balance < betamount) {
        alert("You do not have enough balance to place this bet.");
        return;
    }

    balance -= betamount; // Odejmij zakład od balansu
    updateBalanceDisplay(); // Zaktualizuj wyświetlany balans

    przycisk.classList.remove('przycisk-gray');
    przycisk.classList.add('przycisk-green');
    przycisk.textContent = `Cashout ${(number * betamount).toFixed(2)}`;
    gamecolor = "lightgreen";
    number = 1;
    x = 1;
    speed = 2.5;

    gameState = "running";
    updateCanvasGlow(); // Zaktualizuj glowienie canvasu
    animate();
    updateNumber();
}

function updateNumber() {
    const random_num = Math.floor(Math.random() * 1000 + 1);

    // Crashing
    if (random_num > 995) {
        clearTimeout(intervalID);
        intervalID = null;

        const liczbaDisplay = document.getElementById("liczbaDisplay");
        liczbaDisplay.style.color = "red";
        liczbaDisplay.textContent = iks + number;

        if (gameState === "observing") {
            gameState = "crashed";
            updateCanvasGlow(); // Zaktualizuj glowienie canvasu
            return;
        }

        const przycisk = document.getElementById('przycisk-gray');
        przycisk.classList.remove('przycisk-green');
        przycisk.classList.add('przycisk-red');
        przycisk.textContent = "You lost";

        balance -= betamount;
        gameState = "crashed";
        updateCanvasGlow(); // Zaktualizuj glowienie canvasu
        resetSekNumbers();
        return;
    }

    number = parseFloat((number + 0.01).toFixed(2));
    updateDisplay();

    if (gameState === "running" || gameState === "observing") {
        intervalID = setTimeout(updateNumber, MsLowering(number));
    }
}

function updateDisplay() {
    const liczbaDisplay = document.getElementById("liczbaDisplay");
    liczbaDisplay.style.color = "green";
    liczbaDisplay.textContent = iks + number;

    if (gameState === "running") {
        const przycisk = document.getElementById('przycisk-gray');
        const potentialCashout = (betamount * number).toFixed(2);
        przycisk.textContent = `Cashout $${potentialCashout}`;
    }
    SekNumbers();
    MulitplyNumbers();
}

function MsLowering(number) {
    return number > 2 ? Math.max(200 / number, 10) : 100;
}

function dodaj(amount) {
    if (gameState === "ready" || gameState === "crashed") {
        const input = document.getElementById('typeNumber');
        const currentValue = parseFloat(input.value) || 0;
        input.value = (currentValue + amount).toFixed(2);
        betamount = parseFloat(input.value);
    }
}

function usun(amount) {
    if (gameState === "ready" || gameState === "crashed") {
        const input = document.getElementById('typeNumber');
        const currentValue = parseFloat(input.value) || 0;
        input.value = (currentValue * amount).toFixed(2);
        betamount = parseFloat(input.value);
    }
}

function animate() {
    if (gameState === "running" || gameState === "observing") {
        drawFrame();

        speed = Math.max(speed * 0.9985, 0.001);
        x += speed;

        if (x <= canvas.width) {
            requestAnimationFrame(animate);
        }
    } else if (gameState === "crashed") {
        gamecolor = "darkred";
        glowingcolor = "rgba(139, 0, 0, 1)";
        drawFrame(); // Rysowanie ostatniej klatki
    }
    glowingcolor = "rgba(0, 255, 0, 0.8)";
}

function drawFrame() {
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    ctx.beginPath();
    ctx.moveTo(0, canvas.height);
    ctx.lineJoin = "round";
    ctx.lineCap = "round";
    ctx.shadowColor = glowingcolor;
    ctx.shadowBlur = Pulsacja();
    let krok = 0.25;
    let scaleY = 1;
    let prevX = 0,
        prevY = canvas.height;
    let curX = 0,
        curY = canvas.height;

    for (let i = 0; i <= x; i += krok) {
        let yVal = (i * i) / 3000;
        let cx = i;
        let cy = canvas.height - yVal * scaleY;
        if (cy < 0) break;

        ctx.lineTo(cx, cy);

        prevX = curX;
        prevY = curY;
        curX = cx;
        curY = cy;
    }

    ctx.lineWidth = 30;
    ctx.strokeStyle = gamecolor;
    ctx.stroke();

    const dx = curX - prevX;
    const dy = curY - prevY;
    const angle = Math.atan2(dy, dx);

    drawTriangle(curX, curY, angle);
}

function drawTriangle(x, y, angle) {
    const s = 150;

    ctx.save();
    ctx.translate(x, y);
    ctx.rotate(angle);

    ctx.beginPath();
    ctx.moveTo(s / 2, 0);
    ctx.lineTo(-s / 2, -s / 4);
    ctx.lineTo(-s / 2, s / 4);
    ctx.closePath();

    ctx.fillStyle = gamecolor;
    ctx.fill();

    ctx.restore();
}

let pulseDirection = 1; // Kierunek pulsowania (1 = zwiększanie, -1 = zmniejszanie)
let shadowBlurValue = 20; // Początkowa wartość rozmycia
function Pulsacja() {
    shadowBlurValue += pulseDirection * 0.2;

    if (shadowBlurValue >= 40 || shadowBlurValue <= 10) {
        pulseDirection *= -1;
    }

    return shadowBlurValue;
}

function MulitplyNumbers() {
    const multinum1 = document.getElementById("multinum1");
    const multinum2 = document.getElementById("multinum2");
    const multinum3 = document.getElementById("multinum3");
    const multinum4 = document.getElementById("multinum4");
    const multinum5 = document.getElementById("multinum5");
    const liczbaDisplay = document.getElementById("liczbaDisplay");

    const displayValue = parseFloat(liczbaDisplay.textContent.replace(iks, ''));

    multinum1.textContent = Math.max(displayValue, 2).toFixed(1);
    multinum2.textContent = Math.max(displayValue - 0.2, 1.8).toFixed(1);
    multinum3.textContent = Math.max(displayValue - 0.4, 1.6).toFixed(1);
    multinum4.textContent = Math.max(displayValue - 0.6, 1.4).toFixed(1);
    multinum5.textContent = Math.max(displayValue - 0.8, 1.2).toFixed(1);
}

let sekNumbersStarted = false;

let seknumIntervals = [];

function SekNumbers() {
    if (sekNumbersStarted) return;

    const liczbaDisplay = document.getElementById("liczbaDisplay");
    const liczbaValue = parseFloat(liczbaDisplay.textContent.replace('x', ''));

    if (liczbaValue > 2) {
        sekNumbersStarted = true;

        const seknums = [
            { element: document.getElementById("seknum1"), value: 1, increment: 1, interval: 3500 },
            { element: document.getElementById("seknum2"), value: 2, increment: 1, interval: 3000 },
            { element: document.getElementById("seknum3"), value: 3, increment: 1, interval: 2500 },
            { element: document.getElementById("seknum4"), value: 5, increment: 1, interval: 2000 },
            { element: document.getElementById("seknum5"), value: 8, increment: 1, interval: 1500 },
            { element: document.getElementById("seknum6"), value: 14, increment: 1, interval: 1000 },
        ];

        seknums.forEach(obj => {
            obj.element.textContent = obj.value + 's';
            const intervalID = setInterval(() => {
                obj.value += obj.increment;
                obj.element.textContent = obj.value + 's';
            }, obj.interval);

            seknumIntervals.push(intervalID);
        });
    }
}

function resetSekNumbers() {
    const seknums = [
        { element: document.getElementById("seknum1"), initialValue: 1 },
        { element: document.getElementById("seknum2"), initialValue: 2 },
        { element: document.getElementById("seknum3"), initialValue: 3 },
        { element: document.getElementById("seknum4"), initialValue: 5 },
        { element: document.getElementById("seknum5"), initialValue: 8 },
        { element: document.getElementById("seknum6"), initialValue: 14 },
    ];

    seknumIntervals.forEach(intervalID => clearInterval(intervalID));
    seknumIntervals = [];

    seknums.forEach(obj => {
        obj.element.textContent = obj.initialValue + 's';
    });

    sekNumbersStarted = false;
}

function updateCanvasGlow() {
    const canvas = document.getElementById("mysecondCanvas");

    if (gameState === "running") {
        canvas.classList.remove("red-glow");
        canvas.classList.add("green-glow");
    } else if (gameState === "crashed") {
        canvas.classList.remove("green-glow");
        canvas.classList.add("red-glow");
    } else {
        canvas.classList.remove("green-glow", "red-glow");
    }
}

