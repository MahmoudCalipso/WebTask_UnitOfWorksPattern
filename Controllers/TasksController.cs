using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebTask.DTO;
using WebTask.Entities;
using WebTask.IServices;

namespace WebTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITasksService _tasksService;
        public TasksController(ITasksService tasksService)
        {
            _tasksService = tasksService;
        }

        [HttpGet("GetAllTasks")]
        public async Task<ActionResult<IEnumerable<TasksDto>>> GetTasks(
        [FromQuery] EnumStatus? status = null,
        [FromQuery] EnumPriority? priority = null,
        [FromQuery] string? Title = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        {
            var tasks = await _tasksService.GetPaginatedTasksAsync(page, pageSize, status, priority, Title);
            return Ok(tasks);
        }

        [HttpPost("CreateTask")]
        public async Task<ActionResult<TasksDto>> CreateTask(CreateTasksDto createTaskDto)
        {
            try
            {
                var task = await _tasksService.CreateTaskAsync(createTaskDto);
                return task != null ? task : new TasksDto();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetTask/{id}")]
        public async Task<ActionResult<TasksDto>> GetTask(Guid id)
        {
            var task = await _tasksService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        [HttpPut("UpdateTask/{id}")]
        public async Task<ActionResult<TasksDto>> UpdateTask(Guid id, [FromBody] UpdateTasksDto updateTaskDto)
        {
            try
            {
                var task = await _tasksService.UpdateTaskAsync(id, updateTaskDto);
                return Ok(task);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteTask/{id}")]
        public async Task<ActionResult<String>> DeleteTask(Guid id)
        {
            var result = await _tasksService.DeleteTaskAsync(id);
            if (!result)
            {
                return "Task not found";
            }
            return "Task deleted successfully";
        }

    }
}
