using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using LocalSportsLeague.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LocalSportsLeague.Controllers
{
    public class AccountController : Controller
    {
        private readonly Team112DBContext _context;

        public AccountController(Team112DBContext context)
        {
            _context = context;
        }

        public IActionResult Login(string returnURL)
        {
            returnURL = string.IsNullOrEmpty(returnURL) ? "~/Games/SubmitScores" : returnURL;

            return View(new LoginInput { ReturnURL = returnURL });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,UserPassword,ReturnURL")] LoginInput loginInput)
        {
            if (ModelState.IsValid)
            {
                var aUser = await _context.Official.FirstOrDefaultAsync(u => u.Email == loginInput.Username && u.Password == loginInput.UserPassword);

                if (aUser != null)
                {
                    var claims = new List<Claim>();

                    claims.Add(new Claim(ClaimTypes.Email, aUser.Email));
                    claims.Add(new Claim(ClaimTypes.Name, aUser.Fname));
                    claims.Add(new Claim(ClaimTypes.Sid, aUser.Officialid.ToString()));

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return LocalRedirect(loginInput?.ReturnURL ?? "~/Games/SubmitScores");
                    
                }
                else
                {
                    ViewData["message"] = "Invalid credentials";
                    return View(loginInput);
                }
            }

            return View(loginInput);
        }

        public async Task<RedirectToActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}