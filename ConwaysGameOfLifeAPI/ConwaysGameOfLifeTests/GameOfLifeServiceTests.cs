using ConwaysGameOfLifeAPI.Models;
using ConwaysGameOfLifeAPI.Services;
using Xunit;
using Microsoft.Extensions.Caching.Memory;

namespace ConwaysGameOfLifeAPI.ConwaysGameOfLifeTests
{
    public class GameOfLifeServiceTests
    {
        private readonly GameOfLifeService _service;

        public GameOfLifeServiceTests()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            _service = new GameOfLifeService(memoryCache);
        }

        [Fact]
        public void UploadBoard_ShouldReturnValidBoardId()
        {
            int rows = 5;
            int columns = 5;
            int[][] initialState = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                initialState[i] = new int[columns];
            }
            initialState[2][2] = 1;

            Guid boardId = _service.UploadBoard(rows, columns, initialState);

            Assert.NotEqual(Guid.Empty, boardId);
        }

        [Fact]
        public void GetNextState_ShouldReturnNextState()
        {
            int rows = 3;
            int columns = 3;
            int[][] initialState = new int[3][]
            {
                new int[] { 0, 1, 0 },
                new int[] { 0, 1, 0 },
                new int[] { 0, 1, 0 }
            };

            Guid boardId = _service.UploadBoard(rows, columns, initialState);
            GameResult result = _service.GetNextState(boardId);

            Assert.Equal(boardId, result.BoardId);
            Assert.Equal(0, result.State[0][1]);
            Assert.Equal(1, result.State[1][1]);
            Assert.Equal(0, result.State[2][1]);
        }

        [Fact]
        public void GetNthState_ShouldReturnCorrectGeneration()
        {
            int rows = 3;
            int columns = 3;
            int[][] initialState = new int[3][]
            {
                new int[] { 0, 1, 0 },
                new int[] { 0, 1, 0 },
                new int[] { 0, 1, 0 }
            };
            Guid boardId = _service.UploadBoard(rows, columns, initialState);
            GameResult result = _service.GetNthState(boardId, 2);

            Assert.Equal(2, result.Generation);
        }

        [Fact]
        public void GetFinalState_ShouldReturnStableState()
        {
            int rows = 3;
            int columns = 3;
            int[][] initialState = new int[3][]
            {
                new int[] { 0, 1, 0 },
                new int[] { 0, 1, 0 },
                new int[] { 0, 1, 0 }
            };

            Guid boardId = _service.UploadBoard(rows, columns, initialState);
            GameResult result = _service.GetFinalState(boardId, 10);

            Assert.True(result.IsFinalState);
        }
    }
}
