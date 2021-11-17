using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PursiXApi.Models;

namespace PursiXApi.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly PursiDBContext _db;
        public EventsController(PursiDBContext db)
        {
            _db = db;
        }


        //****************************************************************************
        //List all events from the database (this is public information)
        //****************************************************************************
        [HttpGet]
        [Route("")]

        public List<Events> GetAllEvents()
        {
            var allEvents = (from ev in _db.Events
                             where ev.EventDateTime >= DateTime.Now.AddDays(-1) //Think it would be nice to see yesterday's event in the listing too, maybe there is possibility that the event takes place like 3 days in a row, but that is another story
                             select ev).ToList();

            return allEvents;
        }

        //****************************************************************************
        //ADMIN LIST ALL EVENTS
        //****************************************************************************
        [HttpPost]
        [Route("adminallevents")]

        public List<Events> AdminGetAllEvents(AddEventModel input)
        {
            var adminlogged = (from ad in _db.Login
                                 where (ad.Admin == input.AdminLogged)
                                 select ad.Admin).FirstOrDefault();

            if (adminlogged == true)
            {
                    try
                    {
                        var allEvents = (from ev in _db.Events
                                         select ev).ToList();

                        return allEvents;
                    }
                    catch (Exception)
                    {
                    throw new ArgumentException("Something went wrong while fetching data from the API!");
                }
                    finally
                    {
                        _db.Dispose();
                    }
            }
            else if (adminlogged == false)
            {
                throw new ArgumentException("You are not logged in as an Administrator!");
            }
            else
            {
                throw new ArgumentException("Something went wrong while fetching data from the API!");
            }
        }


        //*********************************************************
        //CREATING A NEW EVENT
        //*********************************************************
        [HttpPost]
        [Route("addnewevent")]
        public bool NewTask(AddEventModel input)
        {
            var adminlogged =    (from ad in _db.Login
                                 where ad.Admin == input.AdminLogged
                                 select ad.Admin).FirstOrDefault();

            //only admin can now create new event
            if (adminlogged == true)
            {

                try
                {
                    Events newEvent = new Events()
                    {
                        EventDateTime = input.EventDateTime,
                        Name = input.Name,
                        Description = input.Description,
                        MaxParticipants = input.MaxParticipants,
                        Url = input.Url,
                        AdditionalDetails = input.AdditionalDetails,
                        Latitude = input.Latitude,
                        Longitude = input.Longitude

                    };

                    _db.Events.Add(newEvent);
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


        //*********************************************************
        //EDIT EVENT
        //*********************************************************
        [HttpPut]
        [Route("editevent")]
        public bool EditTask(AddEventModel input)
        {
            var adminlogged = (from ad in _db.Login
                               where ad.Admin == input.AdminLogged
                               select ad.Admin).FirstOrDefault();

            //only admin can now create new event
            if (adminlogged == true)
            {

                try
                {
                    Events editEvent = _db.Events.Find(input.EventId);
                    if (editEvent != null)
                    {
                        editEvent.EventDateTime = input.EventDateTime;
                        editEvent.Name = input.Name;
                        editEvent.Description = input.Description;
                        editEvent.MaxParticipants = input.MaxParticipants;
                        editEvent.Url = input.Url;
                        editEvent.AdditionalDetails = input.AdditionalDetails;
                        editEvent.Latitude = input.Latitude;
                        editEvent.Longitude = input.Longitude;


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
        //DELETE EVENT
        //*********************************************************
        [HttpPost]
        [Route("deleteevent")]
        public bool DeleteEvent(AddEventModel input)
        {
            var adminlogged = (from ad in _db.Login
                               where ad.Admin == input.AdminLogged
                               select ad.Admin).FirstOrDefault();

            //only admin can now create new event
            if (adminlogged == true)
            {

                try
                {
                    var deleteEventId = (from de in _db.Events
                                         where de.EventId == input.EventId
                                         select de.EventId).FirstOrDefault();

                    if (deleteEventId != 0)
                    {
                        Events eventToDelete = _db.Events.Find(deleteEventId);
                        if (eventToDelete != null)
                        {
                            _db.Events.Remove(eventToDelete);
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



    }
}
