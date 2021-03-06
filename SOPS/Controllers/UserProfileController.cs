﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Cookies;
using SOPS.ModelHelpers;
using SOPS.Models;

namespace SOPS.Controllers
{
    [RoutePrefix("api/User")]
    public class UserProfileController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ApplicationUserManager UserManager => Request.GetOwinContext().GetUserManager<ApplicationUserManager>();

        // GET: api/User
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("")]
        public IEnumerable<UserProfileViewModel> GetUserProfiles()
        {
            var users = db.Users.ToList();
            var profiles = new List<UserProfileViewModel>();
            foreach(var user in users)
            {
                var asEmployee = db.Employees.Find(user.Id);
                Company employeeCompany = null;
                if (asEmployee != null)
                {
                    db.Entry(asEmployee).Reference(e => e.Company).Load();
                    employeeCompany = asEmployee.Company;
                }

                profiles.Add(new UserProfileViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Name = user.Name,
                    Surname = user.Surname,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    WatchedProducts = user.WatchedProducts,
                    IsEmployee = asEmployee != null,
                    Company = employeeCompany,
                });
            }
            return profiles;
        }

        /// <summary>
        /// get current user profile
        /// </summary>
        /// <returns></returns>
        [Route("Current")]
        [ResponseType(typeof(UserProfileViewModel))]
        public IHttpActionResult GetCurrentProfile()
        {
            var currentUser = UserHelper.GetCurrentUser();
            if (currentUser == null)
            {
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            var asEmployee = db.Employees.Find(UserHelper.GetCurrentUserId());
            Company employeeCompany = null;
            if (asEmployee != null)
            {
                employeeCompany = asEmployee.Company;
            }

            return Ok(new UserProfileViewModel()
            {
                Id = currentUser.Id,
                UserName = currentUser.UserName,
                Name = currentUser.UserName,
                Surname = currentUser.Surname,
                PhoneNumber = currentUser.PhoneNumber,
                Email = currentUser.Email,
                WatchedProducts = currentUser.WatchedProducts,
                IsEmployee = asEmployee != null,
                Company = employeeCompany,
            });
        }

        // GET: api/User/Profile/5
        /// <summary>
        /// daj profil uzytkownika imie, nazwisko, numer, mail itd
        /// dodac obserwowane produkty
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("Profile")]
        [ResponseType(typeof(UserProfileViewModel))]
        public IHttpActionResult GetUserProfile(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            var asEmployee = db.Employees.Find(id);
            Company employeeCompany = null;
            if (asEmployee != null)
            {
                employeeCompany = asEmployee.Company;
            }

            return Ok(new UserProfileViewModel()
            {
                Id = applicationUser.Id,
                UserName = applicationUser.UserName,
                Name = applicationUser.Name,
                Surname = applicationUser.Surname,
                PhoneNumber = applicationUser.PhoneNumber,
                Email = applicationUser.Email,
                WatchedProducts = applicationUser.WatchedProducts,
                IsEmployee = asEmployee != null,
                Company = employeeCompany,
            });
        }

        // PUT: api/User/Profile?id=asd
        /// <summary>
        /// zmodyfikuj profil uzytkownika (oczywisciej est autoryzacja)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userProfile"></param>
        /// <returns></returns>
        [Authorize]
        [Route("Profile")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUserProfile(string id, UserProfileBindingModel userProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = UserHelper.GetCurrentUser();
            user.Name = userProfile.Name;
            user.Surname = userProfile.Surname;
            user.Email = userProfile.Email;
            user.PhoneNumber = userProfile.PhoneNumber;
            UserManager.UpdateAsync(user).Wait();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing) 
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ApplicationUserExists(string id)
        {
            return db.Users.Count(e => e.Id == id) > 0;
        }
    }
}