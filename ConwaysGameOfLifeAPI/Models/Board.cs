namespace ConwaysGameOfLifeAPI.Models
{
    public class Board
    {
        //private int[,]? currentState;

        public Guid Id { get; set; } = Guid.NewGuid();
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int[][] CurrentState { get; set; }
        //public int[,]? CurrentState { get => currentState; set => currentState = value; } // Change to int[,] for 0/1 representation
    }
}
