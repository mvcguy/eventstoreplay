import React, { Component } from 'react';
import { Button, Glyphicon } from 'react-bootstrap';
class CommentList extends Component {
    constructor(props, context) {
        super(props, context);
    }

    render() {
        return (
            <div  style={{ maxHeight: "300px", maxWidth: "600px", overflowY: "auto" }}>
                {
                    this.props.comments.map((value, index) => mapModelToCommentView(value))
                }
            </div>
        );
    }
}

function mapModelToCommentView(model) {
    return (
        <div key={model.id}>
            <span>{model.userName}</span>
            <div style={{
                border: '1px',
                width: '500px',
                background: 'lightgray',
                padding: '10px',
                borderRadius: '15px'
            }}>
                <span>{model.commentText}</span>
                <div>
                    <Button bsStyle="link">
                        <Glyphicon glyph="thumbs-up" />
                        <span>{model.likes}</span>
                    </Button>
                    <Button bsStyle="link">
                        <Glyphicon glyph="thumbs-down" />
                        <span>{model.dislikes}</span>
                    </Button>
                </div>
            </div>
        </div>
    )
}

export default CommentList;