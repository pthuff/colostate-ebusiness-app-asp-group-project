using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LocalSportsLeague.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace LocalSportsLeague.Controllers
{
    public class HomeController : Controller
    {
        private readonly Team112DBContext aContext;

        public HomeController(Team112DBContext context)
        {
            aContext = context;
        }

        //Home page
        public IActionResult Index()
        {
            return View();
        }

        //Team Score Page
        public IActionResult TeamScores(string selectSeason, string searchName)
        {
            //ViewData["SeasonFilter"] = selectSeason; //Don't need this with dropdown
            ViewData["NameFilter"] = searchName;

            //Query games
            var games = from g in aContext.Game
                        .Include(g => g.Ateam)
                        .Include(g => g.Hteam)
                        .Include(g => g.Season)
                        .Include(g => g.WinnerNavigation)
                        .Include(g => g.SportNavigation)
                        select g;

            //Query seasons
            var seasons = from s in aContext.Season.ToList()
                          select s;

            //Instantiate new list and loop to populate with seasons
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var s in seasons)
            {
                list.Add(new SelectListItem { Text = s.Name, Value = s.Name });
            }
            list.Reverse();
            ViewBag.ListOfSeasons = list;

            //Check for applied filters
            if (!String.IsNullOrEmpty(selectSeason))
            {
                games = games.Where(g => g.Season.Name.Contains(selectSeason));
            }
            if (!String.IsNullOrEmpty(searchName))
            {
                games = games.Where(g => g.Hteam.Name.Contains(searchName) || g.Ateam.Name.Contains(searchName));
            }

            //Return the result
            return View (games.OrderBy(g => g.Datetime));
        }

        //Standings Page
        public IActionResult LeagueStandings(string selectSeason)
        {
            //Method variables
            var standings = new List<Standing>();
            var seasons = from s in aContext.Season.ToList() select s;
            var games = from g in aContext.Game.ToList() select g;
            var coaches = from c in aContext.Coach.ToList() select c;
            var teams = from t in aContext.Team.ToList() select t;
            var gamesMap = new Dictionary<int, int>();

            //Instantiate new list and loop to populate with seasons
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var s in seasons)
            {
                list.Add(new SelectListItem { Text = s.Name, Value = s.Name });
            }
            list.Reverse();
            ViewBag.ListOfSeasons = list;

            //Check for applied filters
            if (!String.IsNullOrEmpty(selectSeason))
            {
                games = games.Where(g => g.Season.Name.Contains(selectSeason));
            }
            else
            {
                //No filter has been applied, so return an empty model
                return View(standings);
            }

            // Construct Dictionary for Records
            foreach (var game in games)
            {
                var hteamId = game.Hteamid.GetValueOrDefault();
                var ateamId = game.Ateamid.GetValueOrDefault();

                if (!gamesMap.ContainsKey(hteamId)) { gamesMap[hteamId] = 0; }
                if (!gamesMap.ContainsKey(ateamId)) { gamesMap[ateamId] = 0; }

                if (game.Hscore > game.Ascore)
                {
                    gamesMap[hteamId] += 100;
                    gamesMap[ateamId] += 10;
                }
                else if (game.Ascore > game.Hscore)
                {
                    gamesMap[ateamId] += 100;
                    gamesMap[hteamId] += 10;
                }
                else
                {
                    gamesMap[hteamId] += 1;
                    gamesMap[ateamId] += 1;
                }
            }

            foreach (KeyValuePair<int, int> team in gamesMap)
            {
                var standing = new Standing();
                var theTeam = teams.Where(t => t.Teamid == team.Key).First();
                standing.Team = theTeam.Name;
                var theCoach = coaches.Where(c => c.Coachid == theTeam.Coachid).First();
                standing.Coach = theCoach.Fname + ' ' + theCoach.Lname;
                var recordAsString = team.Value.ToString();
                var wins = int.Parse(recordAsString.Substring(0, 1));
                var losses = int.Parse(recordAsString.Substring(1, 1));
                var ties = int.Parse(recordAsString.Substring(2, 1));
                standing.Record = wins.ToString() + '-' + losses + '-' + ties;
                var winPercentage = ((wins * 1.0) / (wins + losses + ties));
                standing.WinPercentage = winPercentage.ToString("0.###");
                standings.Add(standing);
            }
            standings.Sort((s1, s2) => s1.WinPercentage.CompareTo(s2.WinPercentage));
            standings.Reverse();
            //Return the result
            return View(standings);
        }

        //Login Page
        public IActionResult Login()
        {
            return View();
        }

        ////GET: Add Scores Page
        //public IActionResult AddScores(int seasonID)
        //{
        //    ViewData["seasonID"] = seasonID;

        //    // *** NEED TO REPLACE THE HARD-CODED 21 WITH THE SEASONID VARIABLE *** //

        //    var teams = aContext.SeasonTeam.Include(t => t.Team).Where(t => t.Seasonid == 21).ToList();
            

        //    ViewData["Hteamid"] = new SelectList(teams.OrderBy(h => h.Team.Name), "Teamid", "Team.Name");
        //    ViewData["Ateamid"] = new SelectList(teams.OrderBy(a => a.Team.Name), "Teamid", "Team.Name");
        //    return View();
        //}

        ////POST: Add Scores Page for Editing
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditScore(int id, [Bind("Gameid,Hteamid,Ateamid,Sportid,Seasonid,Officialid,Winner,Hscore,Ascore,Ot,Datetime")] Game game)
        //{
        //    if (id != game.Gameid)
        //    {
        //        return RedirectToAction(nameof(EditScores));
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            aContext.Update(game);
        //            await aContext.SaveChangesAsync();
        //        }
        //        catch
        //        {
        //            TempData["message"] = $"{game.Gameid} update failed";
        //            return RedirectToAction(nameof(Index));
        //        }

        //        TempData["message"] = $"{game.Gameid} has been updated";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewData["Hteamid"] = new SelectList(aContext.Team.OrderBy(h => h.Name), "Hteamid", "Name", game.Hteamid);
        //    ViewData["Ateamid"] = new SelectList(aContext.Team.OrderBy(a => a.Name), "Ateamid", "Name", game.Ateamid);
        //    return View(game);
        //}


        ////Edit Scores Page
        //public IActionResult EditScores()
        //{

        //    // *** NEED TO REPLACE THE HARD-CODED 21 WITH THE SEASONID VARIABLE *** //

        //    var games = aContext.Game.Include(g => g.Season)
        //        .Include(g => g.Hteam)
        //        .Include(g => g.Ateam)
        //        .Where(g => g.Seasonid == 21)
        //        .ToList();

        //    return View(games);
        //}

        //// GET: Delete Scores
        //public async Task<IActionResult> DeleteScores(int? id)
        //{
        //    if (id == null)
        //    {
        //        //if ID doesn't exist, send back to Edit Scores page
        //        return RedirectToAction(nameof(EditScores));
        //    }

        //    var game = await aContext.Game.Include(g => g.Season)
        //        .Include(g => g.Hteam)
        //        .Include(g => g.Ateam)
        //        .FirstOrDefaultAsync(g => g.Gameid == id);

        //    if (game == null)
        //    {
        //        return RedirectToAction(nameof(EditScores));
        //    }

        //    //find game information and display it
        //    return View(game);
        //}

        //// POST: Delete Scores
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var game = await aContext.Game.FindAsync(id);

        //    if (game == null)
        //    {
        //        TempData["message"] = $"Game {id} not found";
        //        return RedirectToAction(nameof(EditScores));
        //    }

        //    try
        //    {
        //        aContext.Game.Remove(game);
        //        await aContext.SaveChangesAsync();
        //    }
        //    catch
        //    {
        //        TempData["message"] = $"{game.Gameid} not deleted";
        //        return RedirectToAction(nameof(EditScores));
        //    }

        //    TempData["message"] = $"{game.Gameid} has been deleted";
        //    return RedirectToAction(nameof(EditScores));
        //}

        //private bool GameExists(int id)
        //{
        //    return aContext.Game.Any(e => e.Gameid == id);
        //}


        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
