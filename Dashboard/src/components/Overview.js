import React from 'react'

const Overview = ({reports}) => {
    return(
        <div>
            <div className="row" style={{height: 80}}></div>
            <div className="row border-bottom">
                <div className="col-lg-6">
                    <div className="row">
                        <div className="col-lg-1"></div>
                        <div className="col-lg-11"><h2>Time in Range Overview (last 15 days)</h2></div>
                    </div>
                </div>
            </div>
            <div className="row" style={{height: 50}}></div>
            {
                reports !== null ?
                    reports.map((report, i) => {
                        return (
                            <div className="row" key={i}>
                                <div className="col-lg-1">
                                </div> 
                                <div className="col-lg-2">
                                    <h5>{ report.patientName }</h5>
                                </div>
                                <div className="col-lg-8">
                                    <table>
                                        <tbody>
                                            <tr>
                                                {
                                                    report.report.map((dailyReport, i) => {
                                                        return(
                                                            <td style={{border: "1px solid"}} key={i}>
                                                                <div style={
                                                                    { backgroundColor: colorResolver(dailyReport.averageGlucose), height: "18px", width: "52px" }
                                                                    } />
                                                            </td>
                                                        )
                                                    })
                                                }
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        )
                    }) :
                    null
            }
            <div className="row border-bottom" style={{height: 50}}></div>
            <div className="row" style={{height: 15}}></div>
            <div className="row">
                <div className="col-lg-1"></div>
                <div className="col-lg-11"><h3>Color indicated average glucose levels</h3></div>
            </div>
            <div className="row" style={{height: 5}}></div>
            <div className="row">
                <div className="col-lg-1"></div>
                <div className="col-lg-11">
                    <div className="row">
                        <div className="col-md-1" style={
                                { backgroundColor: "#ff2d2d", height: "18px", width: "52px" }
                                } />
                        <div className="col-md-1">> 250 mg/dL</div>    
                    </div>
                    <div className="row" style={{height: 2}}></div>
                    <div className="row">
                        <div className="col-md-1" style={
                                { backgroundColor: "#ff502d", height: "18px", width: "52px" }
                                } />
                        <div className="col-md-1">> 230 mg/dL</div>   
                    </div>
                    <div className="row" style={{height: 2}}></div>
                    <div className="row">
                        <div className="col-md-1" style={
                                { backgroundColor: "#ffc12d", height: "18px", width: "52px" }
                                } />
                        <div className="col-md-1">> 190 mg/dL</div>   
                    </div>
                    <div className="row" style={{height: 2}}></div>
                    <div className="row">
                        <div className="col-md-1" style={
                                { backgroundColor: "#3cff2d", height: "18px", width: "52px" }
                                } />
                        <div className="col-md-1">> 150 mg/dL</div>   
                    </div>
                    <div className="row" style={{height: 2}}></div>
                    <div className="row">
                        <div className="col-md-1" style={
                                { backgroundColor: "#03d91d", height: "18px", width: "52px" }
                                } />
                        <div className="col-md-1">> 110 mg/dL</div>   
                    </div>
                    <div className="row" style={{height: 2}}></div>
                    <div className="row">
                        <div className="col-md-1" style={
                                { backgroundColor: "#7b53ff", height: "18px", width: "52px" }
                                } />
                        <div className="col-md-1">&lt; 110 mg/dL</div>   
                    </div>
                    <div className="row" style={{height: 2}}></div>
                    <div className="row">
                        <div className="col-md-1" style={
                                { backgroundColor: "#3f3bf2", height: "18px", width: "52px" }
                                } />
                        <div className="col-md-1">&lt; 70 mg/dL</div>   
                    </div>
                </div>
            </div>
        </div>
    )
}

const colorResolver = (glucoseValue) => {
    if (glucoseValue > 250) {
        return '#ff2d2d';
    }

    if (glucoseValue > 230) {
        return '#ff502d';
    }

    if (glucoseValue > 190) {
        return '#ffc12d';
    }

    if (glucoseValue > 150) {
        return '#3cff2d';
    }

    if (glucoseValue > 110) {
        return '#03d91d';
    }

    if (glucoseValue > 70) {
        return '#7b53ff';
    }

    if (glucoseValue > 50) {
        return '#3f3bf2';
    }

    return '#0600ff';
}

export default Overview;