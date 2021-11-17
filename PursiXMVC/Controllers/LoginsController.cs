using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PursiXMVC.Data;
using PursiXMVC.Models;
using PursiXMVC.Models.User;

namespace PursiXMVC.Controllers
{
    public class LoginsController : Controller
    {

        private readonly PursiDBContext _context;

        public LoginsController(PursiDBContext context)
        {
            _context = context;
        }

        //*************************************************************************
        //ADMIN LIST LOGINS
        //*************************************************************************

        public async Task<IActionResult> Index()
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
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

                ViewBag.userCount = _context.Login.Count();


                return View(await _context.Login.ToListAsync());
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        //*************************************************************************
        //ADMIN CREATE LOGIN
        //*************************************************************************


        public IActionResult Create(RegisterViewModel reg)
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
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
                return View(reg);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        //*************************************************************************
        //ADMIN EDIT LOGIN
        //*************************************************************************

        public async Task<IActionResult> Edit(int? id)
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = (from u in _context.UserInfo
                            where u.LoginId == id
                            select u).FirstOrDefault();

                var login = await _context.Login.FindAsync(id);
                if (login == null)
                {
                    return NotFound();
                }
                return View(login);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoginId,UserName,PassWord,Email,EmailConfirmed,VerificationCode,Admin,Confirmed")] Login login)
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                if (id != login.LoginId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(login);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!LoginExists(login.LoginId))
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
                return View(login);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult AdminEditUserInfo(int? id)
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                if (id == null)
                {
                    return NotFound();
                }

                var user = (from u in _context.UserInfo
                            where u.LoginId == id
                            select u).FirstOrDefault();
                
                if (user == null)
                {
                    return NotFound();
                }
                return View(user);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminEditUserInfo([Bind("LoginId,FirstName,LastName,Address,PostalCode,City,Phone")] UserInfo user)
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                EditUserInfoModel updateUser = new EditUserInfoModel()
                {
                    LoginId = user.LoginId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Address = user.Address,
                    PostalCode = user.PostalCode,
                    City = user.City.ToUpper(),
                    Phone = user.Phone.ToString(),
                    isLogged = true
                };

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("yourapiipaddress");
                string input = JsonConvert.SerializeObject(updateUser);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PutAsync("/api/userinfo/updateuserinfo", content);
                string reply = await message.Content.ReadAsStringAsync();
                bool success = JsonConvert.DeserializeObject<bool>(reply);

                if (success)
                {
                    TempData["success"] = "Yhteystiedot tallennettu onnistuneesti";
                    return RedirectToAction("Index", "Logins");
                }
                else
                {
                    TempData["error"] = "Yhteystietojen tallennuksessa tapahtui virhe, ole hyvä ja yritä uudelleen";
                    return RedirectToAction("Index", "Logins");
                }
            }
            else
            {
                TempData["error"] = "Et ole kirjautuneena sisään.";
                return RedirectToAction("Index", "Account");
            }
        }

        //*************************************************************************
        //ADMIN DELETE LOGIN
        //*************************************************************************

        public async Task<IActionResult> Delete(int? id)
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                if (id == null)
                {
                    return NotFound();
                }

                var login = await _context.Login
                    .FirstOrDefaultAsync(m => m.LoginId == id);
                if (login == null)
                {
                    return NotFound();
                }

                return View(login);
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        // POST: Logins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //ADMIN AUTHORIZATION
            if (HttpContext.Session.GetString("isAdmin") == "true")
            {
                var login = await _context.Login.FindAsync(id);

                var loginEventPart = (from ev in _context.EventParticipations
                                      where ev.LoginId == id
                                      select ev).FirstOrDefault();

                var loginUserInfo = (from ui in _context.UserInfo
                                     where ui.LoginId == id
                                     select ui).FirstOrDefault();
                if (loginEventPart != null)
                {
                    _context.EventParticipations.Remove(loginEventPart);
                }
                if (loginUserInfo != null)
                {
                    _context.UserInfo.Remove(loginUserInfo);
                }
                _context.Login.Remove(login);

                await _context.SaveChangesAsync();

                TempData["success"] = "Käyttäjä " + login.Email.ToUpper() + " poistettu onnistuneesti!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
        }

        private bool LoginExists(int id)
        {
            return _context.Login.Any(e => e.LoginId == id);
        }
    }
}
