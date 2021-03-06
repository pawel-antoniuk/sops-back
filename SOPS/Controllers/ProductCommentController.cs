﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SOPS;
using SOPS.ModelHelpers;
using SOPS.Models;

namespace SOPS.Controllers
{
    public class ProductCommentController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ProductComment/id
        /// <summary>
        /// daj komentarze dla wskazanego produktu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IQueryable<ProductComment> GetProductComment(int id)
        {
            //we could use '=>' here, idk if necessery
            return db.ProductComments.Where(pc => pc.ProductId == id);
        }

        // POST: api/ProductComment/id
        /// <summary>
        /// postaw komentarz
        /// sprawdzic czy dziala autoryzacja (loggedUserId)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="commentFromBody"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(ProductComment))]
        public IHttpActionResult PostProductComment(int id, ProductCommentBindingModel commentFromBody)
        {
            var userId = UserHelper.GetCurrentUserId();

            if (!db.Products.Any(p => p.Id == id) || commentFromBody == null || userId == null)
                return NotFound();

            var productComment = new ProductComment
            {
                Comment = commentFromBody.Comment,
                ApplicationUserId = userId,
                ProductId = id,
                Date = DateTime.Now
            };

            db.ProductComments.Add(productComment);
            db.SaveChanges();

            return Ok(productComment);
        }

        // DELETE: api/ProductComment/5
        /// <summary>
        /// usun komentarz
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [ResponseType(typeof(ProductComment))]
        [HttpDelete]
        public IHttpActionResult DeleteProductComment(int id)
        {
            ProductComment productComment = db.ProductComments.Find(id);
            if (productComment == null)
            {
                return NotFound();
            }

            db.ProductComments.Remove(productComment);
            db.SaveChanges();

            return Ok(productComment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductCommentExists(int id)
        {
            return db.ProductComments.Count(e => e.Id == id) > 0;
        }
    }
}