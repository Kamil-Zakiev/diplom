namespace AnalizingEllepticCurveResult
{
    public class Statistic
    {
        public int DividerDigitsCount { get; set; }
        public double MinMs { get; set; }
        public double AverageMs { get; set; }
        public double MaxMs { get; set; }

        public int[] Hist { get; set; }
    }
}