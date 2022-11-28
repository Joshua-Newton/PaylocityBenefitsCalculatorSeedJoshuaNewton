using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;
using System.IO;
using System.Text;
using System;
using System.IO.Compression;

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
            var employees = getAllEmployees();

            if (employees != null)
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
            // Use a reusable helper method to get all employees
            var employees = getAllEmployees();

            if(employees != null)
            {

                var newEmployeeRecord = new GetEmployeeDto()
                {
                    Id = employees.Count + 1,
                    FirstName = newEmployee.FirstName,
                    LastName = newEmployee.LastName,
                    Salary = newEmployee.Salary,
                    DateOfBirth = newEmployee.DateOfBirth,
                    Dependents = newEmployee.Dependents as ICollection<GetDependentDto>
                };

                employees.Add(newEmployeeRecord);

                var serializeOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
//                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                //var jsonString = JsonSerializer.Serialize(employees, serializeOptions);

                using FileStream createStream = System.IO.File.Create(jsonFilePath);
                await JsonSerializer.SerializeAsync(createStream, employees, serializeOptions);
                await createStream.DisposeAsync();

                var result = new ApiResponse<List<AddEmployeeDto>>
                {
                    Data = new List<AddEmployeeDto>
                    {
                        newEmployee
                    },
                    Success = true
                };
                return result;
            }
            else
            {
                var result = new ApiResponse<List<AddEmployeeDto>>()
                {
                    Success = false,
                    Message = "Employee JSON Parse failed"
                };
                return result;
            }
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

        private List<GetEmployeeDto> getAllEmployees()
        {
            // Employees will go into a list of GetEmployeeDTOs
            var employees = new List<GetEmployeeDto>();

            // Use a StreamReader to read the json out of EmployeeData.json
            using (StreamReader r = new StreamReader(jsonFilePath))
            {
                string json = r.ReadToEnd();
                employees = JsonSerializer.Deserialize<List<GetEmployeeDto>>(json);
            }

            // return the deserialized collection
            // Don't bother checking for null, this will be handled by API methods and they may have different ways to handle it.
            // Null just means the deserialization failed, likely due to bad JSON. This shouldn't happen, but it's good to note.
            return employees;
        }
    }
}
