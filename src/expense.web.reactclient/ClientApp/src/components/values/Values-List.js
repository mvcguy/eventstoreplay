import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from './../../store/Values';
import ValidationSummary from './../validation-summary';
import { Link } from 'react-router-dom';
import { Button } from 'react-bootstrap';

class ValuesList extends Component {
  constructor(props, context) {
    super(props, context);
    this.addNewRecord = this.addNewRecord.bind(this);
  }
  componentWillMount() {
    this.props.getValuesList();
  }

  addNewRecord(e) {
    this.props.addNewRecord();
    this.props.history.push('/manage-value-record');
  }


  render() {
    return (
      <div>
        <h1>The Values of a successful life</h1>
        <p>This component demonstrates fetching data from the server and working with URL parameters.</p>
        <p>
          <span style={{ margin: '4px' }}></span>
          <Button
            onClick={this.addNewRecord}
            type="button"
            id="newRecord"
            bsStyle="default">Add new</Button>
        </p>
        <div style={{ "display": this.props.hasError ? "block" : "none" }} >
          <ValidationSummary errorState={this.props.errorState} />
        </div>
        {renderValuesTable(this.props)}
      </div>
    );
  }
}

function renderValuesTable(props) {
  return (
    <table className='table'>
      <thead>
        <tr>
          <th>Tenant ID</th>
          <th>Code</th>
          <th>Name</th>
          <th>Value</th>
        </tr>
      </thead>
      <tbody>
        {props.valuesList.map(item =>
          <tr key={item.id}>
            <td><Link to={'/manage-value-record/' + item.id}>{item.code}</Link></td>
            <td>{item.tenantId}</td>
            <td>{item.name}</td>
            <td>{item.value}</td>
          </tr>
        )}
      </tbody>
    </table>
  );
}

export default connect(
  state => state.lifeValues,
  dispatch => bindActionCreators(actionCreators, dispatch)
)(ValuesList);
