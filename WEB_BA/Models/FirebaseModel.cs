using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace WEB_BA.Models
{
    [FirestoreData]
    public class UserDataModel
    {
        public List<List<double>>? Timings { get; set; }

        public List<List<KeyHoldTime>>? KeyHoldTimes { get; set; }

        public List<List<double>>? DotTimings { get; set; }

        public List<List<ShapeTiming>>? ShapeTimings { get; set; }

        public List<List<MouseMovement>>? ShapeMouseMovements { get; set; }

    }

    [FirestoreData]
    public class KeyHoldTime
    {

        [FirestoreProperty("duration")]
        public double? Duration { get; set; }
    }

    [FirestoreData]
    public class ShapeTiming
    {
        [FirestoreProperty("reactionTime")]
        public double? ReactionTime { get; set; }

        [FirestoreProperty("isCorrect")]
        public int? IsCorrect { get; set; }
    }

    [FirestoreData]
    public class MouseMovement
    {
        [FirestoreProperty("time")]
        public double? Time { get; set; }

        [FirestoreProperty("x")]
        public double? X { get; set; }

        [FirestoreProperty("y")]
        public double? Y { get; set; }
    }

}
