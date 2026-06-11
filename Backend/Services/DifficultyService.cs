using FluffGameApi.Dtos;
using FluffGameApi.Repositories;

namespace FluffGameApi.Services
{
    public class DifficultyService
    {
        private readonly IDifficultyRepository _difficultyRepository;

        public DifficultyService(IDifficultyRepository difficultyRepository)
        {
            _difficultyRepository = difficultyRepository;
        }

        public async Task<(bool success, string message, List<DifficultyDto> difficulties)> GetAll()
        {
            try
            {
                var difficulties = await _difficultyRepository.GetAll();
                return (true, "Niveles de dificultad obtenidos correctamente.", difficulties);
            }
            catch (Exception ex)
            {
                return (false, $"Error obteniendo los niveles de dificultad.", []);
            }
        }
    }
}