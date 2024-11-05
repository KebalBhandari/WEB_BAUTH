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

    const totalDots = 15; // Number of dots to click
    const totalShapes = 15; // Number of shape selections
    const totalAttempts = 3; // Number of attempts
    let currentAttempt = 1;

    let timings = []; // Array to hold typing timings for all attempts
    let dotTimings = []; // Array to hold dot timings for attempts 1 and 3
    let shapeTimings = []; // Array to hold shape selection timings for attempt 2
    let promptTexts = []; // Array to hold prompt texts for each attempt
    let userInputs = []; // Array to hold user inputs for each attempt

    // Declare lastTime at the top level
    let lastTime = null;

    // Initialize the test
    function initializeTest() {
        // Reset variables
        timings = [];
        dotTimings = [];
        shapeTimings = [];
        userInputs = [];
        promptTexts = [];
        lastTime = null;
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
        const area = document.getElementById('shapeArea');

        function showShapes() {
            if (shapeCount >= totalShapes) {
                // All shapes selected, clear shapes
                $('#shapesContainer').empty();
                $('#shapeQuestion').text('All shapes selected.');
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

        // Store the timings array for the second attempt
        shapeTimings[0] = timingsArray;
    }

    // Capture typing pattern
    $('#inputText').on('keydown', function (event) {
        let currentTime = new Date().getTime();
        if (lastTime !== null) {
            let interval = currentTime - lastTime;
            if (!timings[currentAttempt - 1]) {
                timings[currentAttempt - 1] = [];
            }
            timings[currentAttempt - 1].push(interval);
        }
        lastTime = currentTime;
    });

    // Next Attempt button functionality
    $('#nextAttempt').click(function () {
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
            lastTime = null;
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
        } else {
            // All attempts completed, proceed to results
            $(this).addClass('btn-animated');
            $('#testSection').hide();
            calculateResults();
        }
    });

    // Calculate and display results
    function calculateResults() {
        let typingMatch = compareMultipleAttempts(timings);
        let actionMatch = compareActions();
        let overallMatch = (typingMatch * 0.6) + (actionMatch * 0.4); // Weighted average
        $('#matchingPercent').text(overallMatch.toFixed(2));
        $('#resultSection').show();

        // Prepare data to save
        const testData = {
            typingMatch: typingMatch,
            actionMatch: actionMatch,
            overallMatch: overallMatch,
            timings: timings,
            dotTimings: dotTimings,
            shapeTimings: shapeTimings
        };

        // Save data to Firestore
        //  saveDataToFirestore(testData);
    }


    // Function to save data to Firebase
    function saveDataToFirestore(data) {
        const user = auth.currentUser;
        if (user) {
            const userId = user.uid;

            // Destructure to exclude nested arrays
            const { timings, dotTimings, shapeTimings, ...testData } = data;

            // Add userId and timestamp to testData
            testData.userId = userId;
            testData.timestamp = firebase.firestore.FieldValue.serverTimestamp();

            db.collection('tests').add(testData)
                .then((docRef) => {
                    // Save 'timings' as subcollection
                    if (timings && timings.length > 0) {
                        timings.forEach((attemptTimings, index) => {
                            db.collection('tests').doc(docRef.id).collection('timings').add({
                                attempt: index + 1,
                                timings: attemptTimings
                            });
                        });
                    }

                    // Save 'dotTimings' as subcollection
                    if (dotTimings && dotTimings.length > 0) {
                        dotTimings.forEach((attemptDotTimings, index) => {
                            db.collection('tests').doc(docRef.id).collection('dotTimings').add({
                                attempt: index + 1,
                                timings: attemptDotTimings
                            });
                        });
                    }

                    // Save 'shapeTimings' as subcollection
                    if (shapeTimings && shapeTimings.length > 0) {
                        shapeTimings.forEach((attemptShapeTimings, index) => {
                            db.collection('tests').doc(docRef.id).collection('shapeTimings').add({
                                attempt: index + 1,
                                timings: attemptShapeTimings
                            });
                        });
                    }

                    console.log('Data saved successfully with ID:', docRef.id);
                })
                .catch((error) => {
                    console.error('Error saving data:', error);
                });
        } else {
            console.error('User not authenticated');
            window.location.href = 'login.html';
            // Optionally redirect to login page
        }
    }


    // Function to compare multiple attempts
    function compareMultipleAttempts(dataArray) {
        let totalScore = 0;
        let comparisons = 0;
        for (let i = 0; i < dataArray.length - 1; i++) {
            for (let j = i + 1; j < dataArray.length; j++) {
                let score = comparePatterns(dataArray[i], dataArray[j]);
                totalScore += score;
                comparisons++;
            }
        }
        let averageScore = totalScore / comparisons;
        return averageScore;
    }

    // Function to compare action patterns (dot timings and shape timings)
    function compareActions() {
        let actionScores = [];

        // Compare dot timings from attempts 1 and 3
        if (dotTimings.length >= 2) {
            let dotScore = comparePatterns(dotTimings[0], dotTimings[1]);
            actionScores.push(dotScore);
        }

        // For shape selection in attempt 2, compute average reaction time and accuracy
        if (shapeTimings.length > 0 && shapeTimings[0].length > 0) {
            let totalReactionTime = 0;
            let correctSelections = 0;
            let totalSelections = shapeTimings[0].length;

            shapeTimings[0].forEach(item => {
                totalReactionTime += item.reactionTime;
                if (item.isCorrect) {
                    correctSelections++;
                }
            });

            let avgReactionTime = totalReactionTime / totalSelections;
            let accuracy = (correctSelections / totalSelections) * 100;

            let maxReactionTime = 5000; // Max reaction time considered (in ms)
            let reactionTimeScore = Math.max(0, (1 - (avgReactionTime / maxReactionTime)) * 100);

            // We can weigh accuracy and reaction time equally
            let shapeScore = (reactionTimeScore + accuracy) / 2;
            actionScores.push(shapeScore);
        }

        // Calculate average action score
        let totalActionScore = actionScores.reduce((a, b) => a + b, 0);
        let averageActionScore = totalActionScore / actionScores.length;
        return averageActionScore;
    }

    // Function to compare patterns
    function comparePatterns(t1, t2) {
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