//NOTE: this component can add and also update an existing record

import React, { Component } from 'react'
import { Prompt } from 'react-router';
import { Form, FormGroup, FormControl, Button, Col, ControlLabel, Alert } from 'react-bootstrap';
import { Messages, Constants } from '../../messages'

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../../store/Values';
import ValidationSummary from '../validation-summary';

class ManageValueRecord extends Component {
    constructor(props, context) {
        super(props, context);
        console.log('constructor');
        this.updateValueRecordState = this.updateValueRecordState.bind(this);
        this.handleSaveValue = this.handleSaveValue.bind(this);
        this.getSuccessClassName = this.getSuccessClassName.bind(this);
        this.cancelChanges = this.cancelChanges.bind(this);

        this.state = {
            valueRecord: Object.assign({}, props.valueRecord),
            isLoading: props.isLoading,
            hasError: props.hasError,
            errorState: props.errorState,
            recordPersisted: props.recordPersisted,
            validationState: {},
            isDirty: false,
        };
    }

    // validates the minimum length of the field
    getValidationState(property, length) {

        var newValidationState = { ...this.state.validationState };
        newValidationState[property] = length < 2 ? Constants.ERROR : Constants.SUCCESS;
        return newValidationState;
    }

    getErrorClass(property) {
        return this.state.validationState[property] === Constants.ERROR ? "show error" : "hide error";
    }

    canSave() {
        return true;
        return this.state.validationState[Constants.TENANT_ID_FIELD_KEY] === Constants.SUCCESS &&
            this.state.validationState[Constants.CODE_FIELD_KEY] === Constants.SUCCESS &&
            this.state.validationState[Constants.NAME_FIELD_KEY] === Constants.SUCCESS &&
            this.state.validationState[Constants.VALUE_FIELD_KEY] === Constants.SUCCESS;
    }

    updateValueRecordState(event) {
        const field = event.target.name;
        let valueRecord = this.state.valueRecord;

        valueRecord[field] = event.target.value;
        let validationState = this.getValidationState(field, event.target.value.length)

        this.setState({ valueRecord: valueRecord, validationState: validationState, isDirty: true, });
    }

    handleSaveValue() {
        this.props.addOrUpdateValueRecord(this.state.valueRecord);
    }

    componentWillReceiveProps(newProps) {
        console.log('componentWillReceiveProps', newProps);

        var newState = {
            valueRecord: Object.assign({}, newProps.valueRecord),
            isLoading: newProps.isLoading,
            hasError: newProps.hasError,
            errorState: newProps.errorState,
            recordPersisted: newProps.recordPersisted,
        };

        this.setState({
            valueRecord: newState.valueRecord,
            isLoading: newState.isLoading,
            hasError: newState.hasError,
            errorState: newState.errorState,
            recordPersisted: newState.recordPersisted,
        });
    }

    shouldBlockNavigation() {
        return !this.props.recordPersisted === true && this.props.isLoading === true || this.state.isDirty;
    }

    componentDidUpdate() {
        console.log('componentdidupdate');
        if (this.shouldBlockNavigation()) {
            window.onbeforeunload = () => true
        } else {
            window.onbeforeunload = undefined
        }
    }

    getSuccessClassName() {
        var className = this.props.recordPersisted === true
            && this.props.isLoading === false
            && this.props.hasError === false
            ? "show"
            : "hide"
        return className;
    }

    componentWillMount() {
        // when we are redirected with an id, fetch that record
        console.log('componentwillmount')
        if (this.props.paramsRecordId) {
            this.props.getValueById(this.props.paramsRecordId);
        }
    }

    cancelChanges(e) {
        this.props.history.push('/values-list');
    }

    render() {
        return (
            <Form horizontal>
                <h2>Create/update a value record</h2>
                <div style={{ "display": this.props.hasError ? "block" : "none" }} >
                    <ValidationSummary errorState={this.props.errorState} />
                </div>

                <Alert bsStyle="success" className={this.getSuccessClassName()}>
                    Record is persisted successfully
                </Alert>
                <Prompt
                    key='block-nav'
                    when={this.shouldBlockNavigation()}
                    message='You have unsaved changes, are you sure you want to leave?'
                />

                <FormGroup
                    validationState={this.state.validationState[Constants.TENANT_ID_FIELD_KEY]}
                    controlId="tenantID">
                    <Col componentClass={ControlLabel} sm={2}>
                        {Constants.TENANT_ID_FIELD_LABEL}
                    </Col>
                    <Col sm={10}>
                        <FormControl
                            name={Constants.TENANT_ID_FIELD_KEY}
                            readOnly={this.state.valueRecord.id}
                            onChange={this.updateValueRecordState}
                            value={this.state.valueRecord.tenantId}
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
                            name={Constants.CODE_FIELD_KEY}
                            readOnly={this.state.valueRecord.id}
                            onChange={this.updateValueRecordState}
                            value={this.state.valueRecord.code}
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
                            name={Constants.NAME_FIELD_KEY}
                            onChange={this.updateValueRecordState}
                            value={this.state.valueRecord.name}
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
                            name={Constants.VALUE_FIELD_KEY}
                            onChange={this.updateValueRecordState}
                            value={this.state.valueRecord.value}
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
                        <Button
                            onClick={this.cancelChanges}
                            type="button" id="cancel" bsStyle="default">Back to list</Button>
                    </Col>
                    <Col xs={1}>
                    </Col>
                </FormGroup>
            </Form >);
    }
}

function mapStateToProps(state, ownProps) {

    var lifeValuesStore = state.lifeValues;
    console.log('mapStateToProps');

    var paramsRecordId = ownProps.match.params.recordId;

    return {
        isLoading: lifeValuesStore.isLoading,
        valueRecord: lifeValuesStore.valueRecord,
        hasError: lifeValuesStore.hasError,
        errorState: lifeValuesStore.errorState,
        recordPersisted: lifeValuesStore.recordPersisted,
        paramsRecordId: paramsRecordId
    }
}

export default connect(mapStateToProps,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(ManageValueRecord);
