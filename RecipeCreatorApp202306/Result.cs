namespace RecipeCreatorApp202306
{
    public class MenuResult
    {
        public string menu { get; set; }
        public string ingredients_notstock { get; set; }
        public string point { get; set; }
        public string recipe { get; set; }
    }

    public class AdviceResult
    {
        public string nutrients { get; set; }
        public string howtocover { get; set; }
    }
}
