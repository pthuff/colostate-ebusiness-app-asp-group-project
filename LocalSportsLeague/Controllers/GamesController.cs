using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LocalSportsLeague.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Threading;

namespace LocalSportsLeague.Controllers
{
    public class GamesController : Controller
    {
        private readonly Team112DBContext _context;

        public GamesController(Team112DBContext context)
        {
            _context = context;
        }

        [Authorize]
        // GET: Games
        public async Task<IActionResult> Index(int? seasonid)
        {

            var team112DBContext = _context.Game
                .Include(g => g.Ateam)
                .Include(g => g.Hteam)
                .Include(g => g.Official)
                .Include(g => g.Season)
                .Include(g => g.SportNavigation)
                .Include(g => g.WinnerNavigation)
                .Where(g => g.Seasonid == seasonid)
              ;
            
            TempData["season"] = seasonid;
            return View(await team112DBContext.ToListAsync());
        }

        //[Authorize]
        //// GET: Games/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }

        //    var game = await _context.Game
        //        .Include(g => g.Ateam)
        //        .Include(g => g.Hteam)
        //        .Include(g => g.Official)
        //        .Include(g => g.Season)
        //        .Include(g => g.SportNavigation)
        //        .Include(g => g.WinnerNavigation)
        //        .FirstOrDefaultAsync(m => m.Gameid == id);
        //    if (game == null)
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(game);
        //}

        [Authorize]
        // GET: Games/Create
        public IActionResult Create(int? seasonid)
        {

            //get teams
            var teams = _context.SeasonTeam.Include(t => t.Team).Where(t => t.Seasonid == seasonid).ToList();
            ViewData["Hteamid"] = new SelectList(teams.OrderBy(h => h.Team.Name), "Teamid", "Team.Name");
            ViewData["Ateamid"] = new SelectList(teams.OrderBy(a => a.Team.Name), "Teamid", "Team.Name");

            //get official id
            var userId = this.User.FindFirstValue(ClaimTypes.Sid);
            TempData["official"] = userId;

            //set season id
            TempData["season"] = seasonid;

            //get sport id
            var sportId = _context.SeasonSport
                .Where(s => s.Seasonid == seasonid)
                .ToArray();
            TempData["sport"] = sportId[0].Sportid;

            return View();
        }

