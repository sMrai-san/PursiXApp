using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
    public class EventParticipationsController : Controller
    {
        private readonly PursiDBContext _context;

        public EventParticipationsController(PursiDBContext context)
        {
            _context = context;
        }

        //********************************************************************
        //LIST PARTICIPATIONS
        //********************************************************************

        public async Task<IActionResult> Index(bool showConfirmed, string sortOrder)
        {            
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                //showconfirmed -button call
                if (showConfirmed == true)
                {
                    var pursiDBContext = _context.EventParticipations.Where(e => e.Confirmed == true).Include(e => e.Event).Include(e => e.Login);
                    ViewBag.showconfirmed = true;
                    return View(await pursiDBContext.ToListAsync());
                }
                else
                {
                    var pursiDBContext = _context.EventParticipations.Where(e => e.Confirmed == false).Include(e => e.Event).Include(e => e.Login);

                    //sorting results for non-confirmed only (simple table header links)
                    ViewBag.EmailSort = String.IsNullOrEmpty(sortOrder) ? "email" : "email";
                    ViewBag.EventNameSort = String.IsNullOrEmpty(sortOrder) ? "event" : "event";
                    switch (sortOrder)
                    {
                        case "email":
                            ViewBag.emailpressed = true;
                            ViewBag.eventpressed = false;
                            pursiDBContext = _context.EventParticipations.OrderBy(e => e.Login.Email).Where(e => e.Confirmed == false).Include(e => e.Event).Include(e => e.Login);
                            break;
                        case "event":
                            ViewBag.eventpressed = true;
                            ViewBag.emailpressed = false;
                            pursiDBContext = _context.EventParticipations.OrderBy(e => e.Event.Name).Where(e => e.Confirmed == false).Include(e => e.Event).Include(e => e.Login);
                            break;
                        default:
                            pursiDBContext = _context.EventParticipations.Where(e => e.Confirmed == false).Include(e => e.Event).Include(e => e.Login);
                            break;
                    }

                    ViewBag.showconfirmed = false;
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

                    return View(await pursiDBContext.ToListAsync());
                }

            }
            else
            {
                TempData["error"] = "Et ole kirjautuneena.";
                return RedirectToAction("Index", "Account");
            }

        }

        //********************************************************************
        //CREATE PARTICIPATION
        //********************************************************************

        [HttpPost]
        public async Task<IActionResult> Create(int eventid, int loginid, string addinfo)
        {
            //USER AUTHORIZATION
            if (HttpContext.Session.GetString("username") != null)
            {
                try
                {
                    EventParticipations addPart = new EventParticipations()
                    {
                        LoginId = loginid,
                        EventId = eventid,
                        Confirmed = false,
                        AddInfo = addinfo
                    };

                    _context.Add(addPart);
                    await _context.SaveChangesAsync();

                    TempData["success"] = "Osallistumispyyntö tapahtumaan lisätty!";
                    return RedirectToAction("Index", "Events");
                }
                catch
                {
                    TempData["error"] = "Tapahtuman osallistumispyynnön käsittelyssä tapahtui virhe, ole hyvä ja yritä uudelleen.";
                    return RedirectToAction("Index", "Events");
                }
            }
            else
            {
                TempData["error"] = "Et ole kirjautuneena.";
                return RedirectToAction("Index", "Account");
            }


        }

        //********************************************************************
        //ADMIN EDIT PARTICIPATION
        //********************************************************************

        public async Task<IActionResult> Edit(int? id)
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                if (id == null)
                {
                    return NotFound();
                }

                var eventParticipations = await _context.EventParticipations.FindAsync(id);
                if (eventParticipations == null)
                {
                    return NotFound();
                }
                ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", eventParticipations.EventId);
                ViewBag.eventName = (from em in _context.Events
                                     where (em.EventId == eventParticipations.EventId)
                                     select em.Name).FirstOrDefault();
                var participantFirstName = (from em in _context.UserInfo
                                            where (em.LoginId == eventParticipations.LoginId)
                                            select em.FirstName).FirstOrDefault();
                var participantLastName = (from em in _context.UserInfo
                                           where (em.LoginId == eventParticipations.LoginId)
                                           select em.LastName).FirstOrDefault();
                ViewBag.participant = participantFirstName + " " + participantLastName;
                ViewData["LoginId"] = new SelectList(_context.Login, "LoginId", "LoginId", eventParticipations.LoginId);
                return View(eventParticipations);
            }
            else
            {
                TempData["error"] = "Et ole kirjautuneena.";
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ParticipationId,EventId,LoginId,,Confirmed,AddInfo")] EventParticipations eventParticipations)
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                if (id != eventParticipations.ParticipationId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(eventParticipations);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EventParticipationsExists(eventParticipations.ParticipationId))
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
                ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", eventParticipations.EventId);
                ViewData["LoginId"] = new SelectList(_context.Login, "LoginId", "LoginId", eventParticipations.LoginId);
                return View(eventParticipations);
            }
            else
            {
                TempData["error"] = "Et ole kirjautuneena.";
                return RedirectToAction("Index", "Account");
            }
        }

        //********************************************************************
        //DELETE PARTICIPATION
        //********************************************************************

        public async Task<IActionResult> Delete(int eventid, int loginid)
        {
            //USER AUTHORIZATION
            if (HttpContext.Session.GetString("username") != null)
            {
                if (eventid == 0)
                {
                    TempData["error"] = "Poistettaessa tapahtumaan osallistumista tapahtui virhe.";
                    return NotFound();
                }

                var participationId = (from pi in _context.EventParticipations
                                       where pi.EventId == eventid && pi.LoginId == loginid
                                       select pi.ParticipationId).FirstOrDefault();


                var eventParticipations = await _context.EventParticipations.FindAsync(participationId);
                _context.EventParticipations.Remove(eventParticipations);
                await _context.SaveChangesAsync();

                TempData["success"] = "Osallistumispyyntö poistettu tapahtumasta onnistuneesti!";
                return RedirectToAction("Index", "Events");
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public async Task<IActionResult> AdminDelete(int eventid, int loginid)
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                if (eventid == 0)
                {
                    TempData["error"] = "Osallistumispyyntöä poistettaessa tapahtui virhe.";
                    return NotFound();
                }

                var participationId = (from pi in _context.EventParticipations
                                       where pi.EventId == eventid && pi.LoginId == loginid
                                       select pi.ParticipationId).FirstOrDefault();


                var eventParticipations = await _context.EventParticipations.FindAsync(participationId);
                _context.EventParticipations.Remove(eventParticipations);
                await _context.SaveChangesAsync();

                TempData["success"] = "Osallistumispyyntö poistettu tapahtumasta onnistuneesti!";
                return RedirectToAction("Index", "EventParticipations");
            }
            else
            {
                TempData["error"] = "Et ole kirjautuneena.";
                return RedirectToAction("Index", "Account");
            }
        }

        private bool EventParticipationsExists(int id)
        {
            return _context.EventParticipations.Any(e => e.ParticipationId == id);
        }

        //Passing participationId from the Index-View Hyväksy -button
        public async Task<IActionResult> ConfirmParticipant(int participationId, int eventId)
        {
            EventParticipationsModel confirmPart = new EventParticipationsModel
            {
                ParticipationId = participationId,
                AdminLogged = true

            };

            //let us check if the event is full or not! Because it's possible to join into a event BEFORE admin has yet to confirm all the participants
            var confirmedPart = (from ep in _context.EventParticipations
                                 where ep.EventId == eventId && ep.Confirmed == true
                                 select ep.Confirmed).Count();

            var maxPart = (from ep in _context.EventParticipations
                          join ev in _context.Events on ep.EventId equals ev.EventId
                          where ev.EventId == ep.EventId && ep.ParticipationId == participationId
                           select ep.Event.MaxParticipants).FirstOrDefault();

            //if the event is full we throw error
            if (confirmedPart == Convert.ToInt32(maxPart) )
            {
                TempData["error"] = "Et voi lisätä käyttäjää tapahtumaan, koska tämä tapahtuma on jo täynnä!";
                return RedirectToAction("Index", "EventParticipations");
            }
            else
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");
                string input = JsonConvert.SerializeObject(confirmPart);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PutAsync("/api/participant/confirmpart", content);
                string reply = await message.Content.ReadAsStringAsync();
                bool success = JsonConvert.DeserializeObject<bool>(reply);

                if (success)
                {
                    TempData["success"] = "Osallistumispyyntö hyväksytty!";
                    return RedirectToAction("Index", "EventParticipations");
                }
                else
                {
                    TempData["error"] = "Osallistumispyynnön hyväksynnässä tapahtui virhe. Yritä uudelleen.";
                    return RedirectToAction("Index", "EventParticipations");
                }
            }
        }

    }
}
