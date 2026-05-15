namespace MiniProjet.Models
{
    public class ProfileViewModel
    {
        public string UserName { get; set; } = "";
        public string? Email { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public bool PeutModifier { get; set; }
    }
}
