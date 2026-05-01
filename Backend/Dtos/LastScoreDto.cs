namespace FluffGameApi.Dtos
{
    public class LastScoreDto
    {
        public int TotalPoints { get; set; }
        public int RedPoints { get; set; }
        public int BluePoints { get; set; }
        public int GreenPoints { get; set; }
        public int YellowPoints { get; set; }
        public string DifficultyName { get; set; }
    }
}
