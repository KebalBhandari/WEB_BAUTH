
namespace WEB_BA.Models
{
    public class UserDataModel
    {
        public string TokenNo { get; set; }
        public List<List<double>> Timings { get; set; }

        public List<List<KeyHoldTime>> KeyHoldTimes { get; set; }

        public List<List<double>> DotTimings { get; set; }

        public List<List<ShapeTiming>> ShapeTimings { get; set; }

        public List<List<MouseMovement>> ShapeMouseMovements { get; set; }

    }

    public class KeyHoldTime
    {
        public double Duration { get; set; }
    }

    public class ShapeTiming
    {
        public double ReactionTime { get; set; }

        public int IsCorrect { get; set; }
    }

    public class MouseMovement
    {
        public double Time { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }

}
