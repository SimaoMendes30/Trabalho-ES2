namespace Backend.DTO_s
{
    public class UtilizadorUpdateDTO
    {
        public string Nome { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int? NumHoras { get; set; }
    }
}