import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { Link } from 'react-router-dom';
import { actionCreators } from '../store/Values';

class ValuesList extends Component {
  componentWillMount() {
    // This method runs when the component is first added to the page   
    this.props.requestLifeValues();
  }

//   componentWillReceiveProps(nextProps) {
//     // This method runs when incoming props (e.g., route params) change
//     this.props.requestLifeValues();
//   }

  render() {
    return (
      <div>
        <h1>Life Values</h1>
        <p>This component demonstrates fetching data from the server and working with URL parameters.</p>
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
        {props.lifeValues.map(lifeValue =>
          <tr key={lifeValue.id}>
            <td>{lifeValue.tenantId}</td>
            <td>{lifeValue.code}</td>
            <td>{lifeValue.name}</td>
            <td>{lifeValue.value}</td>
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
