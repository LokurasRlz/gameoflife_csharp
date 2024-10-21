namespace ConwaysGameOfLifeAPI.Models
{
    public class GameResult
    {
        public Guid BoardId { get; set; }
        public int[][] State { get; set; } // Change to int[,] for 0/1 representation
        public int Generation { get; set; }
        public bool IsFinalState { get; set; }
    }
}
