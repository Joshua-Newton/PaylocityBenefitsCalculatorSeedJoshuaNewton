using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Api.Controllers
{

    [ApiController]
    [Route("api/v1/Employees")]
    public class EmployeesController : ControllerBase
    {
        private string jsonFilePath = "EmployeeData.json";

        [SwaggerOperation(Summary = "Get employee by id")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id)
        {
            throw new NotImplementedException();
        }

        [SwaggerOperation(Summary = "Get all employees")]
        [HttpGet("")]
        public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll()
        {
            var employees = new List<GetEmployeeDto>();

            using (StreamReader r = new StreamReader(jsonFilePath))
            {
                string json = r.ReadToEnd();
                employees = JsonSerializer.Deserialize<List<GetEmployeeDto>>(json);
            }

            if (employees != null && employees.Count > 0)
            {
                if(employees.Count > 0)
                {
                    var result = new ApiResponse<List<GetEmployeeDto>>
                    {
                        Data = employees,
                        Success = true
                    };
                    return result;
                }
                else
                {
                    var result = new ApiResponse<List<GetEmployeeDto>>()
                    {
                        Success = true,
                        Message = "No Employees Exist"
                    };
                    return result;
                }
            }
            else
            {
                var result = new ApiResponse<List<GetEmployeeDto>>()
                {
                    Success = false,
                    Message = "Employee JSON Parse failed"
                };
                return result;
            }
        }

        [SwaggerOperation(Summary = "Add employee")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<List<AddEmployeeDto>>>> AddEmployee(AddEmployeeDto newEmployee)
        {
            var employeesList = new List<AddEmployeeDto>();
            employeesList.Add(newEmployee);

            // Store employee?

            var result = new ApiResponse<List<AddEmployeeDto>>
            {
                Data = employeesList,
                Success = true
            };

            return result;
            //throw new NotImplementedException();
        }

        [SwaggerOperation(Summary = "Update employee")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> UpdateEmployee(int id, UpdateEmployeeDto updatedEmployee)
        {
            throw new NotImplementedException();
        }

        [SwaggerOperation(Summary = "Delete employee")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> DeleteEmployee(int id)
        {
            throw new NotImplementedException();
        }
    }
}
