namespace MiniProjet.Models
{
    public class HomeDashboardViewModel
    {
        public int TotalArticles { get; set; }
        public int TotalCategories { get; set; }
        public List<CategoryStat> ArticlesParCategorie { get; set; } = new();
        public List<Article> DerniersArticles { get; set; } = new();
        public bool EstConnecte { get; set; }
        public string? NomUtilisateur { get; set; }
        public string? RolePrincipal { get; set; }
    }

    public class CategoryStat
    {
        public string NomCategorie { get; set; } = "";
        public int NombreArticles { get; set; }
    }
}
