﻿$(document).ready(function () {
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


    const totalDots = 5; // Number of dots to click
    const totalShapes = 5; // Number of shape selections
    const totalAttempts = 3;
    let currentAttempt = 1;
    let totalShapeDotsCount = 0;

    let timings = []; // Time between key presses for all attempts
    let keyHoldTimes = []; // Duration each key is held down for all attempts
    let dotTimings = []; // Array to hold dot timings for attempts 1 and 3
    let shapeTimings = []; // Array to hold shape selection timings for attempt 2
    let shapeMouseMovements = []; // Mouse movement data during shape selection
    let promptTexts = []; // Prompt texts for each attempt
    let userInputs = []; // User inputs for each attempt

    let lastKeyTime = null; // Last keydown time

    // Initialize the test
    function initializeTest() {
        // Reset
        timings = [];
        keyHoldTimes = [];
        dotTimings = [];
        shapeTimings = [];
        shapeMouseMovements = [];
        userInputs = [];
        promptTexts = [];
        lastKeyTime = null;
        currentAttempt = 1;
        totalShapeDotsCount = 0;

        // Clear UI
        $('#inputText').val('');
        $('#matchingPercent').text('0');
        $('#dotArea').empty().append('<div id="dotCount" class="dot-count">Dots Clicked: 0/5</div>');
        $('#shapesContainer').empty();
        $('#dotArea').show();
        $('#shapeArea').show();
        $('#nextAttempt').removeClass('btn-animated');
        $('#currentAttempt').text(currentAttempt);
        $('#taskInstruction').text(
            'Please complete the dots, shapes, and type the following text:'
        );
        updateProgressBar(0);

        // Pick random sentences for each attempt
        for (let i = 0; i < totalAttempts; i++) {
            promptTexts.push(sentences[Math.floor(Math.random() * sentences.length)]);
        }
        $('#promptText').text(promptTexts[0]);

        $('#testSection').show();
        $('#resultSection').hide();

        // CHANGED: Start both dot + shape sequences on attempt 1
        startDotSequence();
        startShapeSelection();  // CHANGED: show shape from the start
        $('#shapeArea').show(); // CHANGED: reveal shape area now
    }

    initializeTest(); // Call on page load

    // Function to update the progress bar
    function updateProgressBar(percentage) {
        $('#taskProgressBar').css('width', percentage + '%');
        $('#taskProgressBar').text(percentage + '%');
    }

    // Function to start dot sequence
    function startDotSequence() {
        let dotCount = 0;
        const area = document.getElementById('dotArea');
        const timingsArray = [];

        function showDot() {
            if (dotCount >= totalDots) {
                return;
            }

            const dot = document.createElement('div');
            dot.classList.add('dot');

            const x = Math.random() * (area.clientWidth - 50);
            const y = Math.random() * (area.clientHeight - 50);

            dot.style.left = x + 'px';
            dot.style.top = y + 'px';

            const dotAppearTime = performance.now();
            dot.addEventListener('click', function (event) {
                const clickTime = performance.now();
                const reactionTime = clickTime - dotAppearTime;
                timingsArray.push(reactionTime);

                dot.remove();
                dotCount++;
                totalShapeDotsCount++;
                $('#dotCount').text(`Dots Clicked: ${dotCount}/${totalDots}`);
                let progress = Math.floor((totalShapeDotsCount / (totalDots + totalShapes)) * 100);
                updateProgressBar(progress);
                showDot(); // Show next dot
            });

            area.appendChild(dot);
        }

        showDot(); // Start with the first dot

        // Store the timings array for the current attempt
        dotTimings[currentAttempt - 1] = timingsArray; 
    }

    // Function to start shape selection task
    function startShapeSelection() {
        $('#shapeArea').empty().append('<p id="shapeQuestion" class="mb-2"></p><div id="shapesContainer"></div><div id="shapeCount" class="dot-count">Shapes Selected: 0/5</div>');
        $('#taskInstruction').text('Please select the correct shapes and type the following text:');
        updateProgressBar(0);

        const shapes = ['circle', 'square', 'triangle'];
        let shapeCount = 0;
        const timingsArray = [];
        const mouseMovements = []; // Array to store mouse movement data
        const area = document.getElementById('shapeArea');

        // Add mousemove event listener
        $('#shapeArea').on('mousemove', function (event) {
            const currentTime = performance.now();
            const x = event.pageX - $('#shapeArea').offset().left;
            const y = event.pageY - $('#shapeArea').offset().top;
            mouseMovements.push({
                time: currentTime,
                x: x,
                y: y
            });
        });

        function showShapes() {
            if (shapeCount >= totalShapes) {
                // All shapes selected, clear shapes
                $('#shapesContainer').empty();
                $('#shapeQuestion').text('All shapes selected.');

                // Remove mousemove event listener
                $('#shapeArea').off('mousemove');

                // Store the mouseMovements data for this attempt
                shapeTimings[currentAttempt - 1] = timingsArray;       // CHANGED
                shapeMouseMovements[currentAttempt - 1] = mouseMovements; // CHANGED

                return;
            }
            $('#shapesContainer').empty();

            const targetShape = shapes[Math.floor(Math.random() * shapes.length)];
            $('#shapeQuestion').text(`Please select the ${targetShape}.`);

            const shapeAppearTime = performance.now();
            const shuffledShapes = shapes.sort(() => Math.random() - 0.5);
            shuffledShapes.forEach(shapeType => {
                const shape = document.createElement('div');
                shape.classList.add('shape', shapeType);

                const x = Math.random() * (area.clientWidth - 60);
                const y = Math.random() * (area.clientHeight - 60);

                shape.style.left = x + 'px';
                shape.style.top = y + 'px';
                shape.style.position = 'absolute';

                shape.addEventListener('click', function (event) {
                    const clickTime = performance.now();
                    const reactionTime = clickTime - shapeAppearTime;

                    const isCorrect = shapeType === targetShape ? 1 : 0;
                    if (isCorrect) {
                        // Correct selection
                        timingsArray.push({ reactionTime, isCorrect });
                        shapeCount++;
                        totalShapeDotsCount++;
                        $('#shapeCount').text(`Shapes Selected: ${shapeCount}/${totalShapes}`);
                        let progress = Math.floor((totalShapeDotsCount / (totalDots + totalShapes)) * 100);
                        updateProgressBar(progress);
                        showShapes(); // Proceed to next shape selection
                    } else {
                        // Incorrect selection
                        alert('Wrong shape clicked. Please select the correct shape.');
                        // Do not count the shape, let user try again
                    }
                });

                $('#shapesContainer').append(shape);
            });
        }

        showShapes(); // Start with the first shape selection

        // Store the timings array and mouse movements for the second attempt
        shapeTimings[0] = timingsArray;
    }

    // Capture typing patterns
    $('#inputText').on('keydown', function (event) {
        let currentTime = new Date().getTime();
        // Time between key presses
        if (lastKeyTime !== null) {
            let interval = currentTime - lastKeyTime;
            if (!timings[currentAttempt - 1]) {
                timings[currentAttempt - 1] = [];
            }
            timings[currentAttempt - 1].push(interval);
        }
        lastKeyTime = currentTime;

        // Record keydown time for key hold duration
        if (!keyHoldTimes[currentAttempt - 1]) {
            keyHoldTimes[currentAttempt - 1] = [];
        }
        keyHoldTimes[currentAttempt - 1].push({
            keydownTime: currentTime,
            keyupTime: null,
            duration: null
        });
    });

    $('#inputText').on('keyup', function (event) {
        let currentTime = new Date().getTime();

        // Find the last keyHoldTimes entry with null keyupTime
        let keyDataArray = keyHoldTimes[currentAttempt - 1];
        for (let i = keyDataArray.length - 1; i >= 0; i--) {
            if (keyDataArray[i].keyupTime === null) {
                keyDataArray[i].keyupTime = currentTime;
                keyDataArray[i].duration = keyDataArray[i].keyupTime - keyDataArray[i].keydownTime;
                break;
            }
        }
    });

    // Prevent copy, paste, and cut in the input field
    //$('#inputText').on('copy paste cut', function (e) {
    //    e.preventDefault();
    //    alert('Copying and pasting are disabled. Please type the text manually.');
    //});

    //// Disable context menu (right-click) on the input field
    //$('#inputText').on('contextmenu', function (e) {
    //    e.preventDefault();
    //});s

    // Next Attempt button functionality
    $('#nextAttempt').click(async function () {
        const userInput = $('#inputText').val().trim();
        if (userInput === '') {
            alert('Please type the text before proceeding.');
            return;
        }
        if (userInput !== promptTexts[currentAttempt - 1].trim()) {
            alert('Your input does not match the prompt text. Please try again.');
            return;
        }
        if (!timings[currentAttempt - 1] || timings[currentAttempt - 1].length < 1) {
            alert('Not enough typing data captured. Please try typing again.');
            return;
        }
        if (!keyHoldTimes[currentAttempt - 1] || keyHoldTimes[currentAttempt - 1].length < 1) {
            alert('Not enough key hold data captured. Please try typing again.');
            return;
        }
        let dotArray = dotTimings[currentAttempt - 1];
        if (!dotArray || dotArray.length < totalDots) {
            alert(`You need to click all ${totalDots} dots before proceeding.`);
            return;
        }

        // Shapes:
        let shapeArray = shapeTimings[currentAttempt - 1];
        if (!shapeArray || shapeArray.length < totalShapes) {
            alert(`You need to select the shape ${totalShapes} times before proceeding.`);
            return;
        }

        userInputs.push(userInput);

        if (currentAttempt < totalAttempts) {
            // Prepare for the next attempt
            currentAttempt++;
            totalShapeDotsCount = 0;
            lastKeyTime = null;
            $('#inputText').val('');
            $('#currentAttempt').text(currentAttempt);
            updateProgressBar(0);

            $('#dotArea').empty()
                .append('<div id="dotCount" class="dot-count">Dots Clicked: 0/5</div>');
            $('#shapesContainer').empty();
            $('#shapeArea').off('mousemove');

            // CHANGED: Show both dotArea & shapeArea
            $('#dotArea').show();
            $('#shapeArea').show();

            startDotSequence();     // CHANGED: always start a new dot sequence
            startShapeSelection();  // CHANGED: always start a new shape selection

            $('#promptText').text(promptTexts[currentAttempt - 1]);

            // After the second attempt, send data to the server
            if (currentAttempt === 3) {
                $("#nextAttempt").text("SUBMIT");
            }
            } else {
                // All attempts completed, proceed to results
                $(this).addClass('btn-animated');
                $('#testSection').hide();
                await saveDataToServer();
              //  fetchDataAndCalculateResults();
            }
    });

    async function saveDataToServer() {
            const dataToSend = {
                timings: timings.slice(0, 3),               // Sending first 3 attempts for timings
                keyHoldTimes: keyHoldTimes.slice(0, 3),     // Sending first 3 attempts for key hold times
                dotTimings: dotTimings.slice(0, 3),         // Sending first 3 attempts for dot timings
                shapeTimings: shapeTimings.slice(0, 3),     // Sending first 3 attempts for shape timings
                shapeMouseMovements: shapeMouseMovements.slice(0, 3)
            };

        var response = await AjaxCall("/TrainModel/SaveUserData", dataToSend);
        if (response != "") {
            try {
                var result = JSON.parse(response);
                if (result.status === "SUCCESS") {
                    AlertTost("success", result.message);
                } else {
                    AlertTost("error", result.message);
                }
            } catch (e) {
                console.error("Error parsing response:", e);
                AlertTost("error", "Unexpected error. Please try again.");
            }
        }
        else {
            AlertTost("error", "Try Again!!!");
        }
        $('#matchingPercent').text("100");
        $('#resultSection').show();
    };

    // Function to compare multiple attempts of key press intervals
    async function compareMultipleAttempts(dataArray) {
        let totalScore = 0;
        let comparisons = 0;
        for (let i = 0; i < dataArray.length - 1; i++) {
            for (let j = i + 1; j < dataArray.length; j++) {
                let score = await comparePatterns(dataArray[i], dataArray[j]);
                totalScore += score;
                comparisons++;
            }
        }
        let averageScore = totalScore / comparisons;
        return averageScore;
    }

    // Function to compare multiple attempts of key hold times
    async function compareMultipleKeyHolds(dataArray) {
        let totalScore = 0;
        let comparisons = 0;
        for (let i = 0; i < dataArray.length - 1; i++) {
            for (let j = i + 1; j < dataArray.length; j++) {
                let durations1 = dataArray[i].map(item => item.duration);
                let durations2 = dataArray[j].map(item => item.duration);
                let score = await comparePatterns(durations1, durations2);
                totalScore += score;
                comparisons++;
            }
        }
        let averageScore = totalScore / comparisons;
        return averageScore;
    }

    // Function to compare action patterns (dot timings)
    async function compareActions(dotTimingsArray) {
        let actionScores = [];

        // Compare dot timings from previous attempts
        if (dotTimingsArray.length >= 2) {
            let dotScore = await comparePatterns(dotTimingsArray[0], dotTimingsArray[1]);
            actionScores.push(dotScore);
        }

        // Calculate average action score
        let totalActionScore = actionScores.reduce((a, b) => a + b, 0);
        let averageActionScore = totalActionScore / actionScores.length;
        return averageActionScore;
    }

    // Function to compare patterns
    async function comparePatterns(t1, t2) {
        let minLength = Math.min(t1.length, t2.length);
        let diffTotal = 0;
        let maxInterval = 1000; // Max interval considered (in ms)
        for (let i = 0; i < minLength; i++) {
            let diff = Math.abs(t1[i] - t2[i]);
            diffTotal += diff;
        }
        let avgDiff = diffTotal / minLength;
        let matchingScore = Math.max(0, (1 - (avgDiff / maxInterval)) * 100);
        return matchingScore;
    }

    // Try Again button functionality
    $('#tryAgain').click(function () {
        initializeTest();
    });
});
