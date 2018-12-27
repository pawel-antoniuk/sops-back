﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SOPS;
using SOPS.Models;
using SOPS.ModelHelpers;

namespace SOPS.Controllers
{
    public class WatchedProductsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        string loggedUserId = UserHelper.GetCurrentUserId();

        // GET: api/WatchedProducts
        [Authorize]
        [HttpGet]
        public IEnumerable<WatchedProduct> GetWatchedProducts()
        {
            //we could use '=>' here, idk if necessery
            return db.WatchedProducts.Where(u => u.ApplicationUserId == loggedUserId).ToList();
        }


        // POST: api/WatchedProducts/5
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(WatchedProduct))]
        public async Task<IHttpActionResult> PostWatchedProduct(int id)
        {
            //get product's id to add to user's watched products
            var product = await Task.Run(() => db.Products.Find(id));

            if (loggedUserId == null || product == null)
                return NotFound();

            db.WatchedProducts.Add(new WatchedProduct { ProductId = id, ApplicationUserId = loggedUserId });
            db.SaveChanges();

            return Ok();
        }

        // DELETE: api/WatchedProducts/5
        [Authorize]
        [HttpDelete]
        [ResponseType(typeof(WatchedProduct))]
        public async Task<IHttpActionResult> DeleteWatchedProduct(int id)
        {
            if (loggedUserId == null)
                return NotFound();

            var product = await Task.Run(() => db.WatchedProducts.SingleOrDefault(u => u.ApplicationUserId == loggedUserId && u.ProductId == id));
            if (product == null)
                return NotFound();

            db.WatchedProducts.Remove(product);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WatchedProductExists(int id)
        {
            return db.WatchedProducts.Count(e => e.Id == id) > 0;
        }
    }
}