using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Models.Models;
using Movies.Client.ApiServices;
using System.Diagnostics;

namespace Movies.Client.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        private readonly IMovieApiService _movieApiService;
        public MoviesController(IMovieApiService movieApiService)
        {
            _movieApiService = movieApiService;
        }

        // Logging Token and Claims
        public async Task LogTokenAndClaims()
        {
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            Debug.WriteLine($"Identity token: {identityToken}");

            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
        }

        // Logout
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> OnlyAdmin()
        {
            var userInfo = await _movieApiService.GetUserInfo();
            return View(userInfo);
        }

        // GET: MoviesController
        public async Task<ActionResult> Index()
        {
            return View(await _movieApiService.GetMovies());
        }

        // GET: MoviesController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            return View(await _movieApiService.GetMovie(id));
        }

        // GET: MoviesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MoviesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Movie movie)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var movieResult = await _movieApiService.CreateMovie(movie);
                    if (movieResult != null)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                return View(movie);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return View(movie);
            }
        }

        // GET: MoviesController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return View(await _movieApiService.GetMovie(id));
        }

        // POST: MoviesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Movie movie)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var movieResult = await _movieApiService.UpdateMovie(movie);
                    if (movieResult != null)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                return View(movie);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return View(movie);
            }
        }

        // GET: MoviesController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            return View(await _movieApiService.GetMovie(id));
        }

        // POST: MoviesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeletePost(int id)
        {
            try
            {
                await _movieApiService.DeleteMovie(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
