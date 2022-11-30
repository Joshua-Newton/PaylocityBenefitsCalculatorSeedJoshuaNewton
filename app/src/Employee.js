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

        props.getEmployees();
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
                <button className={"btn btn-secondary"} onClick={() => {props.setEmployee(props.id)}} data-bs-toggle="modal" data-bs-target={`#${props.editModalId}`}>Edit</button>  
                <button className={"mx-1 btn btn-secondary"} onClick={() => DeleteEmployee(props.id)}>Delete</button>
                <button className={"btn btn-secondary"} onClick={() => {props.setEmployee(props.id)}} data-bs-toggle="modal" data-bs-target={`#${props.paycheckModalId}`}>Calculate Paycheck</button>
            </td>
        </tr>
    );
};

export default Employee;