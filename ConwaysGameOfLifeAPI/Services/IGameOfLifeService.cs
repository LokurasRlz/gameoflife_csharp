using ConwaysGameOfLifeAPI.Models;

namespace ConwaysGameOfLifeAPI.Services
{
    public interface IGameOfLifeService
    {
        Guid UploadBoard(int rows, int columns, int[][] initialState);
        GameResult GetNextState(Guid boardId);
        GameResult GetNthState(Guid boardId, int n);
        GameResult GetFinalState(Guid boardId, int maxGenerations);
    }
}
