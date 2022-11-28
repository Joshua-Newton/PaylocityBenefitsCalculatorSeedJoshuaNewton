using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DependentsController : ControllerBase
    {
        [SwaggerOperation(Summary = "Get dependent by id")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<GetDependentDto>>> Get(int id)
        {
            var employees = HelperFunctions.GetAllEmployees();
            GetDependentDto targetDependent = HelperFunctions.GetDependentGivenId(id, employees);
            if(targetDependent != null)
            {
                var result = new ApiResponse<GetDependentDto>()
                {
                    Data = targetDependent,
                    Success = true,
                    Message = "Dependent Found"
                };
                return result;
            }
            else
            {
                var result = new ApiResponse<GetDependentDto>()
                {
                    Success = false,
                    Message = "Dependent Not Found"
                };
                return result;
            }
        }

        [SwaggerOperation(Summary = "Get all dependents")]
        [HttpGet("")]
        public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> GetAll()
        {
            List<GetDependentDto> allDependents = HelperFunctions.GetAllDependents(HelperFunctions.GetAllEmployees());
            if(allDependents != null)
            {
                var result = new ApiResponse<List<GetDependentDto>>()
                {
                    Data = allDependents,
                    Success = true,
                    Message = "Dependents Found"
                };
                return result;
            }
            else
            {
                var result = new ApiResponse<List<GetDependentDto>>()
                {
                    Success = false,
                    Message = "Dependents Not Found"
                };
                return result;
            }
        }

        [SwaggerOperation(Summary = "Add dependent")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<List<AddDependentWithEmployeeIdDto>>>> AddDependent(AddDependentWithEmployeeIdDto newDependent)
        {
            throw new NotImplementedException();
        }

        [SwaggerOperation(Summary = "Update dependent")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<GetDependentDto>>> UpdateDependent(int id, UpdateDependentDto updatedDependent)
        {
            throw new NotImplementedException();
        }

        [SwaggerOperation(Summary = "Delete dependent")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> DeleteDependent(int id)
        {
            throw new NotImplementedException();
        }
    }
}
