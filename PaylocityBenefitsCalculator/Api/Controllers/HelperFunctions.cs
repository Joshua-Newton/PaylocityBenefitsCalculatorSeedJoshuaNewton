using Api.Dtos.Employee;
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
    }
}
