import { currencyFormat, baseUrl } from "./Constants";

const Employee = (props) => {
    const firstName = props.firstName || '';
    const lastName = props.lastName || '';
    const salary = props.salary || 0;

    const DeleteEmployee = async (id) => {
        const result = await fetch(`${baseUrl}/api/v1/Employees/${id}`, {
            method: 'DELETE',
            headers: {
                'Content-Type' : 'application/json'
            },
            type: 'json'
        })
    }

    return (
        <tr>
            <th scope="row">{props.id}</th>
            <td>{lastName}</td>
            <td>{firstName}</td>
            <td>{props.dateOfBirth}</td>
            <td>{currencyFormat(salary)}</td>
            <td>{props.dependents?.length || 0}</td>
            <td>
                <a href="#" onClick={() => props.setEmployee(props.id)}data-bs-toggle="modal" data-bs-target={`#${props.editModalId}`}>Edit</a>  
                <a href="#" onClick={() => DeleteEmployee(props.id)}>Delete</a>
            </td>
        </tr>
    );
};

export default Employee;