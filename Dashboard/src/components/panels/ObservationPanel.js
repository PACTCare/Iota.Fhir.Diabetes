import React from 'react'

const ObservationPanel = ({ observations }) => {
    if (typeof(observations) === "undefined") {
        return null;
    }

    if (typeof(observations.entry) === "undefined") {
        return null;
    }

    return (
        <div className="ibox float-e-margins">
            <div className="ibox-title">
                <h5>Observations</h5>
            </div>
            <div className="ibox-content">
                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th>Description</th>
                            <th>Issued</th>
                            <th>Value</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            observations.entry.map((observation, i) => {
                                return (
                                    <tr>
                                        <td>{ observation.resource.code.text }</td>
                                        <td>{ new Date(observation.resource.issued).toLocaleDateString() }</td>
                                        {
                                            observation.resource.valueQuantity !== undefined ?
                                                <td>{ observation.resource.valueQuantity.value.toFixed(2) + ' ' + observation.resource.valueQuantity.unit }</td>
                                            : <td>{ observation.resource.valueCodeableConcept !== undefined ? observation.resource.valueCodeableConcept.coding[0].display : null }</td>
                                        }

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

export default ObservationPanel