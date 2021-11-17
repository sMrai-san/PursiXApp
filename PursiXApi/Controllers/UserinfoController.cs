using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PursiXApi.Models;


namespace PursiXApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserinfoController : ControllerBase
    {
        private readonly PursiDBContext _db;
        public UserinfoController(PursiDBContext db)
        {
            _db = db;
        }

        //*******************************************
        //GETTING USER INFO FOR USER EDIT
        //*******************************************
        [HttpPost]
        [Route("getuserinfo")]

        public List<UserInfo> GetUserInfo(UserInfoEdit input)
        {
            try
            {
                if (input.isLogged == true)
                {
                    var userEditInfo = (from ui in _db.UserInfo
                                        where ui.LoginId == input.LoginId
                                        select ui).ToList();

                    return userEditInfo;
                }
                else
                {
                    throw new ArgumentException("You are not logged in!");
                }
            }
            catch
            {
                throw new ArgumentException("Something went wrong while fetching data from the API");
            }




        }

        //*********************************************************
        //UPDATE USERINFO
        //*********************************************************
        [HttpPut]
        [Route("updateuserinfo")]
        public bool UpdateEmployee(UserInfo input)
        {
            try
            {
                var usrInfoId = (from uid in _db.UserInfo
                                 where uid.LoginId == input.LoginId
                                 select uid.UserInfoId).FirstOrDefault();

                UserInfo updateUserInfo = _db.UserInfo.Find(usrInfoId);
                if (updateUserInfo != null)
                {
                    updateUserInfo.FirstName = input.FirstName;
                    updateUserInfo.LastName = input.LastName;
                    updateUserInfo.Address = input.Address;
                    updateUserInfo.PostalCode = input.PostalCode;
                    updateUserInfo.City = input.City;
                    updateUserInfo.Phone = input.Phone;
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

        //****************************************************************************
        //*********************************************************
        //ADMIN STUFF STARTS HERE
        //*********************************************************
        //****************************************************************************


        //****************************************************************************
        //List all users for Admin
        //****************************************************************************
        [HttpPost]
        [Route("adminlistusers")]

        public List<UserInfo> AdminGetAllEvents(RegistrationModel input)
        {
            var adminlogged = (from ad in _db.Login
                               where (ad.Admin == input.AdminLogged)
                               select ad.Admin).FirstOrDefault();

            if (adminlogged == true)
            {
                try
                {
                    var allUsers = (from ev in _db.UserInfo
                                    select ev).ToList();

                    return allUsers;
                }
                catch (Exception)
                {

                    throw new ArgumentException("Something went wrong while fetching data from the API");
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


        //****************************************************************************
        //List all unconfirmed users for Admin
        //****************************************************************************
        [HttpPost]
        [Route("adminlistunconfirmedaccounts")]

        public List<UserInfo> AdminGetAllUnconfirmedUserAccounts(RegistrationModel input)
        {
            var adminlogged = (from ad in _db.Login
                               where (ad.Admin == input.AdminLogged)
                               select ad.Admin).FirstOrDefault();

            if (adminlogged == true)
            {
                try
                {
                    var allUsers = (from ev in _db.UserInfo
                                    join eg in _db.Login on ev.LoginId equals eg.LoginId
                                    where eg.Confirmed == false
                                    select ev).ToList();

                    return allUsers;
                }
                catch (Exception)
                {
                    throw new ArgumentException("Something went wrong while fetching data from the API");
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
                throw new ArgumentException("Something went wrong while fetching data from the API");
            }
        }




    }


}
