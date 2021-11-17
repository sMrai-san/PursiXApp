using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PursiXApi.Models;

namespace PursiXApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly PursiDBContext _db;
        public LoginController(PursiDBContext db)
        {
            _db = db;
        }

        //****************************************************
        //Get Id via Login name
        //****************************************************
        [HttpPost]
        [Route("getuserid")]

        public int GetMyId(Login input)
        {
            var loginId = (from lo in _db.Login
                           where lo.Email == input.UserName
                           select lo.LoginId).FirstOrDefault();

            return loginId;
        }

        //****************************************************
        //LOGIN
        //****************************************************
        [HttpPost]
        //using Login model to get the info forward, might have to make a custom model, but let's try...
        public bool GetLogin(Login input)
        {
            try
            {
                    var user = (from em in _db.Login
                                where (em.Email == input.Email) &&
                                (em.PassWord == input.PassWord)
                                select em.Email).FirstOrDefault();

                    //If user does not exist
                    if (user == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    } 
            }
            //Error handling
            catch
            {
                return false;
            }
        }

        //*********************************************************
        //CHECK EMAIL ACTIVATION STATUS
        //*********************************************************
        [HttpPost]
        [Route("checkactivation")]
        public bool ActivationCheck(Login input)
        {
            if (input.UserName == "admin")
            {
                return true;
            }
            else
            {
                try
                {
                    var isItConfirmed = (from em in _db.Login
                                         where em.Email == input.Email
                                         select em.EmailConfirmed).FirstOrDefault();

                    if (isItConfirmed == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }

        }

        //*********************************************************
        //CHECK ACCOUNT CONFIRMATION STATUS
        //*********************************************************
        [HttpPost]
        [Route("checkconfirmation")]
        public bool AccountConfirmationCheck(Login input)
        {
            try
            {
                var isConfirmed = (from em in _db.Login
                                  where em.Email == input.Email && em.PassWord == input.PassWord
                                  select em.Confirmed).FirstOrDefault();

                if (isConfirmed == true)
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }

        //*********************************************************
        //ACTIVATE EMAIL
        //*********************************************************
        [HttpPut]
        [Route("activateaccount")]
        public bool ActivateAccount(Login input)
        {
            try
            {
                var verificationCode = (from em in _db.Login
                                        where em.Email == input.Email
                                        select em.VerificationCode).FirstOrDefault();

                if (verificationCode == input.VerificationCode)
                {
                    var loginId = (from em in _db.Login
                                   where em.Email == input.Email
                                   select em.LoginId).FirstOrDefault();
                    try
                    {
                        Login activateLogin = _db.Login.Find(loginId);
                        if (activateLogin != null)
                        {
                            activateLogin.EmailConfirmed = true;
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
            catch
            {
                return false;
            }

        }


        //*************************************************************************************
        //*************************************************************************************
        //REGISTRATION PROCESS
        //*************************************************************************************
        //*************************************************************************************


        //*********************************************************
        //CHECK EMAIL DUPLICATE
        //*********************************************************
        [HttpPost]
        [Route("checkemail")]
        public bool EmailDublicateCheck(RegistrationModel input)
        {
            try
            {
                var emailInUse = (from em in _db.Login
                                  where em.Email == input.Email
                                  select em.Email).FirstOrDefault();

                if (emailInUse == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }


        //*********************************************************
        //REGISTRATION
        //*********************************************************
        [HttpPost]
        [Route("register")]
        public bool Register(RegistrationModel input)
        {

            //last id in the list for automatic username
            int automaticUserName = (from ad in _db.Login
                                   orderby ad.LoginId descending
                                   select ad.LoginId).FirstOrDefault();

            try
            {
                if (String.IsNullOrEmpty(input.Address) &&
                    String.IsNullOrEmpty(input.City) &&
                    String.IsNullOrEmpty(input.Email) &&
                    String.IsNullOrEmpty(input.FirstName) &&
                    String.IsNullOrEmpty(input.LastName) &&
                    String.IsNullOrEmpty(input.PassWord) &&
                    String.IsNullOrEmpty(input.Phone) &&
                    String.IsNullOrEmpty(input.PostalCode)
                    )
                {
                    return false;
                }
                else
                {
                    Login newUserLogin = new Login()
                    {
                        UserName = "pursiuser" + (automaticUserName + 1).ToString(),
                        PassWord = input.PassWord,
                        Email = input.Email,
                        EmailConfirmed = false,
                        VerificationCode = input.VerificationCode,
                        Confirmed = false,
                        Admin = false

                    };
                    _db.Login.Add(newUserLogin);
                    _db.SaveChanges();

                    //then we get the newly made LoginId from login -table to UserInfo table
                    int getUserId = (from ui in _db.Login
                                     where ui.Email == input.Email
                                     select ui.LoginId).FirstOrDefault();

                    UserInfo newUserInfo = new UserInfo()
                    {
                        FirstName = input.FirstName,
                        LastName = input.LastName,
                        Address = input.Address,
                        PostalCode = input.PostalCode,
                        City = input.City,
                        Phone = input.Phone,
                        Email = input.Email,
                        LoginId = getUserId
                    };


                    _db.UserInfo.Add(newUserInfo);
                    _db.SaveChanges();

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


        //*********************************************************
        //USER PASSWORD CHANGE
        //*********************************************************
        [HttpPut]
        [Route("pwdchange")]
        public bool ChangePwd(EditLoginModel input)
        {
            try
            {
                var oldPwdCheck = (from em in _db.Login
                              where em.PassWord == input.PassWord &&
                              em.LoginId == input.LoginId
                              select em.LoginId).FirstOrDefault();

                if (oldPwdCheck == input.LoginId)
                {
                    try
                    {
                        Login changePwd = _db.Login.Find(input.LoginId);
                        if (changePwd != null)
                        {
                            changePwd.PassWord = input.NewPassWord;
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
            catch
            {
                return false;
            }

        }


        //***************************************************************************************
        //***************************************************************************************
        //ADMIN STUFF STARTS HERE
        //***************************************************************************************
        //***************************************************************************************


        //****************************************************
        //ADMIN LOGIN
        //****************************************************
        [HttpPost]
        [Route("adminlogin")]
        public bool GetAdminLogin(Login input)
        {
            try
            {
                var admin = (from em in _db.Login
                             where (em.Email == input.Email) &&
                             (em.PassWord == input.PassWord) &&
                             (em.Admin == true)
                             select em.UserName).FirstOrDefault();
                //if not admin
                if (admin == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            //Error handling
            catch
            {
                return false;
            }

        }



        //****************************************************
        //ADMIN UNCONFIRMED COUNT
        //****************************************************
        [HttpGet]
        [Route("unconfirmedusers")]

        public int GetUnconfirmedCount()
        {
            var unconfirmedCount = (from pc in _db.Login
                                      where pc.Confirmed == false
                                      select pc).Count();

            //return count;
            return unconfirmedCount;
        }



        //*********************************************************
        //ADMIN ACTIVATE USER ACCOUNT
        //*********************************************************
        [HttpPut]
        [Route("activateuseraccount")]
        public bool ActivateUserAccount(EditLoginModel input)
        {
            var adminlogged = (from ad in _db.Login
                               where ad.Admin == input.AdminLogged
                               select ad.Admin).FirstOrDefault();

            //activating user's account as administrator
            if (adminlogged == true)
            {
                try
                {
                    var activateId = (from em in _db.Login
                                      where em.LoginId == input.LoginId
                                      select em.LoginId).FirstOrDefault();

                    if (activateId == input.LoginId)
                    {
                        try
                        {
                            Login activateThis = _db.Login.Find(input.LoginId);
                            if (activateThis != null)
                            {
                                activateThis.Confirmed = true;
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
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }


        //****************************************************
        //CHECKING IF USER IS ADMIN OR NOT
        //****************************************************
        [HttpPost]
        [Route("adminisadmin")]
        public bool GetIsUserAdmin(RegistrationModel input)
        {
            var adminlogged = (from ad in _db.Login
                               where ad.Admin == input.AdminLogged
                               select ad.Admin).FirstOrDefault();

            //only admin can now create new event
            if (adminlogged == true)
            {
                try
                {
                    var admin = (from em in _db.Login
                                 where (em.Email == input.Email) &&
                                 (em.Admin == true)
                                 select em.Admin).FirstOrDefault();
                    //if not admin
                    if (admin == false)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                //Error handling
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        //******************************************************************************************
        //ADMIN ADD USER
        //******************************************************************************************
        [HttpPost]
        [Route("adminadduser")]
        public bool AddUser(RegistrationModel input)
        {
            var adminlogged = (from ad in _db.Login
                               where ad.Admin == input.AdminLogged
                               select ad.Admin).FirstOrDefault();

            //only admin can now create new event
            if (adminlogged == true)
            {
                //last id in the list for automatic username
                int automaticUserName = (from ad in _db.Login
                                         orderby ad.LoginId descending
                                         select ad.LoginId).FirstOrDefault();

                try
                {

                    Login newUserLogin = new Login()
                    {
                        UserName = "pursiuser" + (automaticUserName + 1).ToString(),
                        PassWord = input.PassWord,
                        Email = input.Email,
                        EmailConfirmed = true,
                        VerificationCode = 1234,
                        Confirmed = true,
                        Admin = input.Admin

                    };
                    _db.Login.Add(newUserLogin);
                    _db.SaveChanges();

                    //then we get the newly made LoginId from login -table to UserInfo table
                    int getUserId = (from ui in _db.Login
                                     where ui.Email == input.Email
                                     select ui.LoginId).FirstOrDefault();

                    UserInfo newUserInfo = new UserInfo()
                    {
                        FirstName = input.FirstName,
                        LastName = input.LastName,
                        Address = input.Address,
                        PostalCode = input.PostalCode,
                        City = input.City,
                        Phone = input.Phone,
                        Email = input.Email,
                        LoginId = getUserId
                    };


                    _db.UserInfo.Add(newUserInfo);
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


        //******************************************************************************************
        //ADMIN EDIT USER
        //******************************************************************************************
        [HttpPut]
        [Route("adminedituser")]
        public bool EditUser(RegistrationModel input)
        {
            var adminlogged = (from ad in _db.Login
                               where ad.Admin == input.AdminLogged
                               select ad.Admin).FirstOrDefault();

            //only admin can now create new event
            if (adminlogged == true)
            {

                try
                {
                    var updateLoginId = (from lo in _db.Login
                                        where lo.LoginId == input.LoginId
                                        select lo.LoginId).FirstOrDefault();

                    Login updateLogin = _db.Login.Find(updateLoginId);
                    if (updateLogin != null)
                    {
                        if (String.IsNullOrEmpty(input.PassWord))
                        {

                        }else
                        {
                            updateLogin.PassWord = input.PassWord;
                        }
                        updateLogin.Email = input.Email;
                        updateLogin.Admin = input.Admin;
                        _db.SaveChanges();
                         
                    }
                    else
                    {
                        return false;
                    }

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
                        updateUserInfo.Email = input.Email;
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
        //ADMIN DELETE USER
        //*********************************************************
        [HttpPost]
        [Route("admindeleteuser")]
        public bool DeleteUser(RegistrationModel input)
        {
            var adminlogged = (from ad in _db.Login
                               where ad.Admin == input.AdminLogged
                               select ad.Admin).FirstOrDefault();

            //only admin can now create new event
            if (adminlogged == true)
            {

                try
                {

                    var deleteUserInfo = (from dei in _db.UserInfo
                                          where dei.LoginId == input.LoginId
                                          select dei.UserInfoId).FirstOrDefault();

                    var deleteUserId = (from de in _db.Login
                                        where de.LoginId == input.LoginId
                                        select de.LoginId).FirstOrDefault();

                    var deleteUserPart = (from dep in _db.EventParticipations
                                          where dep.LoginId == input.LoginId
                                          select dep.LoginId).FirstOrDefault();


                    if (deleteUserInfo != 0)
                    {
                        UserInfo userInfoToDelete = _db.UserInfo.Find(deleteUserInfo);
                        if (userInfoToDelete != null)
                        {
                            _db.UserInfo.Remove(userInfoToDelete);
                            _db.SaveChanges();
                        }
                        else
                        {
                            return false;
                        } 
                    }

                    if (deleteUserPart != 0)
                    {
                        EventParticipations userPartToDelete = _db.EventParticipations.Find(deleteUserPart);
                        if (userPartToDelete != null)
                        {
                            _db.EventParticipations.Remove(userPartToDelete);
                            _db.SaveChanges();
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (deleteUserId != 0)
                    {
                        Login userToDelete = _db.Login.Find(deleteUserId);
                        if (userToDelete != null)
                        {
                            _db.Login.Remove(userToDelete);
                            _db.SaveChanges();

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

                    return true;
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
