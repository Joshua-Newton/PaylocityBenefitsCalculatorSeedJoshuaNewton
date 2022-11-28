using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

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
            var employees = HelperFunctions.GetAllEmployees();
            IEnumerable<GetEmployeeDto> employeesAsCollection = 
                from employeeObject in employees
                where employeeObject.Id == id
                select employeeObject;
            GetEmployeeDto targetEmployee = employeesAsCollection.FirstOrDefault();

            if(targetEmployee != null)
            {
                var result = new ApiResponse<GetEmployeeDto>()
                {
                    Data = targetEmployee,
                    Success = true,
                    Message = "Employee Found"
                };
                return result;
            }
            else
            {
                var result = new ApiResponse<GetEmployeeDto>()
                {
                    Success = false,
                    Message = "Employee Not Found"
                };
                return result;
            }
        }

        [SwaggerOperation(Summary = "Get all employees")]
        [HttpGet("")]
        public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll()
        {
            var employees = HelperFunctions.GetAllEmployees();

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
            var employees = HelperFunctions.GetAllEmployees();

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

                HelperFunctions.SerializeEmployeeCollection(employees);
                
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
            // Retrieve list of employees
            var employees = HelperFunctions.GetAllEmployees();

            // Retrieve the matching record
            GetEmployeeDto employeeAsDTO = HelperFunctions.GetEmployeeGivenId(id, employees);

            // If the matching record is not null, then we can update it.
            if(employeeAsDTO != null)
            {
                // Update the record
                employeeAsDTO.FirstName = updatedEmployee.FirstName;
                employeeAsDTO.LastName = updatedEmployee.LastName;
                employeeAsDTO.Salary = updatedEmployee.Salary;

                HelperFunctions.SerializeEmployeeCollection(employees);

                var result = new ApiResponse<GetEmployeeDto>
                {
                    Data = employeeAsDTO,
                    Message = "Employee Updated",
                    Success = true
                };
                return result;
            }
            // If the matching record is null, then it does not exist.
            else
            {
                var result = new ApiResponse<GetEmployeeDto>
                {
                    Message = "Employee of given id does not exist",
                    Success = false
                };
                return result;
            }
        }

        [SwaggerOperation(Summary = "Delete employee")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> DeleteEmployee(int id)
        {
            var employees = HelperFunctions.GetAllEmployees();
            var targetEmployee = HelperFunctions.GetEmployeeGivenId(id, employees);
            employees.Remove(targetEmployee);

            // Update IDs beyond this one
            IEnumerable<GetEmployeeDto> employeesAsCollection =
                from employeeObject in employees
                where employeeObject.Id > id
                select employeeObject;
            foreach(GetEmployeeDto employeeDTO in employeesAsCollection)
            {
                employeeDTO.Id -= 1;
            }

            HelperFunctions.SerializeEmployeeCollection(employees);

            var result = new ApiResponse<List<GetEmployeeDto>>
            {
                Data = employees,
                Message = "Employee Deleted",
                Success = true
            };
            return result;
        }
    }
}
