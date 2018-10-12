import React, { Component } from 'react';
import { Button, Glyphicon } from 'react-bootstrap';
class CommentList extends Component {
    
    mapModelToCommentView(model) {
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
                        <Button bsStyle="link" onClick={() => { this.props.handleLikeComment(model) }}>
                            <Glyphicon glyph="thumbs-up" />
                            <span>{model.likes}</span>
                        </Button>
                        <Button bsStyle="link" onClick={() => { this.props.handleDislikeComment(model) }}>
                            <Glyphicon glyph="thumbs-down" />
                            <span>{model.dislikes}</span>
                        </Button>
                    </div>
                </div>
            </div>
        )
    }

    render() {
        return (
            <div style={{ maxHeight: "300px", maxWidth: "600px", overflowY: "auto" }}>
                {
                    this.props.comments.map((value, index) => this.mapModelToCommentView(value))
                }
            </div>
        );
    }
}



export default CommentList;