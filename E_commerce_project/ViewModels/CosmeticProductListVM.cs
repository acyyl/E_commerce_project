using E_commerce_project.Models;


namespace E_Commerce_Project.ViewModels
{
    
        public class CosmeticProductListVM

        {
            public IEnumerable<CosmeticProduct> Products { get; set; }  // List of cosmetic products
        public string Message { get; set; }  // Message to show when no products are available
        public int CurrentPage { get; internal set; }
        public int TotalPages { get; internal set; }

        public List<string> Categories { get; set; }

        // Selected category for filtering
        public string SelectedCategory { get; set; }

        public List<CosmeticProduct> CosmeticProducts { get; set; }

        // Selected popularity for filtering
        public int? SelectedPopularity { get; set; }

        // Minimum price for filtering
        public decimal? MinPrice { get; set; }

        // Maximum price for filtering
        public decimal? MaxPrice { get; set; }

    }
    

}
