using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_mobile.Context;
using Backend_mobile.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Backend_mobile
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarMaintenanceRecordsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public CarMaintenanceRecordsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/CarMaintenanceRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarMaintenanceRecord>>> GetMaintenanceRecords()
        {
            return await _context.MaintenanceRecords.ToListAsync();
        }

        // GET: api/CarMaintenanceRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CarMaintenanceRecord>> GetCarMaintenanceRecord(int id)
        {
            var carMaintenanceRecord = await _context.MaintenanceRecords.FindAsync(id);

            if (carMaintenanceRecord == null)
            {
                return NotFound();
            }

            return carMaintenanceRecord;
        }

        // PUT: api/CarMaintenanceRecords/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCarMaintenanceRecord(int id, CarMaintenanceRecord carMaintenanceRecord)
        {
            if (id != carMaintenanceRecord.Id)
            {
                return BadRequest();
            }

            _context.Entry(carMaintenanceRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Notify WebSocket clients about the update
                var message = new
                {
                    action = "update",
                    record = carMaintenanceRecord
                };
                Console.WriteLine(JsonConvert.SerializeObject(message, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
                await WebSocketHandler.Broadcast(JsonConvert.SerializeObject(message, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarMaintenanceRecordExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CarMaintenanceRecords
        [HttpPost]
        public async Task<ActionResult<CarMaintenanceRecord>> PostCarMaintenanceRecord(CarMaintenanceRecord carMaintenanceRecord)
        {
            carMaintenanceRecord.Id = 0; // Reset the ID to avoid conflicts
            _context.MaintenanceRecords.Add(carMaintenanceRecord);
            await _context.SaveChangesAsync();

            // Notify WebSocket clients about the new record
            var message = new
            {
                action = "add",
                record = carMaintenanceRecord
            };
            Console.WriteLine(JsonConvert.SerializeObject(message, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            await WebSocketHandler.Broadcast(JsonConvert.SerializeObject(message, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));


            return CreatedAtAction("GetCarMaintenanceRecord", new { id = carMaintenanceRecord.Id }, carMaintenanceRecord);
        }

        // DELETE: api/CarMaintenanceRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCarMaintenanceRecord(int id)
        {
            var carMaintenanceRecord = await _context.MaintenanceRecords.FindAsync(id);
            if (carMaintenanceRecord == null)
            {
                return NotFound();
            }

            _context.MaintenanceRecords.Remove(carMaintenanceRecord);
            await _context.SaveChangesAsync();

            // Notify WebSocket clients about the deletion
            var message = new
            {
                action = "delete",
                record = new { id }
            };
            Console.WriteLine(JsonConvert.SerializeObject(message, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            await WebSocketHandler.Broadcast(JsonConvert.SerializeObject(message, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));


            return NoContent();
        }

        private bool CarMaintenanceRecordExists(int id)
        {
            return _context.MaintenanceRecords.Any(e => e.Id == id);
        }
    }
}
