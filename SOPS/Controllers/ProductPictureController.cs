﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Description;
using SOPS;
using SOPS.Models;

namespace SOPS.Controllers
{
    public class ProductPictureController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ProductPictures/5
        /// <summary>
        /// daj obrazek dla produktu o podanym id
        /// </summary>
        /// <param name="id">id produktu</param>
        /// <returns></returns>
        [ResponseType(typeof(ProductPicture))]
        public HttpResponseMessage GetProductPicture(int id)
        {
            ProductPicture productPicture = db.ProductPictures.Find(id);
            if (productPicture == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            var response = Request.CreateResponse();
            response.Content = new StreamContent(new MemoryStream(productPicture.Content));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

            return response;
        }

        // PUT: api/ProductPictures/5
        // [Authorize(Roles = "Administrator,Employee")]
        [ResponseType(typeof(void))]
        /* public IHttpActionResult PutProductPicture(int id, ProductPicture productPicture)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productPicture.ProductId)
            {
                return BadRequest();
            }

            db.Entry(productPicture).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductPictureExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }*/

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductPictureExists(int id)
        {
            return db.ProductPictures.Count(e => e.ProductId == id) > 0;
        }
    }
}