using Newtonsoft.Json;

namespace WEB_BA.Models
{
    public class UserDataModel
    {
        [JsonProperty("TokenNo")]
        public string TokenNo { get; set; }

        [JsonProperty("timings", NullValueHandling = NullValueHandling.Ignore)]
        public List<List<double>> Timings { get; set; } = new List<List<double>>();

        [JsonProperty("keyHoldTimes", NullValueHandling = NullValueHandling.Ignore)]
        public List<List<KeyHoldTime>> KeyHoldTimes { get; set; } = new List<List<KeyHoldTime>>();

        [JsonProperty("backspaceTimings", NullValueHandling = NullValueHandling.Ignore)]
        public List<List<BackSpaceTiming>> BackSpaceTimings { get; set; } = new List<List<BackSpaceTiming>>();

        [JsonProperty("dotTimings", NullValueHandling = NullValueHandling.Ignore)]
        public List<List<double>> DotTimings { get; set; } = new List<List<double>>();

        [JsonProperty("shapeTimings", NullValueHandling = NullValueHandling.Ignore)]
        public List<List<ShapeTiming>> ShapeTimings { get; set; } = new List<List<ShapeTiming>>();

        [JsonProperty("shapeMouseMovements", NullValueHandling = NullValueHandling.Ignore)]
        public List<List<MouseMovement>> ShapeMouseMovements { get; set; } = new List<List<MouseMovement>>();

        [JsonProperty("detectedLanguages")]
        public List<string> DetectedLanguages { get; set; } = new List<string>();
    }

    public class KeyHoldTime
    {
        [JsonProperty("keydownTime")]
        public double KeydownTime { get; set; }

        [JsonProperty("keyupTime")]
        public double KeyupTime { get; set; }

        [JsonProperty("duration")]
        public double Duration { get; set; }
    }

    public class BackSpaceTiming
    {
        [JsonProperty("time")]
        public double Time { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }
    }

    public class ShapeTiming
    {
        [JsonProperty("reactionTime")]
        public double ReactionTime { get; set; }

        [JsonProperty("isCorrect")]
        public int IsCorrect { get; set; }
    }

    public class MouseMovement
    {
        [JsonProperty("time")]
        public double Time { get; set; }

        [JsonProperty("x")]
        public double X { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }

        [JsonProperty("velocity")]
        public double Velocity { get; set; }

        [JsonProperty("slope")]
        public double Slope { get; set; }
    }
}
