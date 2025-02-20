namespace WEB_BA.Models
{
    public class UserDataModel
    {
        public string TokenNo { get; set; } // Unique identifier for the user
        public List<List<double>> Timings { get; set; } // Time between key presses for each attempt
        public List<List<KeyHoldTime>> KeyHoldTimes { get; set; } // Key hold durations for each attempt
        public List<List<double>> DotTimings { get; set; } // Reaction times for dots for each attempt
        public List<List<ShapeTiming>> ShapeTimings { get; set; } // Reaction times and correctness for shapes for each attempt
        public List<List<MouseMovement>> ShapeMouseMovements { get; set; } // Mouse movements during shape selection for each attempt
        public int BackspaceCount { get; set; } // Number of backspace key presses
        public List<MouseMovement> MousePathDots { get; set; } // Mouse movements during dot task
        public List<MouseMovement> MousePathShapes { get; set; } // Mouse movements during shape task
        public List<double> ClickPrecision { get; set; } // Distance from the center of dots
    }

    public class KeyHoldTime
    {
        public double Duration { get; set; } // Duration a key is held down
    }

    public class ShapeTiming
    {
        public double ReactionTime { get; set; } // Time taken to select a shape
        public int IsCorrect { get; set; } // 1 if correct, 0 if incorrect
    }

    public class MouseMovement
    {
        public double Time { get; set; } // Timestamp of the mouse movement
        public double X { get; set; } // X-coordinate of the mouse
        public double Y { get; set; } // Y-coordinate of the mouse
    }
}