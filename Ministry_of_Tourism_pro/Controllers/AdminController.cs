using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ministry_of_Tourism_pro.Models;

namespace Ministry_of_Tourism_pro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            var model = new EvaluationViewModel
            {
                EstablishmentName = "Sheraton Addis",
                Date = DateTime.Now.ToString("yyyy-MM-dd"), // Correct format for <input type="date">
                Categories = EvaluationMockData.GetCategories()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(EvaluationViewModel model)
        {
            // In a real scenario, we would save the model to a database here.
            // For now, we'll just set a success message and re-display the model.
            
            TempData["SuccessMessage"] = "እንኳን ደስ አላችሁ! ሪፖርቱ በተሳካ ሁኔታ ተልኳል። / Report submitted successfully!";
            
            // Re-populate categories if they weren't fully posted or for validation errors
            if (model.Categories == null || model.Categories.Count == 0)
            {
                model.Categories = EvaluationMockData.GetCategories();
            }

            return View(model);
        }
    }
}
