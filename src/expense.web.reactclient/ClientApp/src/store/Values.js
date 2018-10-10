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
    addComment: function (request) {
        var action = async function (dispatch, getState) {
            dispatch({
                type: requestAddCommentType,
                hasError: false,
                valueRecord: request.valueRecord,
                currentComment: request.currentComment,
            });

            var parentId = request.valueRecord.id;
            var parentVersion = request.valueRecord.version;
            var comment = {
                commentText: { value: request.currentComment.commentText },
                userName: { value: 'shahid.ali' },
                tenantId: { value: request.valueRecord.tenantId },
                parentVersion: { value: parentVersion }
            };
            var newUrl = commentsUrl + '/' + parentId;

            fetch(newUrl, {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json; charset=utf-8",
                },
                body: JSON.stringify(comment)

            }).then(async (addCommentResponse) => {
                var json = await addCommentResponse.json();
                if (addCommentResponse.ok === true) {

                    var currentRecord = request.valueRecord;

                    // BUG: Do not modify the request, rather use a reducer to push comment to list
                    var commentUiModel = toCommentUIModel(json);
                    currentRecord.version = commentUiModel.parentVersion;
                    currentRecord.comments.push(commentUiModel);

                    dispatch({
                        type: receiveAddCommentType,
                        currentComment: {},
                        valueRecord: currentRecord,
                        hasError: false,
                        recordPersisted: true,
                        errorState: {}
                    })
                }
                else {
                    dispatch({
                        type: receiveAddCommentErrorType,
                        currentComment: request.currentComment,
                        valueRecord: request.valueRecord,
                        hasError: true,
                        errorState: json
                    })
                }
            }).catch((errorResponse) => {
                dispatch({
                    type: receiveAddCommentErrorType,
                    currentComment: request.currentComment,
                    valueRecord: request.valueRecord,
                    hasError: true,
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
            newState.isLoading = true;
            newState.recordPersisted = false;
            break;
        case receiveValuesType:
        case receiveValuesErrorType:
        case receiveCreateValueType:
        case receiveCreateValueErrorType:
        case receiveValueByIdType:
        case receiveValueByIdErrorType:
        case cancelChangeType:
        case addNewRecordType:
        case receiveAddCommentType:
        case receiveAddCommentErrorType:
            newState.isLoading = false;
            break;
        default:
            newState = { ...state };
            break;
    }
    return newState;
};
