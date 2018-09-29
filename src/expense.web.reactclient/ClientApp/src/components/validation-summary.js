import React, { Component } from 'react';
import { Alert } from 'react-bootstrap';


class ValidationSummary extends Component {
    getErrorMessage(modelStateItem) {
        return (
            modelStateItem.map((value, key) => <li key={key}>{value}</li>)
        );
    }

    getErrorMessages() {
        let errorState = this.props.errorState;
        if (!errorState) return;

        return (

            Object.keys(errorState).map((keyName, keyIndex) =>
                <ul key={keyName}>
                    <li>{keyName}</li>
                    <ul>{this.getErrorMessage(errorState[keyName])}</ul>
                </ul>)
        );
    }

    render() {
        return (
            <Alert bsStyle="danger">
                <h4>Oh snap! You got an error!</h4>
                <div>
                    {this.getErrorMessages()}
                </div>
            </Alert>
        );
    }
}

export default ValidationSummary