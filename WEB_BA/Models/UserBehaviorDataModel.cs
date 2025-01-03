namespace WEB_BA.Models
{
    public class UserBehaviorDataModel
    {
        public int UserId { get; set; }

        public List<List<double>> Timings { get; set; } = new List<List<double>>();

        public List<List<KeyHoldTime>> KeyHoldTimes { get; set; } = new List<List<KeyHoldTime>>();

        public List<List<double>> DotTimings { get; set; } = new List<List<double>>();

        public List<List<ShapeTiming>> ShapeTimings { get; set; } = new List<List<ShapeTiming>>();

        public List<List<MouseMovement>> ShapeMouseMovements { get; set; } = new List<List<MouseMovement>>();
    }
}
