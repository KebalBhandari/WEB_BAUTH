$(document).ready(function () {
    // Sentences ranging from easy to hard
    const sentences = [
        "The quick brown fox jumps over the lazy dog.",
        "Pack my box with five dozen liquor jugs.",
        "How razorback-jumping frogs can level six piqued gymnasts!",
        "Sympathizing would fix Quaker objectives.",
        "Crazy Frederick bought many very exquisite opal jewels.",
        "Jaded zombies acted quaintly but kept driving their oxen forward.",
        "Amazingly few discotheques provide jukeboxes.",
        "Grumpy wizards make toxic brew for the evil queen and jack.",
        "The five boxing wizards jump quickly.",
        "Big Fuji waves pitch enzymed kex liquor jugs.",
        "Lazy dogs often sleep in the sunniest spot in the yard.",
        "Twelve jovial men quickly pick five ripe mangoes.",
        "Bright yellow flowers bloom during springtime.",
        "Jack quietly observed the mesmerizing sunset from the hilltop.",
        "Few daring explorers venture into the uncharted forest.",
        "Ducks gracefully glide across the shimmering pond.",
        "A brisk wind swept across the barren desert landscape.",
        "Seven curious cats climbed up the ancient oak tree.",
        "Heavy clouds gathered ominously over the distant mountains.",
        "Gleaming stars twinkled brightly in the midnight sky.",
        "Smart students solve tricky problems with logical thinking.",
        "Vast green fields stretch endlessly beyond the horizon.",
        "Colorful birds sing melodious tunes at dawn.",
        "Ten swift deer raced across the open meadow.",
        "Ancient ruins reveal secrets of a forgotten civilization.",
        "The bustling market overflowed with vibrant goods and lively chatter.",
        "Early morning hikers reached the summit before sunrise.",
        "Crystal-clear water cascades down the rocky cliff.",
        "Golden sands glisten under the warm tropical sun.",
        "Mysterious shadows danced along the cobblestone streets.",
        "Adventurous kids built an elaborate treehouse in the backyard.",
        "The chef skillfully prepared a delicious five-course meal.",
        "A sudden thunderstorm caught everyone by surprise.",
        "Enthusiastic fans cheered loudly for their favorite team.",
        "Lush gardens display a dazzling array of exotic flowers.",
        "Careful planning ensures a smooth and successful event.",
        "Fierce winds howled through the narrow mountain pass.",
        "The library offers a quiet haven for avid readers.",
        "Tall skyscrapers define the city’s iconic skyline.",
        "Playful dolphins leaped gracefully out of the water.",
        "Whimsical paintings adorn the walls of the cozy cafe.",
        "The scent of fresh bread wafted through the bustling bakery.",
        "Bright lanterns illuminated the festive village square.",
        "Determined athletes train tirelessly for the upcoming marathon.",
        "Fluffy snowflakes gently blanketed the sleepy town.",
        "Inventive scientists work on groundbreaking new technologies.",
        "The majestic eagle soared high above the rugged cliffs.",
        "Serene lakes reflect the beauty of the surrounding forest.",
        "Friendly neighbors gathered for an evening barbecue.",
        "The musician played a haunting melody on the grand piano."
    ];

    const BehavioralAuth = {
        // Constants
        TOTAL_DOTS: 5,
        TOTAL_SHAPES: 5,
        TOTAL_ATTEMPTS: 3,

        // State
        currentAttempt: 1,
        totalShapeDotsCount: 0,
        timings: [],
        keyHoldTimes: [],
        backspaceTimings: [],
        dotTimings: [],
        shapeTimings: [],
        shapeMouseMovements: [],
        promptTexts: [],
        userInputs: [],
        detectedLanguages: [],
        lastKeyTime: 0,
        lastMoveTime: 0,

        // Initialization
        init() {
            this.resetState();
            $('#testSection').show();
            $('#resultSection').hide();
            this.startAttempt();
        },

        resetState() {
            this.currentAttempt = 1;
            this.totalShapeDotsCount = 0;
            this.timings = [];
            this.keyHoldTimes = [];
            this.backspaceTimings = [];
            this.dotTimings = [];
            this.shapeTimings = [];
            this.shapeMouseMovements = [];
            this.userInputs = [];
            this.detectedLanguages = [];
            this.lastKeyTime = 0;
            this.lastMoveTime = 0;
            $('#inputText').val('');
            $('#matchingPercent').text('0');
            $('#dotArea').empty().append('<div id="dotCount" class="dot-count">Dots Clicked: 0/5</div>');
            $('#shapesContainer').empty();
            $('#dotArea').show();
            $('#shapeArea').show();
            $('#nextAttempt').removeClass('btn-animated');
            this.updateUI();
            this.promptTexts = Array.from({ length: this.TOTAL_ATTEMPTS }, () =>
                sentences[Math.floor(Math.random() * sentences.length)]
            );
        },

        updateUI() {
            $('#currentAttempt').text(this.currentAttempt);
            $('#taskInstruction').text(
                'Step 1: Click all 5 dots. Step 2: Select 5 correct shapes. Step 3: Type the text exactly as shown.'
            );
            $('#nextAttempt').text(this.currentAttempt === this.TOTAL_ATTEMPTS ? 'SUBMIT' : 'Next Attempt');
            this.updateProgressBar(0);
            $('#promptText').text(this.promptTexts[this.currentAttempt - 1]);
        },

        // Progress Bar
        updateProgressBar(percentage) {
            $('#taskProgressBar').css('width', `${percentage}%`).text(`${percentage}%`);
        },

        // Dot Sequence
        startDotSequence() {
            let dotCount = 0;
            const area = $('#dotArea')[0];
            const timingsArray = [];

            const showDot = () => {
                if (dotCount >= this.TOTAL_DOTS) {
                    $('#dotCount').text('All dots clicked successfully!');
                    return;
                }
                const dot = $('<div class="dot"></div>');
                const x = Math.random() * (area.clientWidth - 50);
                const y = Math.random() * (area.clientHeight - 50);
                dot.css({ left: `${x}px`, top: `${y}px` });
                const dotAppearTime = performance.now();

                dot.on('click touchstart', (event) => {
                    event.preventDefault();
                    const clickTime = performance.now();
                    timingsArray.push(clickTime - dotAppearTime);
                    dot.remove();
                    dotCount++;
                    this.totalShapeDotsCount++;
                    $('#dotCount').text(`Dots Clicked: ${dotCount}/${this.TOTAL_DOTS}`);
                    this.updateProgress();
                    showDot();
                });

                $('#dotArea').append(dot);
            };

            showDot();
            this.dotTimings[this.currentAttempt - 1] = timingsArray;
        },

        // Shape Selection
        startShapeSelection() {
            $('#shapeArea').empty().append(
                '<p id="shapeQuestion" class="mb-2"></p><div id="shapesContainer"></div><div id="shapeCount" class="dot-count">Shapes Selected: 0/5</div>'
            );
            const shapes = ['circle', 'square', 'triangle'];
            let shapeCount = 0;
            const timingsArray = [];
            const mouseMovements = [];
            let lastX = null, lastY = null, lastTime = null;

            // Mouse movement tracking
            $('#shapeArea').off('mousemove').on('mousemove', (event) => {
                const currentTime = performance.now();
                if (currentTime - this.lastMoveTime > 50) { // Sample every 50ms
                    const x = event.pageX - $('#shapeArea').offset().left;
                    const y = event.pageY - $('#shapeArea').offset().top;

                    // Calculate velocity
                    let velocity = 0;
                    if (lastX !== null && lastY !== null && lastTime !== null) {
                        const distance = Math.sqrt((x - lastX) ** 2 + (y - lastY) ** 2);
                        const timeDelta = currentTime - lastTime;
                        velocity = distance / timeDelta; 
                    }

                    let slope = 0;
                    if (lastX !== null && lastY !== null) {
                        const deltaX = x - lastX;
                        const deltaY = y - lastY;
                        if (deltaX !== 0) {
                            slope = deltaY / deltaX; 
                        } else {
                            slope = 0; 
                        }
                    }

                    mouseMovements.push({
                        time: currentTime,
                        x,
                        y,
                        velocity,
                        slope
                    });

                    lastX = x;
                    lastY = y;
                    lastTime = currentTime;
                    this.lastMoveTime = currentTime;
                }
            });

            const showShapes = () => {
                if (shapeCount >= this.TOTAL_SHAPES) {
                    $('#shapesContainer').empty();
                    $('#shapeQuestion').text('All shapes selected.');
                    $('#shapeArea').off('mousemove');
                    this.shapeTimings[this.currentAttempt - 1] = timingsArray;
                    this.shapeMouseMovements[this.currentAttempt - 1] = mouseMovements;
                    return;
                }
                $('#shapesContainer').empty();
                const targetShape = shapes[Math.floor(Math.random() * shapes.length)];
                $('#shapeQuestion').text(`Please select the ${targetShape}.`);

                const shapeAppearTime = performance.now();
                shapes.sort(() => Math.random() - 0.5).forEach(shapeType => {
                    const shape = $('<div></div>').addClass(`shape ${shapeType}`).css({
                        left: `${Math.random() * ($('#shapeArea')[0].clientWidth - 60)}px`,
                        top: `${Math.random() * ($('#shapeArea')[0].clientHeight - 60)}px`, 
                        position: 'absolute'
                    });

                    shape.on('click touchstart', (event) => {
                        event.preventDefault();
                        const clickTime = performance.now();
                        const reactionTime = clickTime - shapeAppearTime;
                        if (shapeType === targetShape) {
                            timingsArray.push({ reactionTime, isCorrect: 1 });
                            shapeCount++;
                            this.totalShapeDotsCount++;
                            $('#shapeCount').text(`Shapes Selected: ${shapeCount}/${this.TOTAL_SHAPES}`);
                            this.updateProgress();
                            showShapes();
                        } else {
                            alert('Wrong shape clicked. Please select the correct shape.');
                        }
                    });

                    $('#shapesContainer').append(shape);
                });
            };

            showShapes();
        },

        // Progress Calculation
        updateProgress() {
            const userInput = $('#inputText').val().trim();
            const typingProgress = (userInput.length / this.promptTexts[this.currentAttempt - 1].length) * this.TOTAL_DOTS;
            const progress = Math.floor(
                (this.totalShapeDotsCount + typingProgress) / (this.TOTAL_DOTS + this.TOTAL_SHAPES + this.TOTAL_DOTS) * 100
            );
            this.updateProgressBar(progress);
        },

        // Typing Events
        setupTypingEvents() {
            $('#inputText').off('keydown keyup').on('keydown', (event) => {
                const currentTime = Date.now();
                if (this.lastKeyTime !== null) {
                    this.timings[this.currentAttempt - 1] = this.timings[this.currentAttempt - 1] || [];
                    this.timings[this.currentAttempt - 1].push(currentTime - this.lastKeyTime);
                }
                this.lastKeyTime = currentTime;

                this.keyHoldTimes[this.currentAttempt - 1] = this.keyHoldTimes[this.currentAttempt - 1] || [];
                this.keyHoldTimes[this.currentAttempt - 1].push({ keydownTime: currentTime, keyupTime: null, duration: null });

                if (event.key === 'Backspace') {
                    this.backspaceTimings[this.currentAttempt - 1] = this.backspaceTimings[this.currentAttempt - 1] || [];
                    this.backspaceTimings[this.currentAttempt - 1].push({ time: currentTime, action: 'pressed' });
                }
            }).on('keyup', (event) => {
                const currentTime = Date.now();
                const keyDataArray = this.keyHoldTimes[this.currentAttempt - 1];
                for (let i = keyDataArray.length - 1; i >= 0; i--) {
                    if (keyDataArray[i].keyupTime === null) {
                        keyDataArray[i].keyupTime = currentTime;
                        keyDataArray[i].duration = currentTime - keyDataArray[i].keydownTime;
                        break;
                    }
                }
                if (event.key === 'Backspace') {
                    this.backspaceTimings[this.currentAttempt - 1] = this.backspaceTimings[this.currentAttempt - 1] || [];
                    this.backspaceTimings[this.currentAttempt - 1].push({ time: currentTime, action: 'released' });
                }

                this.updateProgress();
            }).on('copy paste cut contextmenu', (e) => {
                e.preventDefault();
                alert('Copying, pasting, and right-clicking are disabled. Please type manually.');
            });
        },

        // Attempt Handling
        startAttempt() {
            this.updateUI();
            this.startDotSequence();
            this.startShapeSelection();
            this.setupTypingEvents();
        },

        async nextAttempt() {
            const userInput = $('#inputText').val().trim();
            if (!this.validateInput(userInput)) return;

            this.userInputs.push(userInput);
            if (this.currentAttempt < this.TOTAL_ATTEMPTS) {
                this.currentAttempt++;
                this.totalShapeDotsCount = 0;
                this.lastKeyTime = null;
                $('#inputText').val('');
                this.startAttempt();
            } else {
                $('#nextAttempt').addClass('btn-animated');
                $('#testSection').hide();
                await this.saveDataToServer();
            }
        },

        validateInput(userInput) {
            if (!userInput) {
                alert('Please type the text before proceeding.');
                return false;
            }
            const similarity = this.calculateSimilarity(userInput, this.promptTexts[this.currentAttempt - 1]);
            if (similarity < 90) {
                alert(`Text is only ${similarity.toFixed(2)}% similar. Please try again.`);
                return false;
            }
            if (!this.timings[this.currentAttempt - 1]?.length || !this.keyHoldTimes[this.currentAttempt - 1]?.length) {
                alert('Not enough typing data captured. Please try typing again.');
                return false;
            }
            if (this.dotTimings[this.currentAttempt - 1]?.length < this.TOTAL_DOTS) {
                alert(`You need to click all ${this.TOTAL_DOTS} dots before proceeding.`);
                return false;
            }
            if (this.shapeTimings[this.currentAttempt - 1]?.length < this.TOTAL_SHAPES) {
                alert(`You need to select the shape ${this.TOTAL_SHAPES} times before proceeding.`);
                return false;
            }
            const totalTime = this.timings[this.currentAttempt - 1].reduce((a, b) => a + b, 0);
            if (totalTime < 1000) {
                alert('Typing speed seems unnatural. Please try again.');
                return false;
            }
            return true;
        },

        calculateSimilarity(s1, s2) {
            const longer = s1.length > s2.length ? s1 : s2;
            const shorter = s1.length > s2.length ? s2 : s1;
            let matches = 0;
            for (let i = 0; i < shorter.length; i++) {
                if (longer[i] === shorter[i]) matches++;
            }
            return (matches / longer.length) * 100;
        },

        // Data Saving
        async saveDataToServer() {
            const dataToSend = {
                tokenNo: localStorage.getItem('userId') || this.generateUniqueId(),
                timings: this.removeInvalidAttempts(this.timings),
                keyHoldTimes: this.removeInvalidAttempts(this.keyHoldTimes),
                backSpaceTimings: this.removeInvalidAttempts(this.backspaceTimings),
                dotTimings: this.removeInvalidAttempts(this.dotTimings),
                shapeTimings: this.removeInvalidAttempts(this.shapeTimings),
                shapeMouseMovements: this.removeInvalidAttempts(this.shapeMouseMovements),
                detectedLanguages: this.detectedLanguages
            };

            if (!this.validateData(dataToSend)) {
                AlertTost("error", "Incomplete data. Please complete all attempts.");
                $('#resultSection').show();
                return;
            }

            console.log("Data being sent to server:", dataToSend);
            const response = await AjaxCall("/TrainModel/SaveUserData", dataToSend);
            this.handleServerResponse(response);
        },

        removeInvalidAttempts(data) {
            if (!Array.isArray(data)) return [];

    return data.filter(attempt => {
        if (!attempt || typeof attempt !== 'object') return false;
        for (let key in attempt) {
            if (attempt[key] === null || attempt[key] === undefined) {
                return false;
            }
        }
        return true;
    });
},

        validateData(data) {
            return (
                data.timings.length === this.TOTAL_ATTEMPTS &&
                data.keyHoldTimes.length === this.TOTAL_ATTEMPTS &&
                data.dotTimings.length === this.TOTAL_ATTEMPTS &&
                data.shapeTimings.length === this.TOTAL_ATTEMPTS
            );
        },

        handleServerResponse(response) {
            if (response) {
                try {
                    const result = JSON.parse(response);
                    if (result.status === "SUCCESS") {
                        AlertTost("success", result.message);
                        $('#matchingPercent').text(result.confidenceScore || "100");
                        $('#resultSection').show();
                    } else {
                        AlertTost("error", result.message);
                        $('#matchingPercent').text("Error");
                        $('#resultSection').show();
                    }
                } catch (e) {
                    console.error("Error parsing response:", e);
                    AlertTost("error", "Unexpected error. Please try again.");
                    $('#matchingPercent').text("Error");
                    $('#resultSection').show();
                }
            } else {
                AlertTost("error", "Try Again!!!");
            }
        },

        generateUniqueId() {
            return 'xxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
                const r = Math.random() * 16 | 0;
                return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
            });
        }
    };

    // Event Bindings
    $('#nextAttempt').click(async function () {
        const userInput = $('#inputText').val().trim();
        if (!userInput) {
            alert("Please type the text before submitting.");
            return;
        }

        // Detect language only when submitting
        try {
            const detectedLanguage = await detectLanguage(userInput);
            console.log("Detected Language:", detectedLanguage);

            // Save detected language in BehavioralAuth state
            BehavioralAuth.detectedLanguages[BehavioralAuth.currentAttempt - 1] = detectedLanguage;

            // Proceed with submission
            BehavioralAuth.nextAttempt();
        } catch (error) {
            console.error("Error detecting language:", error);
            alert("Language detection failed. Please try again.");
        }
    });

    $('#tryAgain').click(() => BehavioralAuth.init());

    // Global Error Handler
    window.onerror = (msg, url, lineNo, columnNo, error) => {
        console.error(`Error: ${msg}\nAt: ${url}:${lineNo}:${columnNo}\nStack: ${error.stack}`);
    };

    BehavioralAuth.init();

    async function detectLanguage(text) {
        const response = await fetch('https://ws.detectlanguage.com/0.2/detect', {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer a7c8eba8949c03ecb5ac91cf5d1c8497',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ q: text })
        });

        const data = await response.json();
        if (data.data && data.data.detections.length > 0) {
            return data.data.detections[0].language; // Returns detected language code (e.g., "en", "fr")
        } else {
            throw new Error("No language detected");
        }
    }
});