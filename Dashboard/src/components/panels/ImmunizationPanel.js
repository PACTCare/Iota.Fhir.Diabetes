import React from 'react'

const ImmunizationPanel = ({ immunizations }) => {
    if (typeof(immunizations) === "undefined") {
        return null;
    }

    if (typeof(immunizations.entry) === "undefined") {
        return null;
    }

    return (
        <div className="ibox float-e-margins">
        <div className="ibox-title">
            <h5>Immunizations</h5>
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
                        immunizations.entry.map((immunization, i) => {
                            return (
                                <tr>
                                    <td>{ immunization.resource.vaccineCode.text }</td>
                                    <td>{ new Date(immunization.resource.date).toLocaleDateString() }</td>
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

export default ImmunizationPanel