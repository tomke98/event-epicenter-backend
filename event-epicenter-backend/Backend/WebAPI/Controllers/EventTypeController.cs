using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service;

namespace WebAPI.Controllers
{
    [ApiController]
    public class EventTypeController : ControllerBase
    {
        private readonly EventTypeService eventTypeService;

        public EventTypeController()
        {
            eventTypeService = new EventTypeService();
        }

        [Route("eventTypes")]
        [HttpGet]
        public async Task<IActionResult> GetEventTypesAsync()
        {
            var eventTypes = await eventTypeService.GetAllEventTypesAsync();

            if (eventTypes != null && eventTypes.Any())
            {
                return Ok(eventTypes);
            }

            return BadRequest("No event types found.");
        }

        [Route("eventTypes/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetEventTypeByIdAsync(string id)
        {
            var eventType = await eventTypeService.GetEventTypeByIdAsync(id);

            if (eventType != null)
            {
                return Ok(eventType);
            }

            return BadRequest("Event type not found.");
        }

        [Authorize(Roles = "admin")]
        [Route("eventTypes")]
        [HttpPost]
        public async Task<IActionResult> CreateEventTypeAsync(EventTypeREST eventType)
        {
            var result = await eventTypeService.CreateEventTypeAsync(new EventType(null, eventType.Name));

            if (result)
            {
                return Ok(eventType);
            }

            return BadRequest("Event type not created.");
        }

        [Authorize(Roles = "admin")]
        [Route("eventTypes/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateEventTypeAsync(string id, EventTypeREST eventType)
        {
            var existingEventType = await eventTypeService.GetEventTypeByIdAsync(id);

            if (existingEventType == null)
            {
                return BadRequest("Event type does not exist.");
            }

            var result = await eventTypeService.UpdateEventTypeAsync(new EventType(id, eventType.Name));

            if (result)
            {
                return Ok(eventType);
            }

            return BadRequest("Event type not updated.");
        }

        [Authorize(Roles = "admin")]
        [Route("eventTypes/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteEventTypeAsync(string id)
        {
            var existingEventType = await eventTypeService.GetEventTypeByIdAsync(id);

            if (existingEventType == null)
            {
                return BadRequest("Event type does not exist.");
            }

            var result = await eventTypeService.DeleteEventTypeAsync(id);

            if (result)
            {
                return Ok("Event type deleted.");
            }

            return BadRequest("Event type not updated.");
        }
    }

    public class EventTypeREST
    {
        public string Name { get; set; }
    }
}