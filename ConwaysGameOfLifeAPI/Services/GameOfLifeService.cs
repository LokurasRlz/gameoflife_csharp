using ConwaysGameOfLifeAPI.Models;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace ConwaysGameOfLifeAPI.Services
{
    public class GameOfLifeService : IGameOfLifeService
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<Guid, Board> _boards = new();

        public GameOfLifeService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Guid UploadBoard(int rows, int columns, int[][] initialState)
        {
            var board = new Board
            {
                Rows = rows,
                Columns = columns,
                CurrentState = initialState
            };
            _boards[board.Id] = board;
            return board.Id;
        }

        public GameResult GetNextState(Guid boardId)
        {
            if (!_cache.TryGetValue(boardId, out Board board))
            {
                if (!_boards.TryGetValue(boardId, out board))
                    throw new KeyNotFoundException("Board not found.");
            }

            var nextState = ComputeNextState(board.CurrentState);
            board.CurrentState = nextState;

            _cache.Set(boardId, board);

            return new GameResult
            {
                BoardId = board.Id,
                State = nextState,
                Generation = 1,
                IsFinalState = IsStableState(board.CurrentState)
            };
        }

        public GameResult GetNthState(Guid boardId, int n)
        {
            if (!_cache.TryGetValue(boardId, out Board board)) { 
                if (!_boards.TryGetValue(boardId, out board))
                    throw new KeyNotFoundException("Board not found.");
            }

            var currentState = board.CurrentState;
            for (int i = 0; i < n; i++)
            {
                currentState = ComputeNextState(currentState);
                if (IsStableState(currentState))
                {
                    return new GameResult
                    {
                        BoardId = board.Id,
                        State = currentState,
                        Generation = i + 1,
                        IsFinalState = true
                    };
                }

            }

            board.CurrentState = currentState;
            _cache.Set(board.Id, board);
            return new GameResult
            {
                BoardId = board.Id,
                State = currentState,
                Generation = n,
                IsFinalState = IsStableState(currentState)
            };
        }

        public GameResult GetFinalState(Guid boardId, int maxGenerations)
        {
            return GetNthState(boardId, maxGenerations);
        }

        // Game Rule

        // Live cell with fewer than two live neighbors dies (underpopulation).
        // Live cell with two or three live neighbors lives on to the next generation.
        // Live cell with more than three live neighbors dies (overpopulation).
        // Dead cell with exactly three live neighbors becomes a live cell (reproduction).

        //  implement the game logic of the rule.
        private int[][] ComputeNextState(int[][] currentState)
        {
            int rows = currentState.Length;
            int columns = currentState.Length;
            int[][] nextState = new int[rows][];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    int liveNeighbors = CountLiveNeighbors(currentState, row, col);

                    // Apply the rules of Conway's Game of Life using 0/1 representation
                    if (currentState[row][col] == 1)
                    {
                        // Any live cell with two or three live neighbors survives
                        nextState[row][col] = (liveNeighbors == 2 || liveNeighbors == 3) ? 1 : 0;
                    }
                    else
                    {
                        // Any dead cell with exactly three live neighbors becomes a live cell
                        nextState[row][col] = (liveNeighbors == 3) ? 1 : 0;
                    }
                }
            }
            return nextState;
        }

        private int CountLiveNeighbors(int[][] state, int row, int col)
        {
            int rows = state.GetLength(0);
            int columns = state.GetLength(1);
            int liveNeighbors = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue; // Skip the cell itself

                    int neighborRow = row + i;
                    int neighborCol = col + j;

                    if (neighborRow >= 0 && neighborRow < rows && neighborCol >= 0 && neighborCol < columns)
                    {
                        liveNeighbors += state[neighborRow][neighborCol];
                    }
                }
            }
            return liveNeighbors;
        }

        private bool IsStableState(int[][] state)
        {
            // If there are no live cells, or the state does not change after applying the rules, it is stable
            for (int row = 0; row < state.Length; row++)
            {
                for (int col = 0; col < state.Length; col++)
                {
                    if (state[row][col] == 1)
                    {
                        return false; // The state is not stable if at least one cell is alive
                    }
                }
            }
            return true;
        }
    }
}
