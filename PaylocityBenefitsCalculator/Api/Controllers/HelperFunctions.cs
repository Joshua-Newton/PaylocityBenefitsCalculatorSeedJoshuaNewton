using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using System.Text.Json;

namespace Api.Controllers
{

    public static class HelperFunctions
    {
        private static string jsonFilePath = "EmployeeData.json";

        public static List<GetEmployeeDto> GetAllEmployees()
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

        public static async void SerializeEmployeeCollection(List<GetEmployeeDto> employees)
        {
            // Self explanatory. Serialize the collection into json to save it.
            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            using FileStream createStream = System.IO.File.Create(jsonFilePath);
            await JsonSerializer.SerializeAsync(createStream, employees, serializeOptions);
            await createStream.DisposeAsync();
        }

        public static GetEmployeeDto GetEmployeeGivenId(int id, List<GetEmployeeDto> employees)
        {
            // Use LINQ to quickly get the record that matches the id given
            IEnumerable<GetEmployeeDto> employeeAsCollection =
                from employeeObject in employees
                where employeeObject.Id == id
                select employeeObject;

            // Retrieve the matching record
            GetEmployeeDto employeeAsDTO = employeeAsCollection.FirstOrDefault();

            return employeeAsDTO;
        }

        public static GetDependentDto GetDependentGivenId(int id, List<GetEmployeeDto> employees)
        {
            IEnumerable<GetDependentDto> dependentAsCollection =
                from employeeObject in employees
                from dependentObject in employeeObject.Dependents
                where dependentObject.Id == id
                select dependentObject;
            if(dependentAsCollection != null)
            {
                GetDependentDto targetDependent = dependentAsCollection.FirstOrDefault();
                return targetDependent;
            }
            return null;

        }

        public static List<GetDependentDto> GetAllDependents(List<GetEmployeeDto> employees) 
        {
            IEnumerable<GetDependentDto> dependentsAsCollection =
                from employeeObject in employees
                from dependentObject in employeeObject.Dependents
                select dependentObject;
            if(dependentsAsCollection != null)
            {
                // For some reason this:
                // dependentsAsCollection.ToList()
                // Is causing a null reference exception.
                // Working around with loops
                List<GetDependentDto> listOfDependents = new List<GetDependentDto>();
                bool endFound = false;
                int index = 0;
                while(!endFound)
                {
                    // This isn't a very good way to do this.... but ToList() failing is forcing my hand.
                    // TODO: Try to find a better way after the rest of the project is done.
                    try {
                        listOfDependents.Add(dependentsAsCollection.ElementAt<GetDependentDto>(index));
                        index++;
                    }
                    catch (Exception ex)
                    {
                        endFound = true;
                        continue;
                    }
                }
                return listOfDependents;
            }
            return null;
        }

        public static GetEmployeeDto GetEmployeeDtoContainingDependentId(int id, List<GetEmployeeDto> employees)
        {
            IEnumerable<GetEmployeeDto> targetEmployee =
                from employeeObject in employees
                from dependentObject in employeeObject.Dependents
                where dependentObject.Id == id
                select employeeObject;
            return targetEmployee.FirstOrDefault();
        }

        public static bool CheckIfSpouseOrPartnerExists(GetEmployeeDto employee)
        {
            foreach(GetDependentDto dependent in employee.Dependents)
            {
                if(dependent.Relationship == Relationship.DomesticPartner || dependent.Relationship == Relationship.Spouse)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckIfSpouseOrPartnerExistsExcludingId(GetEmployeeDto employee, int id)
        {
            foreach (GetDependentDto dependent in employee.Dependents)
            {
                if (dependent.Id != id && (dependent.Relationship == Relationship.DomesticPartner || dependent.Relationship == Relationship.Spouse))
                {
                    return true;
                }
            }
            return false;
        }
    } 
}
