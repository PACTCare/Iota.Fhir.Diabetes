import React from 'react'

const ConditionPanel = ({ conditions }) => {
    if (typeof(conditions) === "undefined") {
        return null;
    }

    if (typeof(conditions.entry) === "undefined") {
        return null;
    }

    return (
        <div className="ibox float-e-margins">
        <div className="ibox-title">
            <h5>Conditions</h5>
        </div>
        <div className="ibox-content">
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th>Description</th>
                        <th>Issued</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        conditions.entry.map((condition, i) => {
                            return (
                                <tr>
                                    <td>{ condition.resource.code.text }</td>
                                    <td>{ new Date(condition.resource.assertedDate).toLocaleDateString() }</td>
                                </tr>
                            )
                        })
                    }
                </tbody>
            </table>
        </div>
    </div>
    );
}

export default ConditionPanel