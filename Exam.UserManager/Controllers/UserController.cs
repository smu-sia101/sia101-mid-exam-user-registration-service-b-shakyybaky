using AutoMapper;
using Exam.UserManager.Models;
using Exam.UserManager.Service;
using Microsoft.AspNetCore.Mvc;

namespace Exam.UserManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserQueryService _userQueryService;
        private readonly IUserWriteService _userWriteService;
        private readonly IMapper _mapper;

        public UserController(IUserQueryService userQueryService, IUserWriteService userWriteService, IMapper mapper)
        {
            _userQueryService = userQueryService;
            _userWriteService = userWriteService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
            {
                UserDTO user = _userQueryService.Get(id);
                if (user == null)
                {
                    return NotFound();
                }
                UserResourceModel mapped = _mapper.Map<UserResourceModel>(user);
                return Ok(mapped);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                IEnumerable<UserDTO> users = _userQueryService.GetAll();
               
                if(users == null || !users.Any())
                {
                    return NotFound();
                }

                IEnumerable<UserResourceModel> mapped = 
                    _mapper.Map<IEnumerable<UserResourceModel>>(users);

                return Ok(mapped);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Add([FromBody] CreateUserModel user)
        {
            try
            {
                UserDTO mapped = _mapper.Map<UserDTO>(user);
                string userId = _userWriteService.Add(mapped);
                return CreatedAtAction(nameof(Get), new { id = userId }, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] UserResourceModel user)
        {
            try
            {
                user.Id = id;
                UserDTO mapped = _mapper.Map<UserDTO>(user);
                bool result = _userWriteService.Update(mapped);
                if (result)
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                var result = _userWriteService.Delete(id);
                if (result)
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
