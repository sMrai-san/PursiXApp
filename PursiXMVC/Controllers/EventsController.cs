using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PursiXMVC.Data;
using PursiXMVC.Models;

namespace PursiXMVC.Controllers
{
    public class EventsController : Controller
    {
        private readonly PursiDBContext _context;

        public EventsController(PursiDBContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index(string searchEvent, bool showPast)
        {
            //USER AUTHORIZATION
            if (HttpContext.Session.GetString("username") != null)
            {
                EventListModel events = new EventListModel();

                if (showPast == true)
                {
                    //admin can see all the events
                    ViewBag.showPasat = true;
                    events.EventsList = await _context.Events.OrderBy(x => x.EventDateTime).ToListAsync();
                }
                else
                {
                    //we do not need to see all the events before this date or yesterday, and we need to sort it from date -> ascending
                    ViewBag.showPast = false;
                    events.EventsList = await _context.Events.Where(x => x.EventDateTime > DateTime.Now.AddDays(-1)).OrderBy(x => x.EventDateTime).ToListAsync();
                }
                //fetching participant data from database
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");
                Task<string> json = client.GetStringAsync("/api/participant");
                var eventsData = JsonConvert.DeserializeObject<List<EventParticipations>>(await json);

                events.ParticipationList = eventsData;
                if (TempData["success"] != null)
                {
                    ViewBag.success = TempData["success"].ToString();
                }
                else { }
                if (TempData["error"] != null)
                {
                    ViewBag.error = TempData["error"].ToString();
                }
                else { }

                
                //Let's use infinite scroll in the further versions of this

                //Some search goodness
                if (!String.IsNullOrEmpty(searchEvent))
                {
                    events.EventsList = events.EventsList.Where(s => s.Name.ToLower().Contains(searchEvent.ToLower()));
                    ViewBag.searchString = searchEvent;
                }

                ViewBag.eventCount = events.EventsList.Count();

                return View(events);
            }
            else
            {
                TempData["error"] = "Et ole kirjautuneena.";
                return RedirectToAction("Index", "Account");
            }
        }

        //********************************************************************
        //CREATE
        //********************************************************************

        public IActionResult Create()
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventDateTime,Name,Description,MaxParticipants,Url,AdditionalDetails,Latitude,Longitude")] Events events)
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                if (ModelState.IsValid)
                {
                    _context.Add(events);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Tapahtuma luotu onnistuneesti";
                    return RedirectToAction(nameof(Index));
                }
                return View(events);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        //********************************************************************
        //EDIT
        //********************************************************************

        //GET
        public async Task<IActionResult> Edit(int? id)
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                if (id == null)
                {
                    return NotFound();
                }

                var events = await _context.Events.FindAsync(id);
                if (events == null)
                {
                    return NotFound();
                }
                return View(events);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventDateTime,Name,Description,MaxParticipants,Url,AdditionalDetails,Latitude,Longitude")] Events events)
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                if (id != events.EventId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        TempData["success"] = "Tapahtumaa muokattu onnistuneesti";
                        _context.Update(events);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EventsExists(events.EventId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(events);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        //********************************************************************
        //DELETE
        //********************************************************************

        //GET
        public async Task<IActionResult> Delete(int? id)
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                if (id == null)
                {
                    return NotFound();
                }

                var events = await _context.Events
                    .FirstOrDefaultAsync(m => m.EventId == id);
                if (events == null)
                {
                    return NotFound();
                }

                return View(events);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                var events = await _context.Events.FindAsync(id);

                var eventpartid = (from ep in _context.EventParticipations
                                   where ep.EventId == id
                                   select ep).ToList();

                //we have to delete every participation from the event first!
                foreach (var item in eventpartid)
                {
                    var eventpart = await _context.EventParticipations.FindAsync(item.ParticipationId);
                    _context.EventParticipations.Remove(eventpart);
                }

                _context.Events.Remove(events);
                await _context.SaveChangesAsync();
                TempData["success"] = "Tapahtuma " + events.Name.ToUpper() + " poistettu onnistuneesti";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        private bool EventsExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
