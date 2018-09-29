//NOTE: this component can add and also update an existing record

import React, { Component } from 'react'
import { Form, FormGroup, FormControl, Button, Col, ControlLabel, Alert } from 'react-bootstrap';
import { Messages, Constants } from './../../messages'

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from './../../store/Values';
import ValidationSummary from '../validation-summary';

class CreateValue extends Component {
    constructor(props, context) {
        super(props, context);

        this.handleTenantIDChange = this.handleTenantIDChange.bind(this);
        this.handleCodeChange = this.handleCodeChange.bind(this);
        this.handleNameChange = this.handleNameChange.bind(this);
        this.handleValueChange = this.handleValueChange.bind(this);
        this.handleSaveValue = this.handleSaveValue.bind(this);

        this.state = {
            tenantId: '',
            code: '',
            name: '',
            value: '',
            id: null,
            version: null,
            persistSuccessClass: "hide"
        };

        this.state[Constants.VALIDATION_STATE_KEY] = {};
    }

    // validates the minimum length of the field
    getValidationState(property, length) {

        var newValidationState = { ...this.state[Constants.VALIDATION_STATE_KEY] };
        newValidationState[property] = length < 2 ? Constants.ERROR : Constants.SUCCESS;
        return newValidationState;
    }

    getErrorClass(property) {
        return this.state.validationState[property] === Constants.ERROR ? "show error" : "hide error";
    }

    canSave() {
        return this.state.validationState[Constants.TENANT_ID_FIELD_KEY] === Constants.SUCCESS &&
            this.state.validationState[Constants.CODE_FIELD_KEY] === Constants.SUCCESS &&
            this.state.validationState[Constants.NAME_FIELD_KEY] === Constants.SUCCESS &&
            this.state.validationState[Constants.VALUE_FIELD_KEY] === Constants.SUCCESS;
    }

    handleTenantIDChange(event) {

        var newState = {};
        newState.tenantId = event.target.value;
        newState[Constants.VALIDATION_STATE_KEY] = this.getValidationState(Constants.TENANT_ID_FIELD_KEY, event.target.value.length)

        this.setState(newState);

    }

    handleCodeChange(event) {
        var newState = {};
        newState.code = event.target.value;
        newState[Constants.VALIDATION_STATE_KEY] = this.getValidationState(Constants.CODE_FIELD_KEY, event.target.value.length)

        this.setState(newState);
    }

    handleNameChange(event) {
        var newState = {};
        newState.name = event.target.value;
        newState[Constants.VALIDATION_STATE_KEY] = this.getValidationState(Constants.NAME_FIELD_KEY, event.target.value.length)

        this.setState(newState);
    }

    handleValueChange(event) {
        var newState = {};
        newState.value = event.target.value;
        newState[Constants.VALIDATION_STATE_KEY] = this.getValidationState(Constants.VALUE_FIELD_KEY, event.target.value.length)

        this.setState(newState);
    }

    handleSaveValue() {
        this.props.createValue(this.state);
    }

    componentWillReceiveProps(props) {

        var newState = {};

        if (props.createValueResponse && props.createValueResponse.version)
            newState = { id: props.createValueResponse.id, version: props.createValueResponse.version.value }

        // show/hide success message
        var cls = "hide";
        if (props.isLoading !== true && props.recordPersisted === true) {
            cls = "show";
        }

        newState = { ...newState, persistSuccessClass: cls }
        this.setState(newState);

    }


    render() {
        return (
            <Form horizontal>
                <h2>Create a new value</h2>
                <div style={{ "display": this.props.hasError ? "block" : "none" }} >
                    <ValidationSummary errorState={this.props.errorState} />
                </div>

                <Alert bsStyle="success" className={this.state.persistSuccessClass}>
                    Record is persister successfully
                </Alert>

                <FormGroup
                    validationState={this.state.validationState[Constants.TENANT_ID_FIELD_KEY]}
                    controlId="tenantID">
                    <Col componentClass={ControlLabel} sm={2}>
                        {Constants.TENANT_ID_FIELD_LABEL}
                    </Col>
                    <Col sm={10}>
                        <FormControl
                            readOnly={this.props.createValueResponse.id}
                            onChange={this.handleTenantIDChange}
                            value={this.state.tenantId}
                            type="number" placeholder={Constants.TENANT_ID_FIELD_LABEL} />
                        <span className={this.getErrorClass(Constants.TENANT_ID_FIELD_KEY)}>
                            {Messages.FIELD_LENGTH_MIN_ERROR(Constants.TENANT_ID_FIELD_LABEL, 2)}
                        </span>
                    </Col>

                </FormGroup>

                <FormGroup
                    validationState={this.state.validationState[Constants.CODE_FIELD_KEY]}
                    controlId="code">
                    <Col componentClass={ControlLabel} sm={2}>
                        {Constants.CODE_FIELD_LABEL}
                    </Col>
                    <Col sm={10}>
                        <FormControl
                            readOnly={this.props.createValueResponse.id}
                            onChange={this.handleCodeChange}
                            value={this.state.code}
                            type="text" placeholder={Constants.CODE_FIELD_LABEL} />
                        <span className={this.getErrorClass(Constants.CODE_FIELD_KEY)}>
                            {Messages.FIELD_LENGTH_MIN_ERROR(Constants.CODE_FIELD_LABEL, 2)}
                        </span>
                    </Col>
                </FormGroup>

                <FormGroup
                    validationState={this.state.validationState[Constants.NAME_FIELD_KEY]}
                    controlId="name">
                    <Col componentClass={ControlLabel} sm={2}>
                        {Constants.NAME_FIELD_LABEL}
                    </Col>
                    <Col sm={10}>
                        <FormControl
                            onChange={this.handleNameChange}
                            value={this.state.name}
                            type="text" placeholder={Constants.NAME_FIELD_LABEL} />
                        <span className={this.getErrorClass(Constants.NAME_FIELD_KEY)}>
                            {Messages.FIELD_LENGTH_MIN_ERROR(Constants.NAME_FIELD_LABEL, 2)}
                        </span>
                    </Col>
                </FormGroup>

                <FormGroup
                    validationState={this.state.validationState[Constants.VALUE_FIELD_KEY]}
                    controlId="value">
                    <Col componentClass={ControlLabel} sm={2}>
                        {Constants.VALUE_FIELD_LABEL}
                    </Col>
                    <Col sm={10}>
                        <FormControl
                            onChange={this.handleValueChange}
                            value={this.state.value}
                            componentClass="textarea" placeholder={Constants.VALUE_FIELD_LABEL} />

                        <span className={this.getErrorClass(Constants.VALUE_FIELD_KEY)}>
                            {Messages.FIELD_LENGTH_MIN_ERROR(Constants.VALUE_FIELD_LABEL, 2)}
                        </span>
                    </Col>
                </FormGroup>

                <FormGroup>
                    <Col smOffset={2} xs={10}>
                        <Button
                            onClick={!this.props.isLoading ? this.handleSaveValue : (e) => {/*request in progress*/ }}
                            disabled={this.canSave() && !this.props.isLoading ? false : true}
                            type="button" id="submit" bsStyle="primary">Save</Button>
                        <span style={{ margin: '4px' }}></span>
                        <Button type="button" id="cancel" bsStyle="warning">Cancel</Button>
                    </Col>
                    <Col xs={1}>
                    </Col>
                </FormGroup>
            </Form >);
    }
}

export default connect(
    state => state.lifeValues,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(CreateValue);
