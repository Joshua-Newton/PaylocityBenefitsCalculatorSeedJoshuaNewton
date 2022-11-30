const PaycheckModal = (props) => {
    return (
        <div>
            <div className="modal fade" id={props.paycheckModalId} tabIndex="-1" aria-labelledby="add-employee-modal-label" aria-hidden="true">
                <div className="modal-dialog">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h1 className="modal-title fs-5" id="add-employee-modal-label">Paycheck</h1>
                            <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div className="modal-body">
                            <div className='row'>
                                <div className='col-6'>
                                    <label>Salary: </label>
                                </div>
                                <div className="col-6">
                                    <span>{props.salary}</span>
                                </div>
                            </div>
                            <div className='row'>
                                <div className='col-6'>
                                    <label>Base Pay Per Paycheck: </label>
                                </div>
                                <div className="col-6">
                                    <span>{props.basePay}</span>
                                </div>
                            </div>
                            <div className='row'>
                                <div className='col-6'>
                                    <label>Standard Benefit Deduction: </label>
                                </div>
                                <div className="col-6">
                                    <span>{props.monthlyDeduction}</span>
                                </div>
                            </div>
                            <div className='row'>
                                <div className='col-6'>
                                    <label>Dependent Deduction: </label>
                                </div>
                                <div className="col-6">
                                    <span>{props.dependentDeduction}</span>
                                </div>
                            </div>
                            <div className='row'>
                                <div className='col-6'>
                                    <label>High Earner Deduction: </label>
                                </div>
                                <div className="col-6">
                                    <span>{props.highEarnerDeduction}</span>
                                </div>
                            </div>
                            <div className='row'>
                                <div className='col-6'>
                                    <label>Old Dependent Deduction: </label>
                                </div>
                                <div className="col-6">
                                    <span>{props.oldDependentDeduction}</span>
                                </div>
                            </div>
                            <div className='row'>
                                <div className='col-6'>
                                    <label>Final Paycheck: </label>
                                </div>
                                <div className="col-6">
                                    <span>{props.finalPayCheckValue}</span>
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
    );
};


export default PaycheckModal;