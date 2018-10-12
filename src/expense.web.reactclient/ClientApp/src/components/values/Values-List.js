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
        this.gotoNext = this.gotoNext.bind(this);
        this.gotoPrevious = this.gotoPrevious.bind(this);

        this.state = { currentPage: 1 };
    }
    componentWillMount() {
        this.props.getValuesList(this.state.currentPage);
    }

    addNewRecord(e) {
        this.props.addNewRecord();
        this.props.history.push('/manage-value-record');
    }

    gotoNext(event) {
        var gotoPage = this.state.currentPage + 1;
        if (gotoPage > this.props.valuesList.totalPages) {
            gotoPage = this.props.valuesList.totalPages;
        }

        this.setState({ currentPage: gotoPage });
        this.props.getValuesList(gotoPage);
    }

    gotoPrevious(event) {
        var gotoPage = this.state.currentPage - 1;
        if (gotoPage < 0) {
            gotoPage = 1;
        }

        this.setState({ currentPage: gotoPage });
        this.props.getValuesList(gotoPage);
    }

    mapModelToTableRow(model) {
        return (<tr key={model.id}>
            <td><Link to={'/manage-value-record/' + model.id}>{model.code}</Link></td>
            <td>{model.tenantId}</td>
            <td>{model.name}</td>
            <td>{model.value}</td>
        </tr>);
    }

    renderValuesTable() {
        return (
            <table className='table'>
                <thead>
                    <tr>
                        <th>Code</th>
                        <th>Tenant ID</th>
                        <th>Name</th>
                        <th>Value</th>
                    </tr>
                </thead>
                <tbody>
                    {this.props.valuesList.list.map(item => this.mapModelToTableRow(item))}
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td>
                            <Button bsStyle="link" onClick={this.gotoPrevious}
                                disabled={this.state.currentPage === 1}>
                                <span>{"<< Previous"}</span>
                            </Button>
                            <Button bsStyle="link" onClick={this.gotoNext}
                                disabled={this.state.currentPage === this.props.valuesList.totalPages}>
                                <span>{"Next >>"}</span>
                            </Button>
                        </td>
                    </tr>
                </tbody>
            </table>
        );
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
                {this.renderValuesTable()}
            </div>
        );
    }
}



export default connect(
    state => state.lifeValues,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(ValuesList);
