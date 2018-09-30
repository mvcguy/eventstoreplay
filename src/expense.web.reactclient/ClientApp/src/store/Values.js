const requestValuesType = 'REQUEST_VALUES';
const receiveValuesType = 'RECEIVE_VALUES';
const receiveValuesErrorType = 'RECEIVE_VALUES_ERROR'

const requestCreateValueType = 'REQUEST_CREATE_VALUE';
const receiveCreateValueType = 'RECEIVE_CREATE_VALUE';
const receiveCreateValueErrorType = 'RECEIVE_CREATE_VALUE_ERROR'

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
        value: ''
    },
    hasError: false,
    errorState: {},
    recordPersisted: false,
};
// const url = 'http://localhost:50178/api/values';
const url = 'http://localhost:5000/api/values';

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
    };

    return model;
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

        dispatch({ type: requestCreateValueType, hasError: false });

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
                    errorState: {}
                })
            }
            else if (createValueResponse.ok === false && createValueResponse.status === 400) {
                dispatch({
                    type: receiveCreateValueErrorType,
                    valueRecord: {},
                    hasError: true,
                    errorState: json
                })
            }
        }).catch((errorResponse) => {
            dispatch({
                type: receiveCreateValueErrorType,
                valueRecord: {},
                hasError: true,
                errorState: { "Error": ["An unexpected error occurred while processing your request, please try again later!"] }
            });
        });
    }
};

export const reducer = (state, action) => {

    var newState = { ...state, ...action };

    switch (action.type) {
        case requestValuesType:
            newState.isLoading = true;
            break;
        case receiveValuesType:
            newState.isLoading = false;
            break;
        case receiveValuesErrorType:
            newState.isLoading = false;
            break;
        case requestCreateValueType:
            newState.isLoading = true;
            break;
        case receiveCreateValueType:
            newState.isLoading = false;
            newState.recordPersisted = true;
            break;
        case receiveCreateValueErrorType:
            newState.isLoading = false;
            break;
        default:
            newState = { ...initialState, state };
            break;
    }
    return newState;
};
