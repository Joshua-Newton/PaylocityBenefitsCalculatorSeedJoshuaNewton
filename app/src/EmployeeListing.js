import React, { useState, useEffect, useRef } from 'react';
import Employee from './Employee';
import { baseUrl, currencyFormat } from './Constants';

const EmployeeListing = () => {
    // START PAYCHECK CALCULATION VARIABLES/FUNCTIONS
    const payPeriods = 26;
    const monthlyDeduction = 1000;
    const perDependentMonthlyDeduction = 600;
    const highEarnerThreshold = 80000;
    const highEarnerAdditionalPercentCost = 0.02;
    const oldDependentAgeThreshold = 50;
    const oldDependentAdditionalMonthlyDeduction = 200;

    const [payCheckSalary, setPayCheckSalary] = useState(Number);
    const [basePay, setBasePay] = useState(Number);
    const [dependentDeduction, setDependentDeduction] = useState(Number);
    const [highEarnerDeduction, setHighEarnerDeduction] = useState(Number);
    const [oldDependentDeduction, setOldDependentDeduction] = useState(Number);
    const [finalPayCheckValue, setFinalPayCheckValue] = useState(Number);

    async function setPayVariables(){
        let _payCheckSalary = (Number(currentEmployee == null ? 0 : currentEmployee.salary))
        await setPayCheckSalary(_payCheckSalary)
        // Base Pay
        let _basePay = (Number(currentEmployee == null ? 0 : currentEmployee.salary)/payPeriods)
        await setBasePay(_basePay);
        // Dependent Deduction
        let _dependentDeduction = (Number(currentEmployee == null || currentEmployee.dependents == null ? 0 : currentEmployee.dependents.length * perDependentMonthlyDeduction));
        await setDependentDeduction(_dependentDeduction);
        // High Earner Deduction
        let _highEarnerDeduction = 0;
        if(Number(currentEmployee == null || currentEmployee.salary == null ? 0 : currentEmployee.salary) > highEarnerThreshold){
            _highEarnerDeduction = (Number(currentEmployee.salary) * highEarnerAdditionalPercentCost / payPeriods);
            await setHighEarnerDeduction(_highEarnerDeduction);
        }
        else{
            await setHighEarnerDeduction((0));
        }
        // OldDependent Deduction
        let oldDependentDeductionAmount = 0;
        if(currentEmployee == null || currentEmployee.dependents == null || currentEmployee.dependents.length <= 0){
            await setOldDependentDeduction((oldDependentDeductionAmount));
        }
        else {
            let currentDate = new Date();
            // To calculate the no. of days between two dates
            let daysAgeThreshold = oldDependentAgeThreshold * 365;
            for (let i = 0; i < currentEmployee.dependents.length; i++) {
                var timeAge = currentDate.getTime() - new Date(currentEmployee.dependents[i].dateOfBirth.split('T')[0]).getTime();
                var daysAge = timeAge / (1000 * 3600 * 24);
                if(daysAge > daysAgeThreshold){
                    oldDependentDeductionAmount += oldDependentAdditionalMonthlyDeduction;
                }
            }
            await setOldDependentDeduction((oldDependentDeductionAmount));
        }
        // Final Paycheck value
        let result = _basePay - monthlyDeduction - _dependentDeduction - _highEarnerDeduction - oldDependentDeductionAmount;
        await setFinalPayCheckValue(Number(result));
    }
    
    // Step 1 - Get Salary
    // Step 2 - Get base pay per pay period
    // Step 3 - Deduct base employee benefits cost from paycheck
    // Step 4 - Get Pay after Dependent Deduction
    // Step 5 - If it's a high earner, deduct the high earner rate
    // Step 6 - If any dependents are older than the threshold age, then deduct additional costs for these dependents
    
    // END PAYCHECK CALCULATION VARIABLES/FUNCTIONS
    
    // States to keep track of
    const [employees, setEmployees] = useState([]);
    const [error, setError] = useState(null);
    const [currentEmployee, setCurrentEmployee] = useState([]);
    const [newEmployee, setNewEmployee] = useState('');
    // Fields for Employee values.
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [dateOfBirth, setDateOfBirth] = useState('');
    const [salary, setSalary] = useState('');

    const [dependentFields, setDependentFields] = useState([
        {id: '', lastName: '', firstName: '', dateOfBirth: '', relationship: ''}
    ])

    const possibleRelationships = ["none", "spouse", "domesticPartner", "child"]

    function DetermineRelationship(relationship) {
        return possibleRelationships[relationship];
    }

    // Refs for loading in existing values
    // This solves issue where states were not updating in time for modals to open
    // However, I couldn't figure out how to pass them to child components, which is why this file ballooned.
    const lastNameRef = useRef(null);
    const firstNameRef = useRef(null);
    const dateOfBirthRef = useRef(null);
    const salaryRef = useRef(null);

    const payCheckSalaryRef = useRef(null);
    const basePayRef = useRef(null);
    const monthlyDeductionRef = useRef(null);
    const dependentDeductionRef = useRef(null);
    const highEarnerDeductionRef = useRef(null);
    const oldDependentDeductionRef = useRef(null);
    const finalPayCheckValueRef = useRef(null);
    
    // change handlers
    const handleFirstNameChange = event => {
        setFirstName(event.target.value);
    }
    const handleLastNameChange = event => {
        setLastName(event.target.value);
    }
    const handleDateOfBirthChange = event => {
        setDateOfBirth(event.target.value);
    }
    const handleSalaryChange = event => {
        setSalary(event.target.value);
    }

    const SubmitEmployee = async () => {
        const employeeData = {
            FirstName: firstName,
            LastName: lastName,
            DateOfBirth: dateOfBirth,
            Salary: salary,
            Dependents: dependentFields
        }
        
        // If this is a new employee, call POST
        if(newEmployee) {
            const result = await fetch(`${baseUrl}/api/v1/Employees/`, {
                method: 'POST',
                headers: {
                    'Content-Type' : 'application/json'
                },
                body: JSON.stringify(employeeData),
                type: 'json'
            })
        }
        // If this is an existing employee being edited, call PUT
        else {
            const resultEmployeePut = await fetch(`${baseUrl}/api/v1/Employees/UpdateDependents/${currentEmployee.id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type' : 'application/json'
                },
                body: JSON.stringify(employeeData),
                type: 'json'
            })
            // Also need to update their dependents, add new ones, and delete old ones.

        }

        getEmployees();
    }

    async function setEmployee(employeeId){
        const rawEmployee = await fetch(`${baseUrl}/api/v1/Employees/${employeeId}`, {
            method: 'GET',
            headers: {
                'Content-Type' : 'application/json'
            }
        });

        const responseEmployee = await rawEmployee.json();

        if(responseEmployee.success){
            setNewEmployee(false);
            setCurrentEmployee(responseEmployee.data);
            setDependentFields(responseEmployee.data.dependents);
            setFirstName(responseEmployee.data.firstName);
            setLastName(responseEmployee.data.lastName);
            setDateOfBirth(responseEmployee.data.dateOfBirth);
            setSalary(responseEmployee.data.salary);
            lastNameRef.current.value = responseEmployee.data.lastName;
            firstNameRef.current.value = responseEmployee.data.firstName;
            dateOfBirthRef.current.value = responseEmployee.data.dateOfBirth.split('T')[0];
            salaryRef.current.value = responseEmployee.data.salary;
        }
        else {
            setNewEmployee(true);
            setCurrentEmployee(null);
            setDependentFields(null);
            setFirstName(null);
            setLastName(null);
            setDateOfBirth(null);
            setSalary(null);
            lastNameRef.current.value = "";
            firstNameRef.current.value = "";
            dateOfBirthRef.current.value = null;
            salaryRef.current.value = "";
        }
        

    }

    const getEmployees = async () => {
        const raw = await fetch(`${baseUrl}/api/v1/Employees`, {
            method: 'GET',
            headers: {
                'Content-Type' : 'application/json'
            }
        });
        const response = await raw.json();
        if (response.success) {
            setEmployees(response.data);
            setError(null);
        }
        else {
            setEmployees([]);
            setError(response.error);
        }
    };

    useEffect(() => {  
        getEmployees();
    }, []);

    useEffect(() => {
        console.log("Current Employee update: " + currentEmployee)
        setPayVariables()
    }, [currentEmployee])

    useEffect(() => {
        basePayRef.current.innerHTML = currencyFormat(basePay);
    }, [basePay])

    useEffect(() => {
        payCheckSalaryRef.current.innerHTML = currencyFormat(payCheckSalary);
    }, [payCheckSalary]);
    
    useEffect(() => {
        dependentDeductionRef.current.innerHTML = currencyFormat(dependentDeduction)
    }, [dependentDeduction]);
    
    useEffect(() => {
        highEarnerDeductionRef.current.innerHTML = currencyFormat(highEarnerDeduction);
    }, [highEarnerDeduction]);
    
    useEffect(() => {
        oldDependentDeductionRef.current.innerHTML = currencyFormat(oldDependentDeduction);
    }, [oldDependentDeduction]);
    
    useEffect(() => {
        finalPayCheckValueRef.current.innerHTML = currencyFormat(finalPayCheckValue);
    }, [finalPayCheckValue]);
    

    // Handler for when a dependent field changes
    const handleDependentFormChange = (index, event) => {
        let data = [...dependentFields];
        if(event.target.name == "relationship"){
            switch(event.target.value){
                case "spouse":
                    data[index][event.target.name] = 1;
                    break;
                case "domesticPartner":
                    data[index][event.target.name] = 2;
                    break;
                case "child":
                    data[index][event.target.name] = 3;
                    break;
                default:
                    data[index][event.target.name] = 0;
                    break;
            }
        }
        else {
            data[index][event.target.name] = event.target.value;
        }
        setDependentFields(data);
    }

    // Function to add another dependent section in the employee modal
    const addFields = () => {
        let newField = {id: '0', lastName: '', firstName: '', dateOfBirth: '', relationship: ''}
        if(dependentFields){
            setDependentFields([...dependentFields, newField]);
        }
        else {
            setDependentFields([newField]);
        }
    }

    // Function to remove a dependent section in the employee modal
    const removeDependent = (index) => {
        let data = [...dependentFields];
        data.splice(index, 1);
        setDependentFields(data);
    }

    const addEmployeeModalId = "add-employee-modal";
    const paycheckModalId = "paycheck-modal";

    return (
    <div className="employee-listing">
        <table className="table caption-top">
            <caption>Employees</caption>
            <thead className="table-dark">
                <tr>
                    <th scope="col">Id</th>
                    <th scope="col">LastName</th>
                    <th scope="col">FirstName</th>
                    <th scope="col">DOB</th>
                    <th scope="col">Salary</th>
                    <th scope="col">Dependents</th>
                    <th scope="col">Actions</th>
                </tr>
            </thead>
            <tbody>
            {employees.map(({id, firstName, lastName, dateOfBirth, salary, dependents}) => (
                <Employee
                    key={id}
                    id={id}
                    firstName={firstName}
                    lastName={lastName}
                    dateOfBirth={dateOfBirth}
                    salary={salary}
                    dependents={dependents}
                    editModalId={addEmployeeModalId}
                    paycheckModalId={paycheckModalId}
                    currentEmployee={currentEmployee}
                    setEmployee={setEmployee}
                    getEmployees={getEmployees}
                />
            ))}
            </tbody>
        </table>
        <button onClick={() => setEmployee(0)} type="button" className="btn btn-primary" data-bs-toggle="modal" data-bs-target={`#${addEmployeeModalId}`}>Add Employee</button>
        {/* Former AddEmployeeModal, was having trouble passing refs so I pulled it back into the EmployeeListing. */}
        <div className="modal fade" id={addEmployeeModalId} tabIndex="-1" aria-labelledby="add-employee-modal-label" aria-hidden="true">
            <div className="modal-dialog">
                <div className="modal-content">
                    <div className="modal-header">
                        <h1 className="modal-title fs-5" id="add-employee-modal-label">Add/Edit Employee</h1>
                        <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div className="modal-body">
                        <div className="row">
                            <div className="col">
                                <h4>Employee Details</h4>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-6">
                                <label>LastName</label>
                            </div>
                            <div className="col-6">
                                <input id="idLastName" ref={lastNameRef} type={"text"} onChange={handleLastNameChange}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-6">
                                <label>FirstName</label>
                            </div>
                            <div className="col-6">
                                <input id="idFirstName" ref={firstNameRef} type={"text"} onChange={handleFirstNameChange}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-6">
                                <label>DOB</label>
                            </div>
                            <div className="col-6">
                                <input id="idDOB" ref={dateOfBirthRef} type={"date"} onChange={handleDateOfBirthChange}></input>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-6">
                                <label>Salary</label>
                            </div>
                            <div className="col-6">
                                <input id="idSalary" ref={salaryRef} type={"number"} onChange={handleSalaryChange}></input>
                            </div>
                        </div>

                        <form>
                            {dependentFields == null ? null : dependentFields.map((input, index) => {
                                return (
                                    <div key={index} className="DependentDetails">
                                        <div className="row">
                                            <div className="col">
                                                <h4>Dependent</h4>
                                            </div>
                                        </div>
                                        <div className="row">
                                            <div className='col'>
                                                <button type='button' className='btn btn-secondary' onClick={() => removeDependent(index)}>Remove Dependent</button>
                                            </div>
                                        </div>
                                        <div className="row">
                                            <div className="col-6">
                                                <label>First Name</label>
                                            </div>
                                            <div className="col-6">
                                                <input 
                                                    name="firstName"
                                                    value={input.firstName} 
                                                    type={"text"}
                                                    onChange={event => handleDependentFormChange(index, event)}>
                                                </input>
                                            </div>
                                        </div>
                                        <div className="row">
                                            <div className="col-6">
                                                <label>Last Name</label>
                                            </div>
                                            <div className="col-6">
                                                <input 
                                                    name="lastName"
                                                    value={input.lastName}
                                                    type={"text"}
                                                    onChange={event => handleDependentFormChange(index, event)}>
                                                 </input>
                                            </div>
                                        </div>
                                        <div className="row">
                                            <div className="col-6">
                                                <label>DateOfBirth</label>
                                            </div>
                                            <div className="col-6">
                                                <input 
                                                    name="dateOfBirth"
                                                    value={input.dateOfBirth.split("T")[0]}
                                                    type={"date"}
                                                    onChange={event => handleDependentFormChange(index, event)}>
                                                </input>
                                            </div>
                                        </div>
                                        <div className="row">
                                            <div className="col-6">
                                                <label>Relationship</label>
                                            </div>
                                            <div className="col-6">
                                                <select name="relationship" value={DetermineRelationship(input.relationship)} onChange={event => handleDependentFormChange(index, event)}>
                                                    <option value="none">None</option>
                                                    <option value="spouse">Spouse</option>
                                                    <option value="domesticPartner">Domestic Partner</option>
                                                    <option value="child">Child</option>
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                )
                            })}
                        </form>


                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type='button' className='btn btn-secondary' onClick={addFields}>Add Dependent</button>
                        <button type="button" className="btn btn-primary" data-bs-dismiss="modal" onClick={SubmitEmployee}>Save changes</button>
                    </div>
                </div>
            </div>
        </div>

        <div>
            <div className="modal fade" id={paycheckModalId} tabIndex="-1" aria-labelledby="add-employee-modal-label" aria-hidden="true">
                <div className="modal-dialog">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h1 className="modal-title fs-5" id="add-employee-modal-label">Paycheck</h1>
                            <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div className="modal-body">
                            <div className='row'>
                                <div className='col-6'>
                                    <label>Name: </label>
                                </div>
                                <div className='col-6'>
                                    <span>{lastName} , {firstName}</span>
                                </div>
                            </div>
                            <div className='row'>
                                <div className='col-6'>
                                    <label>Salary: </label>
                                </div>
                                <div className="col-6">
                                    <span id="idPayCheckSalary" ref={payCheckSalaryRef}></span>
                                </div>
                            </div>
                            <div className='row'>
                                <div className='col-6'>
                                    <label>Base Pay Per Paycheck: </label>
                                </div>
                                <div className="col-6">
                                    <span ref={basePayRef}></span>
                                </div>
                            </div>
                            <div className='row'>
                                <div className='col-6'>
                                    <label>Standard Benefit Deduction: </label>
                                </div>
                                <div className="col-6">
                                    <span ref={monthlyDeductionRef}>{currencyFormat(monthlyDeduction)}</span>
                                </div>
                            </div>
                            <div className='row'>
                                <div className='col-6'>
                                    <label>Dependent Deduction: </label>
                                </div>
                                <div className="col-6">
                                    <span ref={dependentDeductionRef}></span>
                                </div>
                            </div>
                            <div className='row'>
                                <div className='col-6'>
                                    <label>High Earner Deduction: </label>
                                </div>
                                <div className="col-6">
                                    <span ref={highEarnerDeductionRef}></span>
                                </div>
                            </div>
                            <div className='row'>
                                <div className='col-6'>
                                    <label>Old Dependent Deduction: </label>
                                </div>
                                <div className="col-6">
                                    <span ref={oldDependentDeductionRef}></span>
                                </div>
                            </div>
                            <div className='row'>
                                <div className='col-6'>
                                    <label>Paycheck Per Pay Period: </label>
                                </div>
                                <div className="col-6">
                                    <span ref={finalPayCheckValueRef}></span>
                                </div>
                            </div>
                        </div>
                        <div className='modal-footer'>
                            <button type="button" className="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    );
};

export default EmployeeListing;