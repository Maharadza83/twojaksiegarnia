using Microsoft.AspNetCore.Mvc;
using ksiegarnia.Repositories.Abstract;
using System.Security.Claims;

namespace ksiegarnia.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMovieService _movieService;

        public HomeController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        public IActionResult Index(string term = "", int currentPage = 1)
        {
            var movies = _movieService.List(term, true, currentPage);
            return View(movies);
        }

        public IActionResult About()
        {
            var username = User.FindFirst("Username")?.Value;
            var email = User.FindFirst("Email")?.Value;

            var model = new UserInfoViewModel
            {
                Username = username,
                Email = email
            };

            return View(model);
        }

        public IActionResult MovieDetail(int movieId)
        {
            var movie = _movieService.GetById(movieId);
            return View(movie);
        }
    }

    public class UserInfoViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
