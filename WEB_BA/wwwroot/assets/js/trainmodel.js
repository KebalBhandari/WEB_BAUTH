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
        "Big Fuji waves pitch enzymed kex liquor jugs."
    ];

    const totalDots = 2; // Number of dots to click
    const totalShapes = 2; // Number of shape selections
    const totalAttempts = 3;
    let currentAttempt = 1;

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
        // Reset variables
        timings = [];
        keyHoldTimes = [];
        dotTimings = [];
        shapeTimings = [];
        shapeMouseMovements = [];
        userInputs = [];
        promptTexts = [];
        lastKeyTime = null;
        currentAttempt = 1;
        $('#inputText').val('');
        $('#matchingPercent').text('0');
        $('#dotArea').empty().append('<div id="dotCount" class="dot-count">Dots Clicked: 0/15</div>');
        $('#shapeArea').hide();
        $('#dotArea').show();
        $('#nextAttempt').removeClass('btn-animated');
        $('#currentAttempt').text(currentAttempt);
        $('#taskInstruction').text('Please click on the dots that appear and type the following text:');
        updateProgressBar(0);

        // Randomly select sentences for each attempt
        for (let i = 0; i < totalAttempts; i++) {
            promptTexts.push(sentences[Math.floor(Math.random() * sentences.length)]);
        }
        $('#promptText').text(promptTexts[0]);

        // Show test section and hide results
        $('#testSection').show();
        $('#resultSection').hide();

        // Start dot sequence for the first attempt
        startDotSequence();
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

            const x = Math.random() * (area.clientWidth - 30);
            const y = Math.random() * (area.clientHeight - 30);

            dot.style.left = x + 'px';
            dot.style.top = y + 'px';

            const dotAppearTime = performance.now();
            dot.addEventListener('click', function (event) {
                const clickTime = performance.now();
                const reactionTime = clickTime - dotAppearTime;
                timingsArray.push(reactionTime);

                dot.remove();
                dotCount++;
                $('#dotCount').text(`Dots Clicked: ${dotCount}/${totalDots}`);
                let progress = Math.floor((dotCount / totalDots) * 100);
                updateProgressBar(progress);
                showDot(); // Show next dot
            });

            area.appendChild(dot);
        }

        showDot(); // Start with the first dot

        // Store the timings array for the current attempt
        if (currentAttempt === 1) {
            dotTimings[0] = timingsArray;
        } else if (currentAttempt === 3) {
            dotTimings[1] = timingsArray;
        }
    }

    // Function to start shape selection task
    function startShapeSelection() {
        $('#dotArea').hide();
        $('#shapeArea').show();
        $('#shapeArea').empty().append('<p id="shapeQuestion" class="mb-2"></p><div id="shapesContainer"></div><div id="shapeCount" class="dot-count">Shapes Selected: 0/15</div>');
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
                shapeMouseMovements[0] = mouseMovements;

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
                        $('#shapeCount').text(`Shapes Selected: ${shapeCount}/${totalShapes}`);
                        let progress = Math.floor((shapeCount / totalShapes) * 100);
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
    //});

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

        if (currentAttempt === 2) {
            if (!shapeTimings[0] || shapeTimings[0].length < totalShapes) {
                alert(`You need to select the correct shape ${totalShapes} times before proceeding.`);
                return;
            }
        } else if (currentAttempt === 1 || currentAttempt === 3) {
            let dotIndex = currentAttempt === 1 ? 0 : 1;
            if (!dotTimings[dotIndex] || dotTimings[dotIndex].length < totalDots) {
                alert(`You need to click all ${totalDots} dots before proceeding.`);
                return;
            }
        }

        userInputs.push(userInput);

        if (currentAttempt < totalAttempts) {
            // Prepare for the next attempt
            currentAttempt++;
            lastKeyTime = null;
            $('#inputText').val('');
            $('#currentAttempt').text(currentAttempt);
            updateProgressBar(0);

            if (currentAttempt === 2) {
                // Start shape selection task
                startShapeSelection();
            } else if (currentAttempt === 3) {
                // Reset to dot sequence
                $('#shapeArea').hide();
                $('#dotArea').show();
                $('#dotArea').empty().append('<div id="dotCount" class="dot-count">Dots Clicked: 0/15</div>');
                $('#taskInstruction').text('Please click on the dots that appear and type the following text:');
                startDotSequence();
            }

            $('#promptText').text(promptTexts[currentAttempt - 1]);

            // After the second attempt, send data to the server
            if (currentAttempt === 3) {
                await saveDataToServer();
            }
        } else {
            // All attempts completed, proceed to results
            $(this).addClass('btn-animated');
            $('#testSection').hide();
            fetchDataAndCalculateResults();
        }
    });

    // Function to send data to the server after the second attempt
    async function saveDataToServer() {
        const dataToSend = {
            timings: timings.slice(0, 2), // First two attempts
            keyHoldTimes: keyHoldTimes.slice(0, 2),
            dotTimings: dotTimings.slice(0, 1),
            shapeTimings: shapeTimings,
            shapeMouseMovements: shapeMouseMovements
        };

        var response = await AjaxCall("/TrainModel/SaveUserData",dataToSend);
        if (response != "") {
            AlertTost("success", "Data Saved Successfully");
        }
        else {
            AlertTost("error", "Try Again!!!");
        }
    }

    // Function to fetch data from the server after the third attempt
    async function fetchDataAndCalculateResults() {
        var savedData = await AjaxCallWithoutData("/TrainModel/GetUserData");
        if (savedData != "") {
            // Parse JSON if necessary
            if (typeof savedData === "string") {
                savedData = JSON.parse(savedData);
            }

            if (Array.isArray(savedData)) {
                await compareDataAndDisplayResults(savedData);
            } else {
                console.error("Error: Expected an array, but received:", savedData);
            }
        }
        else {
            AlertTost("Info", "Try Again!!!");
        }
    }

    // Function to compare data and display results
    async function compareDataAndDisplayResults(savedData) {
        // Extract data from the third attempt
        const thirdAttemptData = {
            timings: timings[2],
            keyHoldTimes: keyHoldTimes[2],
            dotTimings: dotTimings[1]
        };
        console.log('savedData:', savedData);
        console.log('ThirdAttemptData:', thirdAttemptData);
        // Combine savedData and thirdAttemptData for comparison

        let newAllTimings = [];
        let newAllKeyHoldTimes = [];
        let newAllDotTimings = [];

        // Iterate over savedDataArray to collect all timings
        savedData.forEach(async function (savedData) {
            if (savedData.timings) {
                newAllTimings = newAllTimings.concat(savedData.timings);
            }
            if (savedData.keyHoldTimes) {
                newAllKeyHoldTimes = newAllKeyHoldTimes.concat(savedData.keyHoldTimes);
            }
            if (savedData.dotTimings) {
                newAllDotTimings = newAllDotTimings.concat(savedData.dotTimings);
            }
        });

        // Add the third attempt data
        newAllTimings.push(thirdAttemptData.timings);
        newAllKeyHoldTimes.push(thirdAttemptData.keyHoldTimes);
        newAllDotTimings.push(thirdAttemptData.dotTimings);

        const allTimings = newAllTimings.concat([thirdAttemptData.timings]);
        const allKeyHoldTimes = newAllKeyHoldTimes.concat([thirdAttemptData.keyHoldTimes]);
        const allDotTimings = newAllDotTimings.concat([thirdAttemptData.dotTimings]);

        // Calculate matching scores
        let typingMatch = await compareMultipleAttempts(allTimings);
        let keyHoldMatch = await compareMultipleKeyHolds(allKeyHoldTimes);
        let actionMatch = await compareActions(allDotTimings);

        let overallMatch = (typingMatch * 0.4) + (keyHoldMatch * 0.3) + (actionMatch * 0.3);

        $('#matchingPercent').text(overallMatch.toFixed(2));
        $('#resultSection').show();

        // Optionally, you can send the final result to the server to save it
        // saveFinalResultToServer(overallMatch);
    }

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
