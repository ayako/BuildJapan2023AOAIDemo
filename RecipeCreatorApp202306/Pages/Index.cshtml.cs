using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SkillDefinition;
using System.Text.Json;

namespace RecipeCreatorApp202306.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string? Ingredients { get; set; }
        public string? Family { get; set; }
        public string? Menu { get; set; }
        public string? Point { get; set; }
        public string? Recipe { get; set; }
        public string? Advice { get; set; }

        private readonly ILogger<IndexModel> _logger;
        private static IKernel kernel;
        private static IDictionary<string, ISKFunction> function;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            kernel = new KernelBuilder()
                .WithAzureTextCompletionService("text-davinci-003","https://YOUR_SERVICE_NAME.openai.azure.com/","YOUR_API_KEY")
                .Build();
            function = kernel.ImportSemanticSkillFromDirectory("Skills", "RecipeCreatorSkill");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostCreateMenuAsync(string Ingredients, string Family)
        {
            var context = kernel.CreateNewContext();
            context["ingredients"] = Ingredients;
            context["family"] = Family;

            var result = await function["Recipe"].InvokeAsync(context);

            if (string.IsNullOrEmpty(result.Result))
            {
                Menu = "NoCompletionResponded: 入力値を変更してやり直してみてください";
            }
            else 
            {
                var menuResult = JsonSerializer.Deserialize<MenuResult>(result.Result);
                Menu = menuResult.menu;
                Point = menuResult.point;
                Recipe = menuResult.recipe;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateAdviceAsync(string Menu, string Ingredients, string Family, string Point, string Recipe)
        {
            var result = await function["Advice"].InvokeAsync(Menu);

            if (string.IsNullOrEmpty(result.Result))
            {
                Advice = "NoCompletionResponded: 入力値を変更してやり直してみてください";
            }
            else
            {
                var adviceResult = JsonSerializer.Deserialize<AdviceResult>(result.Result);
                Advice = $"足りない栄養素は {adviceResult.nutrients} です。{adviceResult.howtocover}";
            }

            return Page();
        }
    }
}