        [Authorize]
        // POST: Games/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Hteamid,Ateamid,Sportid,Seasonid,Officialid,Winner,Hscore,Ascore,Ot,Datetime")] Game game)
        {
            TempData["season"] = game.Seasonid;

            if (game.Hteamid == game.Ateamid)
            {

                //get teams
                var cteams = _context.SeasonTeam.Include(t => t.Team).Where(t => t.Seasonid == game.Seasonid).ToList();
                ViewData["Hteamid"] = new SelectList(cteams.OrderBy(h => h.Team.Name), "Teamid", "Team.Name", game.Hteamid);
                ViewData["Ateamid"] = new SelectList(cteams.OrderBy(a => a.Team.Name), "Teamid", "Team.Name", game.Ateamid);

                TempData["message"] = "A team cannot play against itself.";
                return View(game);
            }

            var sdate = _context.Season.Where(s => s.Seasonid == game.Seasonid).ToArray();
            if (game.Datetime > DateTime.Now || game.Datetime < sdate[0].Sdate)
            {
                //get teams
                var dteams = _context.SeasonTeam.Include(t => t.Team).Where(t => t.Seasonid == game.Seasonid).ToList();
                ViewData["Hteamid"] = new SelectList(dteams.OrderBy(h => h.Team.Name), "Teamid", "Team.Name", game.Hteamid);
                ViewData["Ateamid"] = new SelectList(dteams.OrderBy(a => a.Team.Name), "Teamid", "Team.Name", game.Ateamid);

                TempData["message"] = "Please select a valid date during the season.";
                return View(game);

            }

            var gameList = _context.Game.Where(g => g.Seasonid == game.Seasonid).ToList();
            foreach (Game g in gameList)
            {
                if (game.Datetime == g.Datetime)
                {
                    //get teams
                    var gteams = _context.SeasonTeam.Include(t => t.Team).Where(t => t.Seasonid == game.Seasonid).ToList();
                    ViewData["Hteamid"] = new SelectList(gteams.OrderBy(h => h.Team.Name), "Teamid", "Team.Name", game.Hteamid);
                    ViewData["Ateamid"] = new SelectList(gteams.OrderBy(a => a.Team.Name), "Teamid", "Team.Name", game.Ateamid);

                    TempData["message"] = "Please select a valid date when another game was not played.";
                    return View(game);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {

                if (game.Hscore > game.Ascore)
                {
                    game.Winner = game.Hteamid;
                }

                if (game.Ascore > game.Hscore)
                {
                    game.Winner = game.Ateamid;
                }

                _context.Add(game);
                await _context.SaveChangesAsync();
                }
                catch
                {
                    TempData["message"] = $"Game {game.Gameid} update failed";
                    return RedirectToAction("Index", new { seasonid = game.Seasonid });
                }
                TempData["message"] = $"Game {game.Gameid} has been added";
                return RedirectToAction("Index", new { seasonid = game.Seasonid });
            }

            var teams = _context.SeasonTeam.Include(t => t.Team).Where(t => t.Seasonid == game.Seasonid).ToList();
            ViewData["Hteamid"] = new SelectList(teams.OrderBy(h => h.Team.Name), "Teamid", "Team.Name", game.Hteamid);
            ViewData["Ateamid"] = new SelectList(teams.OrderBy(a => a.Team.Name), "Teamid", "Team.Name", game.Ateamid);

            return View(game);
        }

        [Authorize]
        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var game = await _context.Game.FindAsync(id);
            if (game == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var teams = _context.SeasonTeam.Include(t => t.Team).Where(t => t.Seasonid == game.Seasonid).ToList();
            ViewData["Hteamid"] = new SelectList(teams.OrderBy(h => h.Team.Name), "Teamid", "Team.Name", game.Hteamid);
            ViewData["Ateamid"] = new SelectList(teams.OrderBy(a => a.Team.Name), "Teamid", "Team.Name", game.Ateamid);

            TempData["season"] = game.Seasonid;
            return View(game);
        }

        [Authorize]
        // POST: Games/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Gameid,Hteamid,Ateamid,Sportid,Seasonid,Officialid,Winner,Hscore,Ascore,Ot,Datetime")] Game game)
        {
            TempData["season"] = game.Seasonid;

            if (id != game.Gameid)
            {
                return RedirectToAction("Index", new { seasonid = game.Seasonid });
            }

            if (game.Hteamid == game.Ateamid)
            {

                //get teams
                var eteams = _context.SeasonTeam.Include(t => t.Team).Where(t => t.Seasonid == game.Seasonid).ToList();
                ViewData["Hteamid"] = new SelectList(eteams.OrderBy(h => h.Team.Name), "Teamid", "Team.Name", game.Hteamid);
                ViewData["Ateamid"] = new SelectList(eteams.OrderBy(a => a.Team.Name), "Teamid", "Team.Name", game.Ateamid);

                TempData["message"] = "A team cannot play against itself.";
                return View(game);
            }

            var sdate = _context.Season.Where(s => s.Seasonid == game.Seasonid).ToArray();
            if (game.Datetime > DateTime.Now || game.Datetime < sdate[0].Sdate)
            {
                //get teams
                var edteams = _context.SeasonTeam.Include(t => t.Team).Where(t => t.Seasonid == game.Seasonid).ToList();
                ViewData["Hteamid"] = new SelectList(edteams.OrderBy(h => h.Team.Name), "Teamid", "Team.Name", game.Hteamid);
                ViewData["Ateamid"] = new SelectList(edteams.OrderBy(a => a.Team.Name), "Teamid", "Team.Name", game.Ateamid);

                TempData["message"] = "Please select a valid date during the season.";
                return View(game);

            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (game.Hscore > game.Ascore)
                    {
                        game.Winner = game.Hteamid;
                    }

                    if (game.Ascore > game.Hscore)
                    {
                        game.Winner = game.Ateamid;
                    }

                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                    TempData["message"] = $"Game {game.Gameid} update failed";
                    return RedirectToAction("Index", new { seasonid = game.Seasonid });
                }

                TempData["message"] = $"Game {game.Gameid} has been updated";
                return RedirectToAction("Index", new { seasonid = game.Seasonid });
            }

            var teams = _context.SeasonTeam.Include(t => t.Team).Where(t => t.Seasonid == game.Seasonid).ToList();
            ViewData["Hteamid"] = new SelectList(teams.OrderBy(h => h.Team.Name), "Teamid", "Team.Name", game.Hteamid);
            ViewData["Ateamid"] = new SelectList(teams.OrderBy(a => a.Team.Name), "Teamid", "Team.Name", game.Ateamid);

            return View(game);
        }

