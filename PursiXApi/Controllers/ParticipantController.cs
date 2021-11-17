using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PursiXApi.Models;

namespace PursiXApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantController : ControllerBase
    {
        private readonly PursiDBContext _db;
        public ParticipantController(PursiDBContext db)
        {
            _db = db;
        }

        //**********************************************************************************************************
        //LIST ALL PARTICIPATIONS (can be get, nothing usable info there, login id can not be confirmed in any way)
        //**********************************************************************************************************
        [HttpGet]
        [Route("")]

        public List<EventParticipations> GetAllParticipations()
        {
            var allParticipations = (from ep in _db.EventParticipations
                                    select ep).ToList();

            var allParticipants = new List<EventParticipations>();
            foreach (var item in allParticipations)
            {
                allParticipants.Add(new EventParticipations { ParticipationId = item.ParticipationId, EventId = item.EventId, LoginId = item.LoginId, Confirmed = item.Confirmed });
            }

            //return allParticipations;
            return allParticipants;
        }

        //*********************************************************
        //CREATING A NEW PARTICIPATION
        //*********************************************************
        [HttpPost]
        [Route("addparticipation")]
        public bool NewParticipation(EventParticipations input)
        {
            Login userLogged = (from ul in _db.Login
                                where (ul.LoginId == input.LoginId)
                                select ul).FirstOrDefault();

            if (userLogged != null)
            {

                try
                {
                    EventParticipations newPart = new EventParticipations()
                    {
                        EventId = input.EventId,
                        LoginId = input.LoginId,
                        Confirmed = false,
                        AddInfo = input.AddInfo
                    };

                    _db.EventParticipations.Add(newPart);
                    _db.SaveChanges();

                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    _db.Dispose();
                }

                return true;

            }
            else
            {
                return false;
            }
        }


        //**********************************************************************************************************
        //**********************************************************************************************************
        //ADMIN STUFF STARTS HERE
        //**********************************************************************************************************
        //**********************************************************************************************************


        //****************************************************
        //ADMIN LIST ALL UNCONFIRMED PARTICIPATIONS
        //****************************************************
        [HttpPost]
        [Route("admingetpart")]

        public List<EventsAndParticipationsCombinedModel> GetAllParticipantsAdmin(EventsAndParticipationsCombinedModel input)
        {
            var adminlogged = (from ad in _db.Login
                               where (ad.Admin == input.AdminLogged)
                               select ad.Admin).FirstOrDefault();

            if (adminlogged == true)
            {

                var allParticipations = (from ep in _db.EventParticipations
                                         join ev in _db.Events on ep.EventId equals ev.EventId
                                         join ui in _db.UserInfo on ep.LoginId equals ui.LoginId
                                         where ep.Confirmed == false
                                         select new { ep, ev, ui }).ToList();

                var allParticipants = new List<EventsAndParticipationsCombinedModel>();
                foreach (var item in allParticipations)
                {
                    var participantCount = (from ap in _db.EventParticipations
                                            where ap.EventId == item.ep.EventId
                                            select ap.LoginId).Count();

                    allParticipants.Add(new EventsAndParticipationsCombinedModel
                    {
                        ParticipationId = item.ep.ParticipationId,
                        EventId = item.ep.EventId,
                        LoginId = item.ep.LoginId,
                        AddInfo = item.ep.AddInfo,
                        Confirmed = item.ep.Confirmed,
                        Name = item.ev.Name,
                        Description = item.ev.Description,
                        EventDateTime = item.ev.EventDateTime,
                        MaxParticipants = item.ev.MaxParticipants,
                        FirstName = item.ui.FirstName,
                        LastName = item.ui.LastName,
                        Email = item.ui.Email,
                        JoinedParticipants = participantCount
                });
                }

                //return allParticipations;
                return allParticipants;
            }
            else
            {
                return null;
            }
        }


        //****************************************************
        //ADMIN LIST PARTICIPATIONS
        //****************************************************
        [HttpPost]
        [Route("admingetpartlist")]

        public List<EventsAndParticipationsCombinedModel> GetAllParticipantsAdminList(EventsAndParticipationsCombinedModel input)
        {
            var adminlogged = (from ad in _db.Login
                               where (ad.Admin == input.AdminLogged)
                               select ad.Admin).FirstOrDefault();

            if (adminlogged == true)
            {

                var allParticipations = (from ep in _db.EventParticipations
                                         join ev in _db.Events on ep.EventId equals ev.EventId
                                         join ui in _db.UserInfo on ep.LoginId equals ui.LoginId
                                         where ep.Confirmed == true
                                         select new { ep, ev, ui }).ToList();

                var allParticipants = new List<EventsAndParticipationsCombinedModel>();
                foreach (var item in allParticipations)
                {
                    var participantCount = (from ap in _db.EventParticipations
                                            where ap.EventId == item.ep.EventId
                                            select ap.LoginId).Count();

                    allParticipants.Add(new EventsAndParticipationsCombinedModel
                    {
                        ParticipationId = item.ep.ParticipationId,
                        EventId = item.ep.EventId,
                        LoginId = item.ep.LoginId,
                        AddInfo = item.ep.AddInfo,
                        Confirmed = item.ep.Confirmed,
                        Name = item.ev.Name,
                        Description = item.ev.Description,
                        EventDateTime = item.ev.EventDateTime,
                        MaxParticipants = item.ev.MaxParticipants,
                        FirstName = item.ui.FirstName,
                        LastName = item.ui.LastName,
                        Email = item.ui.Email,
                        JoinedParticipants = participantCount
                    });
                }

                //return allParticipations;
                return allParticipants;
            }
            else
            {
                return null;
            }
        }




        //*********************************************************
        //ADMIN CONFIRM PARTICIPATION
        //*********************************************************
        [HttpPut]
        [Route("confirmpart")]
        public bool ConfirmPart(EventParticipationsConfirm input)
        {
            var adminlogged = (from ad in _db.Login
                               where ad.Admin == input.AdminLogged
                               select ad.Admin).FirstOrDefault();

            //only admin can now create new event
            if (adminlogged == true)
            {

                try
                {
                    EventParticipations editPart = _db.EventParticipations.Find(input.ParticipationId);
                    if (editPart != null)
                    {
                        editPart.Confirmed = true;
                        _db.SaveChanges();

                    }
                    else
                    {
                        return false;
                    }


                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    _db.Dispose();
                }

                return true;

            }
            else
            {
                return false;
            }
        }



        //*********************************************************
        //ADMIN EDIT PARTICIPATION
        //*********************************************************
        [HttpPut]
        [Route("editpart")]
        public bool EditPart(EventsAndParticipationsCombinedModel input)
        {
            var adminlogged = (from ad in _db.Login
                               where ad.Admin == input.AdminLogged
                               select ad.Admin).FirstOrDefault();

            //only admin can now create new event
            if (adminlogged == true)
            {

                try
                {
                    EventParticipations editPart = _db.EventParticipations.Find(input.ParticipationId);
                    if (editPart != null)
                    {
                        editPart.Confirmed = input.Confirmed;
                        editPart.AddInfo = input.AddInfo;
                        _db.SaveChanges();

                    }
                    else
                    {
                        return false;
                    }


                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    _db.Dispose();
                }

                return true;

            }
            else
            {
                return false;
            }
        }

        //*********************************************************
        //ADMIN DELETE PARTICIPATION
        //*********************************************************
        [HttpPost]
        [Route("deletepart")]
        public bool DeletePart(EventParticipationsConfirm input)
        {
            var adminlogged = (from ad in _db.Login
                               where ad.Admin == input.AdminLogged
                               select ad.Admin).FirstOrDefault();

            //only admin can now create new event
            if (adminlogged == true)
            {

                try
                {
                    var deleteParticipationId = (from de in _db.EventParticipations
                                         where de.ParticipationId == input.ParticipationId
                                         select de.ParticipationId).FirstOrDefault();

                    if (deleteParticipationId != 0)
                    {
                        EventParticipations partToDelete = _db.EventParticipations.Find(deleteParticipationId);
                        if (partToDelete != null)
                        {
                            _db.EventParticipations.Remove(partToDelete);
                            _db.SaveChanges();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    _db.Dispose();
                }



            }
            else
            {
                return false;
            }
        }




        //****************************************************
        //ADMIN LIST UNCONFIRMED PARTICIPATIONS COUNT
        //****************************************************
        [HttpGet]
        [Route("unconfirmed")]

        public int GetUnconfirmedCount()
        {
            var participationCount = (from pc in _db.EventParticipations
                                      where pc.Confirmed == false
                                     select pc).Count();

            //return count;
            return participationCount;
        }






    }
}
