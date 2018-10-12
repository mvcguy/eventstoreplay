const requestValuesType = 'REQUEST_VALUES';
const receiveValuesType = 'RECEIVE_VALUES';
const receiveValuesErrorType = 'RECEIVE_VALUES_ERROR';

const requestValueByIdType = 'REQUEST_VALUE_BY_ID';
const receiveValueByIdType = 'RECEIVE_VALUE_BY_ID';
const receiveValueByIdErrorType = 'RECEIVE_VALUE_BY_ID_ERROR';

const requestCreateValueType = 'REQUEST_CREATE_VALUE';
const receiveCreateValueType = 'RECEIVE_CREATE_VALUE';
const receiveCreateValueErrorType = 'RECEIVE_CREATE_VALUE_ERROR';

const requestAddCommentType = 'REQUEST_ADD_COMMENT';
const receiveAddCommentType = 'RECEIVE_ADD_COMMENT';
const receiveAddCommentErrorType = 'RECEIVE_ADD_COMMENT_ERROR';


const requestLikeCommentType = 'REQUEST_LIKE_COMMENT';
const receiveLikeCommentType = 'RECEIVE_LIKE_COMMENT';
const receiveLikeCommentErrorType = 'RECEIVE_LIKE_COMMENT_ERROR';

const requestDislikeCommentType = 'REQUEST_DISLIKE_COMMENT';
const receiveDislikeCommentType = 'RECEIVE_DISLIKE_COMMENT';
const receiveDislikeCommentErrorType = 'RECEIVE_DISLIKE_COMMENT_ERROR';

const cancelChangeType = 'CANCEL_CHANGES';
const addNewRecordType = 'ADD_NEW_RECORD';

const initialState = {
    valuesList: [],
    // TODO: Create unique flags/states for each action, 
    // otherwise it will be overwritten by another request
    isLoading: false,
    valueRecord: {
        id: null,
        version: null,
        tenantId: '',
        code: '',
        name: '',
        value: '',
        comments: []
    },
    hasError: false,
    errorState: {},
    recordPersisted: false,
    currentComment: {
        commentText: '',
        likes: 0,
        dislikes: 0,
        tenantId: 0,
        parentVersion: 0,
        parentId: null,
        id: null,
    }
};
// const url = 'http://localhost:50178/api/values';
const url = 'http://localhost:5000/api/values';
const commentsUrl = 'http://localhost:5000/api/valuecomment';

function toServerModel(uiModel) {
    var model = {
        tenantId: { value: uiModel.tenantId },
        code: { value: uiModel.code },
        name: { value: uiModel.name },
        value: { value: uiModel.value },
        id: uiModel.id ? uiModel.id : '',
        version: { value: uiModel.version }
    };

    return model;
}

function toUiModel(serverModel) {
    var model = {
        id: serverModel.id,
        version: serverModel.version != null ? serverModel.version.value : null,
        tenantId: serverModel.tenantId != null ? serverModel.tenantId.value : null,
        code: serverModel.code != null ? serverModel.code.value : null,
        name: serverModel.name != null ? serverModel.name.value : null,
        value: serverModel.value != null ? serverModel.value.value : null,
        comments: serverModel.comments.map((value, key) => toCommentUIModel(value))
    };

    return model;
}

function toCommentUIModel(model) {
    return {
        id: model.id,
        parentId: model.parentId,
        parentVersion: model.parentVersion.value,
        tenantId: model.tenantId.value,
        commentText: model.commentText.value,
        userName: model.userName.value,
        likes: model.likes.value,
        dislikes: model.dislikes.value
    };
}

function toCommentServerModel(model) {
    return {
        commentText: { value: model.commentText },
        userName: { value: 'shahid.ali' },
        tenantId: { value: model.tenantId },
        parentId: model.parentId,
        parentVersion: { value: model.parentVersion }
    };
}

