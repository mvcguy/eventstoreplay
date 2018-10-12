import React, { Component } from 'react';
import { Form, FormGroup, FormControl, Button,  ControlLabel } from 'react-bootstrap';

class ValueComment extends Component {
    
    render() {
        return (
            <div>
                <Form inline>
                    <h4>Comments</h4>
                    <FormGroup
                        controlId="value">
                        <ControlLabel >Guest: </ControlLabel>
                        <span style={{padding:'2px'}}></span>
                        <FormControl
                            style={{minWidth:'400px'}}
                            name={this.props.name}
                            onChange={this.props.onCommentChange}
                            value={this.props.commentText}
                            componentClass="textarea" placeholder="Add your thoughts" />
                            <span style={{padding:'2px'}}></span>
                            <Button
                            onClick={!this.props.isLoading ? this.props.handleSaveComment : (e) => {/*request in progress*/ }}
                            disabled={!this.props.isLoading ? false : true}
                            type="button" id="save_comment" bsStyle="primary">Add</Button>
                    </FormGroup>                    
                </Form>
            </div>
        );
    }
}

export default ValueComment;