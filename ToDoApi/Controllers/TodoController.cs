using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ToDoApi.Models;

namespace ToDoApi.Controllers
{
    /// <summary>
    /// TodoController Class
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private readonly TodoContext _context;

        /// <summary>
        /// Contructor for the TodoControlller
        /// </summary>
        public TodoController(TodoContext context)
        {
            _context = context;

            if (_context.TodoItems.Count() == 0)
            {
                //Seed To Do Items
                _context.TodoItems.Add(new TodoItem { Name = "Watch One Piece from Episode 1" });
                _context.TodoItems.Add(new TodoItem { Name = "Create test API with Swagger" });
                _context.TodoItems.Add(new TodoItem { Name = "Get a Project" });
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Gets all TodoItems.
        /// </summary>
        /// <returns>All To Do Items</returns>
        /// <response code="200">Returns all items on the list</response> 
        [HttpGet]
        [ProducesResponseType(typeof(TodoItem), 200)]
        public IEnumerable<TodoItem> GetAll()
        {
            return _context.TodoItems.ToList();
        }

        /// <summary>
        /// Gets a specific TodoItem.
        /// </summary>
        /// <param name="id"></param> 
        /// /// <returns>The To Do item if found otherwise 404</returns>
        /// <response code="200">Returns the To Do item</response>
        /// <response code="404">If the item is not found</response>   
        [HttpGet("{id}", Name = "GetTodo")]
        [ProducesResponseType(typeof(TodoItem), 200)]
        [ProducesResponseType(typeof(TodoItem), 404)]
        public IActionResult GetById(long id)
        {
            var item = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        /// <summary>
        /// Creates a TodoItem.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>A newly-created TodoItem</returns>
        /// <response code="201">Returns the newly-created item</response>
        /// <response code="400">If the item is null</response>            
        [HttpPost]
        [ProducesResponseType(typeof(TodoItem), 201)]
        [ProducesResponseType(typeof(TodoItem), 400)]
        public IActionResult Create([FromBody] TodoItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            _context.TodoItems.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute("GetTodo", new { id = item.Id }, item);
        }

        /// <summary>
        /// Updates a specific TodoItem.
        /// </summary>
        /// /// <param name="id"></param> 
        /// <param name="item"></param> 
        /// <response code="200">Updated succesfully</response>
        /// <response code="400">Incorrect Request</response>  
        /// <response code="404">Item not found</response>  
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoItem), 200)]
        [ProducesResponseType(typeof(TodoItem), 400)]
        [ProducesResponseType(typeof(TodoItem), 404)]
        public IActionResult Update(long id, [FromBody] TodoItem item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var todo = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            todo.IsComplete = item.IsComplete;
            todo.Name = item.Name;

            _context.TodoItems.Update(todo);
            _context.SaveChanges();
            return new NoContentResult();
        }

        /// <summary>
        /// Deletes a specific TodoItem.
        /// </summary>
        /// <param name="id"></param>  
        /// <response code="200">Updated succesfully</response>
        /// <response code="404">Item not found</response> 
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var todo = _context.TodoItems.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todo);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}