export const actionCreators = {
    getValuesList: () => async (dispatch, getState) => {

        dispatch({ type: requestValuesType, hasError: false });

        fetch(url).then(async (response) => {
            var list = await response.json();
            dispatch({
                type: receiveValuesType,
                valuesList: list.map((item) => toUiModel(item)),
                hasError: false,
                errorState: {}
            });
        }).catch((error) => {
            dispatch({
                type: receiveValuesErrorType,
                valuesList: [],
                hasError: true,
                errorState: { "Error": ["An unexpected error occurred while processing your request, please try again later!"] }
            })
        });

    },

    // NOTE: The action can create a record, but also can modify an existing record
    addOrUpdateValueRecord: (request) => async (dispatch, getState) => {

        dispatch({ type: requestCreateValueType, hasError: false, valueRecord: request });

        var newUrl = request.id ? url + `/${request.id}` : url;
        var method = request.id ? 'PUT' : 'POST';

        fetch(newUrl, {
            method: method,
            headers: {
                "Content-Type": "application/json; charset=utf-8",
            },
            body: JSON.stringify(toServerModel(request))

        }).then(async (createValueResponse) => {
            var json = await createValueResponse.json();
            if (createValueResponse.ok === true) {
                dispatch({
                    type: receiveCreateValueType,
                    valueRecord: toUiModel(json),
                    hasError: false,
                    recordPersisted: true,
                    errorState: {}
                })
            }
            else {
                dispatch({
                    type: receiveCreateValueErrorType,
                    valueRecord: request,
                    hasError: true,
                    errorState: json
                })
            }
        }).catch((errorResponse) => {
            dispatch({
                type: receiveCreateValueErrorType,
                valueRecord: request,
                hasError: true,
                errorState: { "Error": ["An unexpected error occurred while processing your request, please try again later!"] }
            });
        });
    },

    getValueById: function (id, version) {
        var action = async function (dispatch, getState) {
            dispatch({ type: requestValueByIdType, hasError: false, valueRecord: initialState.valueRecord });

            var newUrl = url;
            if (id && version)
                newUrl = url + `/${id}/${version}`
            else
                newUrl = url + `/${id}`

            fetch(newUrl).then(async (response) => {
                var result = await response.json();
                if (response.ok === true) {
                    dispatch({
                        type: receiveValueByIdType,
                        valueRecord: toUiModel(result),
                        hasError: false,
                        errorState: {}
                    });
                }
                else {
                    dispatch({
                        type: receiveValueByIdErrorType,
                        valueRecord: initialState.valueRecord,
                        hasError: true,
                        errorState: result
                    })
                }

            }).catch((error) => {
                dispatch({
                    type: receiveValueByIdErrorType,
                    valueRecord: initialState.valueRecord,
                    hasError: true,
                    errorState: { "Error": ["An unexpected error occurred while processing your request, please try again later!"] }
                })
            });
        };

        return action;
    },
    cancelChanges: function () {
        var action = async function (dispatch, getState) {
            dispatch({ type: cancelChangeType, ...initialState });
        };

        return action;
    },
    addNewRecord: function () {
        var action = async function (dispatch, getState) {
            dispatch({ type: addNewRecordType, ...initialState });
        };

        return action;
    },
    addComment: function (model) {

        var action = async function (dispatch, getState) {

            dispatch({ type: requestAddCommentType });
            var newUrl = commentsUrl + '/' + model.parentId;
            fetch(newUrl, {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json; charset=utf-8",
                },
                body: JSON.stringify(toCommentServerModel(model))

            }).then(async (addCommentResponse) => {
                var json = await addCommentResponse.json();
                if (addCommentResponse.ok === true) {
                    var commentUiModel = toCommentUIModel(json);
                    dispatch({ type: receiveAddCommentType, updatedComment: commentUiModel })
                }
                else {
                    dispatch({ type: receiveAddCommentErrorType, errorState: json })
                }
            }).catch((errorResponse) => {
                dispatch({
                    type: receiveAddCommentErrorType,
                    errorState: { "Error": ["An unexpected error occurred while processing your request, please try again later!"] }
                });
            });
        };

        return action;
    },
    respondComment: function (model, respondType) {
        var action = async function (dispatch, getState) {

            var reqType = requestLikeCommentType;
            var recErrorType = receiveLikeCommentErrorType;
            var recType = receiveLikeCommentType;

            if (respondType === "dislike") {
                reqType = requestDislikeCommentType;
                recErrorType = receiveDislikeCommentErrorType;
                recType = receiveDislikeCommentType;
            }

            var serAction = respondType==="dislike"?"/DislikeCommentAction/":"/LikeCommentAction/";

            dispatch({ type: reqType });

            var newUrl = commentsUrl + serAction + model.parentId + "/" + model.id;
            fetch(newUrl, {
                method: 'PUT',
                headers: {
                    "Content-Type": "application/json; charset=utf-8",
                },
                body: JSON.stringify(toCommentServerModel(model))

            }).then(async (addCommentResponse) => {
                var json = await addCommentResponse.json();
                if (addCommentResponse.ok === true) {
                    dispatch({ type: recType, updatedComment: toCommentUIModel(json), })
                }
                else {
                    dispatch({ type: recErrorType, errorState: json, })
                }
            }).catch((errorResponse) => {
                dispatch({
                    type: recErrorType,
                    errorState: { "Error": ["An unexpected error occurred while processing your request, please try again later!"] }
                });
            });


        };

        return action;
    }
};

export const reducer = (state = initialState, action) => {

    var newState = { ...state, ...action };

    switch (action.type) {
        case requestValuesType:
        case requestCreateValueType:
        case requestValueByIdType:
        case requestAddCommentType:
        case requestLikeCommentType:
        case requestDislikeCommentType:
            newState.isLoading = true;
            newState.recordPersisted = false;
            newState.hasError = false;
            break;

        case receiveCreateValueType:
            newState.isLoading = false;
            newState.recordPersisted = true;
            newState.hasError = false;
            break;

        case receiveValuesType:
        case receiveValueByIdType:
            newState.isLoading = false;
            newState.recordPersisted = false;
            newState.hasError = false;
            break;

        case receiveAddCommentType:
        case receiveLikeCommentType:
        case receiveDislikeCommentType:
            newState.isLoading = false;
            newState.recordPersisted = false;
            newState.hasError = false;
            var xIndex = -1;

            newState.valueRecord.comments.forEach((comment, index) => {
                if (comment.id === newState.updatedComment.id) {
                    xIndex = index;
                    return;
                }
            });

            // push the updated comment to the list
            if (xIndex !== -1) {
                newState.valueRecord.comments[xIndex] = newState.updatedComment;
            }
            else {
                newState.valueRecord.comments.push(newState.updatedComment);
            }

            // update the parent version
            newState.valueRecord.version = newState.updatedComment.parentVersion

            break;

        case cancelChangeType:
        case addNewRecordType:
            newState.isLoading = false;
            newState.recordPersisted = false;
            newState.hasError = false;
            break;

        case receiveValuesErrorType:
        case receiveCreateValueErrorType:
        case receiveValueByIdErrorType:
        case receiveAddCommentErrorType:
        case receiveLikeCommentErrorType:
        case receiveDislikeCommentErrorType:
            newState.isLoading = false;
            newState.recordPersisted = false;
            newState.hasError = true;
            break;

        default:
            newState = { ...state };
            break;
    }
    return newState;
};
