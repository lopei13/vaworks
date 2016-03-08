using System;
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
using VaWorks.Web.DataAccess;
using VaWorks.Web.DataAccess.Entities;

namespace VaWorks.Web.Controllers
{
    public class ValvesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Valves
        public IQueryable<Valve> GetValves()
        {
            return db.Valves;
        }

        // GET: api/Valves/5
        [ResponseType(typeof(Valve))]
        public async Task<IHttpActionResult> GetValve(int id)
        {
            Valve valve = await db.Valves.FindAsync(id);
            if (valve == null)
            {
                return NotFound();
            }

            return Ok(valve);
        }

        // PUT: api/Valves/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutValve(int id, Valve valve)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != valve.ValveId)
            {
                return BadRequest();
            }

            db.Entry(valve).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ValveExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Valves
        [ResponseType(typeof(Valve))]
        public async Task<IHttpActionResult> PostValve(Valve valve)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Valves.Add(valve);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = valve.ValveId }, valve);
        }

        // DELETE: api/Valves/5
        [ResponseType(typeof(Valve))]
        public async Task<IHttpActionResult> DeleteValve(int id)
        {
            Valve valve = await db.Valves.FindAsync(id);
            if (valve == null)
            {
                return NotFound();
            }

            db.Valves.Remove(valve);
            await db.SaveChangesAsync();

            return Ok(valve);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ValveExists(int id)
        {
            return db.Valves.Count(e => e.ValveId == id) > 0;
        }
    }
}