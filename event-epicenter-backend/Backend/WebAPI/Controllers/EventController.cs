using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service;

namespace WebAPI.Controllers
{
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventService eventService;

        public EventController()
        {
            eventService = new EventService();
        }

        [Route("events")]
        [HttpGet]
        public async Task<IActionResult> GetEventsAsync()
        {
            var events = await eventService.GetAllEventsAsync();

            if(events != null && events.Any())
            {
                return Ok(events);
            }

            return BadRequest("No events found.");
        }

        [Route("events/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetEventByIdAsync(string id)
        {
            var _event = await eventService.GetEventByIdAsync(id);

            if (_event != null)
            {
                return Ok(_event);
            }

            return BadRequest("Event not found.");
        }

        [Authorize(Roles = "admin")]
        [Route("events")]
        [HttpPost]
        public async Task<IActionResult> CreateEventAsync(EventREST _event)
        {
            var result = await eventService.CreateEventAsync(new Event(null, _event.Address, _event.City, _event.Date, _event.Description, _event.EventTypeId, _event.ImagePath, _event.Location, _event.Name, _event.Price));

            if (result)
            {
                return Ok(_event);
            }

            return BadRequest("Event not created.");
        }

        [Authorize(Roles = "admin")]
        [Route("events/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateEventAsync(string id, EventREST _event)
        {
            var existingEvent = await eventService.GetEventByIdAsync(id);

            if (existingEvent == null)
            {
                return BadRequest("Event not found.");
            }

            var result = await eventService.UpdateEventAsync(new Event(id, _event.Address, _event.City, _event.Date, _event.Description, _event.EventTypeId, _event.ImagePath, _event.Location, _event.Name, _event.Price));

            if (result)
            {
                return Ok(_event);
            }

            return BadRequest("Event not updated.");
        }

        [Authorize(Roles = "admin")]
        [Route("events/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteEventAsync(string id)
        {
            var existingEvent = await eventService.GetEventByIdAsync(id);

            if (existingEvent == null)
            {
                return BadRequest("Event does not exist.");
            }

            var result = await eventService.DeleteEventAsync(id);

            if (result)
            {
                return Ok("Event deleted.");
            }

            return BadRequest("Event not updated.");
        }

        [Authorize(Roles = "admin")]
        [Route("events/image")]
        [HttpPost]
        public async Task<IActionResult> UploadImage()
        {
            string bucketName = "event-epicenter.appspot.com";

            try
            {
                var file = Request.Form.Files[0];

                if (file.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;

                        var credential = GoogleCredential.FromFile("./firebaseConfig.json");
                        var storage = StorageClient.Create(credential);

                        var objectName = $"{fileName}";

                        await storage.UploadObjectAsync(bucketName,objectName,null,memoryStream);

                        var imageUrl = $"https://storage.googleapis.com/{bucketName}/{objectName}";

                        return Ok(imageUrl);
                    }
                }
                else
                {
                    return BadRequest("Image not uploaded.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error uploading image: {ex.Message}");
            }
        }
    }

    public class EventREST
    {
        public string Address { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string EventTypeId { get; set; }
        public string ImagePath { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}