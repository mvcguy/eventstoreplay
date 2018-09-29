import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from './../../store/Values';
import ValidationSummary from './../validation-summary';

class ValuesList extends Component {
  componentWillMount() {
    this.props.getValuesList();
  }

  render() {
    return (
      <div>
        <h1>The Values of a successful life</h1>
        <p>This component demonstrates fetching data from the server and working with URL parameters.</p>
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
        {props.valuesListResponse.map(item =>
          <tr key={item.id}>
            <td>{item.tenantId}</td>
            <td>{item.code}</td>
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
