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
            var employees = HelperFunctions.GetAllEmployees();
            if(employees != null)
            {
                var allDependents = HelperFunctions.GetAllDependents(employees);
                
                IEnumerable<GetEmployeeDto> targetEmployeeAsCollection =
                    from employeeObject in employees
                    where employeeObject.Id == newDependent.EmployeeId
                    select employeeObject;
                GetEmployeeDto targetEmployee = targetEmployeeAsCollection.FirstOrDefault();
                bool validRelationship = true;
                if(newDependent.Relationship == Relationship.Spouse || newDependent.Relationship == Relationship.DomesticPartner)
                {
                    validRelationship = !HelperFunctions.CheckIfSpouseOrPartnerExists(targetEmployee);
                }
                if(validRelationship)
                {
                    // Add dependent to the target Employee
                    GetDependentDto newDependentDto = new GetDependentDto();
                    newDependentDto.Id = allDependents.Count + 1;
                    newDependentDto.FirstName = newDependent.FirstName;
                    newDependentDto.LastName = newDependent.LastName;
                    newDependentDto.DateOfBirth = newDependent.DateOfBirth;
                    newDependentDto.Relationship = newDependent.Relationship;
                
                    if (targetEmployee != null)
                    {
                        targetEmployee.Dependents.Add(newDependentDto);
                    }
                    else
                    {
                        var resultingResponse = new ApiResponse<List<AddDependentWithEmployeeIdDto>>()
                        {
                            Success = false,
                            Message = "Target Employee does not exist"
                        };
                        return resultingResponse;
                    }
                    HelperFunctions.SerializeEmployeeCollection(employees);
                    var result = new ApiResponse<List<AddDependentWithEmployeeIdDto>>()
                    {
                        Data = new List<AddDependentWithEmployeeIdDto> {
                            newDependent
                        },
                        Success = true,
                        Message = "Dependent Added"
                    };
                    return result;
                }
                else
                {
                    var result = new ApiResponse<List<AddDependentWithEmployeeIdDto>>()
                    {
                        Success = false,
                        Message = "Employee already has a Spouse/DomesticPartner. Cannot add a second."
                    };
                    return result;
                }
            }
            else
            {
                var result = new ApiResponse<List<AddDependentWithEmployeeIdDto>>()
                {
                    Success = false,
                    Message = "No Employees to add dependent to"
                };
                return result;
            }
        }

        [SwaggerOperation(Summary = "Update dependent")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<GetDependentDto>>> UpdateDependent(int id, UpdateDependentDto updatedDependent)
        {
            var employees = HelperFunctions.GetAllEmployees();
            var dependent = HelperFunctions.GetDependentGivenId(id, employees);
            var targetEmployee = HelperFunctions.GetEmployeeDtoContainingDependentId(id, employees);
            bool validRelationship = true;
            if (updatedDependent.Relationship == Relationship.Spouse || updatedDependent.Relationship == Relationship.DomesticPartner)
            {
                validRelationship = !HelperFunctions.CheckIfSpouseOrPartnerExistsExcludingId(targetEmployee, id);
            }
            if(validRelationship)
            {
                if (dependent != null)
                {
                    dependent.FirstName = updatedDependent.FirstName;
                    dependent.LastName = updatedDependent.LastName;
                    dependent.DateOfBirth = updatedDependent.DateOfBirth;
                    dependent.Relationship= updatedDependent.Relationship;

                    HelperFunctions.SerializeEmployeeCollection(employees);
                    var result = new ApiResponse<GetDependentDto>
                    {
                        Data = dependent,
                        Message = "Dependent Updated",
                        Success = true
                    };
                    return result;
                }
                else
                // If the matching record is null, then it does not exist.
                {
                    var result = new ApiResponse<GetDependentDto>
                    {
                        Message = "Dependent of given id does not exist",
                        Success = false
                    };
                    return result;
                }
            }
            else
            {
                var result = new ApiResponse<GetDependentDto>
                {
                    Message = "Target Employee already has a Spouse/DomesticPartner. Dependent Not Updated.",
                    Success = false
                };
                return result;
            }

        }

        [SwaggerOperation(Summary = "Delete dependent")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> DeleteDependent(int id)
        {
            var employees = HelperFunctions.GetAllEmployees();
            var targetDependent = HelperFunctions.GetDependentGivenId(id, employees);
            if(targetDependent != null)
            {
                // If the dependent exists it must have an associated employee, so no need to check null on targetEmployee
                var targetEmployee = HelperFunctions.GetEmployeeDtoContainingDependentId(id, employees);
                targetEmployee.Dependents.Remove(targetDependent);
                
                // Update IDs beyond this one
                IEnumerable<GetDependentDto> dependentsAsCollection =
                    from dependentObject in HelperFunctions.GetAllDependents(employees)
                    where dependentObject.Id > id
                    select dependentObject;
                foreach (GetDependentDto dependentDTO in dependentsAsCollection)
                {
                    dependentDTO.Id -= 1;
                }

                HelperFunctions.SerializeEmployeeCollection(employees);
                var result = new ApiResponse<List<GetDependentDto>>
                {
                    Data = targetEmployee.Dependents.ToList(),
                    Message = "Dependent Deleted",
                    Success = false
                };
                return result;
            }
            else
            {
                var result = new ApiResponse<List<GetDependentDto>>
                {
                    Message = "Dependent of given id does not exist",
                    Success = false
                };
                return result;
            }
        }
    }
}
