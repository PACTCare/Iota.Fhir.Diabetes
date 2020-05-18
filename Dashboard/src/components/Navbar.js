import React from 'react'

const Navbar = ({patients, onPatientClick, selectedMenu, selectOverview, selectMenuAction}) => {
    return(
        <nav className="navbar-default navbar-static-side">
            <div className="sidebar-collapse">
                <ul className="nav metismenu" id="side-menu">
                <li className="nav-header">
                        <div className="dropdown profile-element">
                        <span>
                            <img alt="" className="img-circle" src="/logo.png" height="72" width="72" />
                            </span>
                                <span className="clear"> <span className="block m-t-xs"> <strong className="font-bold">Dr. Mary Jane</strong>
                                </span> <span className="text-muted text-xs block" >Diabetes Specialist </span></span>
                        </div>
                    </li>
                    <li className={selectedMenu === 'Overview' ? 'active' : ''}>
                        <a onClick={() => selectOverview(patients) }><i className="fa fa-th-large"></i> <span className="nav-label">Overview</span></a>
                    </li>
                    <li className={selectedMenu === 'Patients' ? 'active' : ''}>
                        <a onClick={() => selectMenuAction('Patients') }><i className="fa fa-th-large"></i> <span className="nav-label">Patients</span></a>
                        { renderPatients(patients, onPatientClick, selectedMenu) }
                    </li>
                </ul>
            </div>
        </nav>
    );
}

const renderPatients = (patients, onPatientClick, selectedMenu) => {
    if (selectedMenu !== 'Patients') {
        return null;
    }

    return (
        <ul className="nav nav-second-level collapse in">
        {
            patients.entry.map((patient, i) => {
                return (
                    <li key={i} onClick={() => onPatientClick(patient)}>
                        <a>
                            {patient.resource.name[0].family + ', ' + patient.resource.name[0].given[0]}
                        </a>
                    </li>
                )
            })
        }
        </ul>
    );
}

export default Navbar