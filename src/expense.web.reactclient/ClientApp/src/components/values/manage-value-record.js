//NOTE: this component can add and also update an existing record

import React, { Component } from 'react'
import { Prompt } from 'react-router';
import { Form, FormGroup, FormControl, Button, Col, ControlLabel, Alert } from 'react-bootstrap';
import { Messages, Constants } from '../../messages'

import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../../store/Values';
import ValidationSummary from '../validation-summary';
import ValueComment from './value-comment';
import CommentList from './comment-list';

class ManageValueRecord extends Component {
    constructor(props, context) {
        super(props, context);
        console.log('constructor');
        this.updateValueRecordState = this.updateValueRecordState.bind(this);
        this.handleSaveValue = this.handleSaveValue.bind(this);
        this.getSuccessClassName = this.getSuccessClassName.bind(this);
        this.backToList = this.backToList.bind(this);

        // comments
        this.handleSaveComment = this.handleSaveComment.bind(this);
        this.onCommentChange = this.onCommentChange.bind(this);

        this.state = {
            valueRecord: Object.assign({}, props.valueRecord),
            isLoading: props.isLoading,
            hasError: props.hasError,
            errorState: props.errorState,
            recordPersisted: props.recordPersisted,
            isDirty: props.isDirty,
            currentComment: {
                commentText: '',
            },
        };
    }


    getErrorClass(property) {
        return "hide error";
    }


    updateValueRecordState(event) {
        const field = event.target.name;
        let valueRecord = this.state.valueRecord;
        valueRecord[field] = event.target.value;
        this.setState({ valueRecord: valueRecord, isDirty: true, });
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
            currentComment: newProps.currentComment,
        };

        this.setState({
            valueRecord: newState.valueRecord,
            isLoading: newState.isLoading,
            hasError: newState.hasError,
            errorState: newState.errorState,
            recordPersisted: newState.recordPersisted,
            currentComment: newState.currentComment
        });
    }

    shouldBlockNavigation() {
        return (this.props.recordPersisted !== true && this.props.isLoading === true) || this.props.isDirty === true;
    }

    componentDidUpdate() {
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
            this.props.getValueById(this.props.paramsRecordId, this.props.paramsVersion);
        }
    }

    componentWillUnmount() {
        console.log('componentwillunmount');
        window.onbeforeunload = undefined
    }

    backToList(e) {
        this.props.history.push('/values-list');
    }

    // comments
    handleSaveComment(event) {
        console.log('saving comment...')
        this.props.addComment(this.state);
    }

    onCommentChange(event) {
        this.setState({ currentComment: { commentText: event.target.value } });
    }

    render() {
        return (
            <div>
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
                                disabled={!this.props.isLoading ? false : true}
                                type="button" id="submit" bsStyle="primary">Save</Button>
                            <span style={{ margin: '4px' }}></span>
                            <Button
                                onClick={this.backToList}
                                type="button" id="cancel" bsStyle="default">Back to list</Button>
                        </Col>
                        <Col xs={1}>
                        </Col>
                    </FormGroup>
                </Form >
                <ValueComment
                    name="unique_id"
                    onCommentChange={this.onCommentChange}
                    commentText={this.state.currentComment.commentText}
                    errorclass="hide error"
                    errorMessage=""
                    isLoading={this.props.isLoading}
                    handleSaveComment={this.handleSaveComment}
                />
                <CommentList comments={this.props.valueRecord.comments} />
            </div>
        );
    }
}

function mapStateToProps(state, ownProps) {

    var lifeValuesStore = state.lifeValues;

    console.log('mapStateToProps', state);

    var paramsRecordId = ownProps.match.params.recordId;
    var paramsVersion = ownProps.match.params.version;

    return {
        isLoading: lifeValuesStore.isLoading,
        valueRecord: lifeValuesStore.valueRecord,
        hasError: lifeValuesStore.hasError,
        errorState: lifeValuesStore.errorState,
        recordPersisted: lifeValuesStore.recordPersisted,
        paramsRecordId: paramsRecordId,
        paramsVersion: paramsVersion,
        isDirty: false,
        currentComment: lifeValuesStore.currentComment
    }
}

export default connect(mapStateToProps,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(ManageValueRecord);