        [Authorize]
        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var game = await _context.Game
                .Include(g => g.Ateam)
                .Include(g => g.Hteam)
                .Include(g => g.Official)
                .Include(g => g.Season)
                .Include(g => g.SportNavigation)
                .Include(g => g.WinnerNavigation)
                .FirstOrDefaultAsync(m => m.Gameid == id);
            if (game == null)
            {
                TempData["message"] = $"Game {game.Gameid} not found";
                return RedirectToAction(nameof(Index));
            }

            TempData["season"] = game.Seasonid;
            return View(game);
        }

        [Authorize]
        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Game.FindAsync(id);

            TempData["season"] = game.Seasonid;

            if (game == null)
            {

                RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Game.Remove(game);
                await _context.SaveChangesAsync();
            }
            catch
            {
                TempData["message"] = $"Game {game.Gameid} not deleted";
                return RedirectToAction("Index", new { seasonid = game.Seasonid });
            }

            TempData["message"] = $"Game {game.Gameid} has been deleted";
            return RedirectToAction("Index", new { seasonid = game.Seasonid });
        }

        [Authorize]
        private bool GameExists(int id)
        {
            return _context.Game.Any(e => e.Gameid == id);
        }

        [Authorize]
        // GET: Games/SubmitScores
        public IActionResult SubmitScores()
        {

            ViewData["Seasonid"] = new SelectList(_context.Season.Where(s => s.Sdate <= DateTime.Now && s.Edate >= DateTime.Now), "Seasonid", "Name");

            return View();
        }

        //[Authorize]
        //// POST: Games/SubmitScores
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SubmitScores([Bind("Hteamid,Ateamid,Sportid,Seasonid,Officialid,Winner,Hscore,Ascore,Ot,Datetime")] Game game)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(game);
        //        await _context.SaveChangesAsync();
        //        TempData["message"] = $"Game {game.Gameid} has been added";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewData["Ateamid"] = new SelectList(_context.Team.OrderBy(a => a.Name), "Teamid", "Name", game.Ateamid);
        //    ViewData["Hteamid"] = new SelectList(_context.Team.OrderBy(h => h.Name), "Teamid", "Name", game.Hteamid);
        //    //ViewData["Officialid"] = new SelectList(_context.Official, "Officialid", "Email", game.Officialid);
        //    //ViewData["Seasonid"] = new SelectList(_context.Season, "Seasonid", "Name", game.Seasonid);
        //    //ViewData["Sportid"] = new SelectList(_context.Sport, "Sportid", "Name", game.Sportid);
        //    //ViewData["Winner"] = new SelectList(_context.Team, "Teamid", "Name", game.Winner);

        //    return View(game);
        //}
    }
}
