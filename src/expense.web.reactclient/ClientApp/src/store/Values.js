const requestValuesType = 'REQUEST_VALUES';
const receiveValuesType = 'RECEIVE_VALUES';
const receiveValuesErrorType = 'RECEIVE_VALUES_ERROR'

const requestCreateValueType = 'REQUEST_CREATE_VALUE';
const receiveCreateValueType = 'RECEIVE_CREATE_VALUE';
const receiveCreateValueErrorType = 'RECEIVE_CREATE_VALUE_ERROR'

const initialState = {
    valuesListResponse: [],
    // TODO: Create unique flags/states for each action, 
    // otherwise it will be overwritten by another request
    isLoading: false,
    createValueResponse: {},
    hasError: false,
    errorState: {},
    action: '',
    recordPersisted: false,
};
const url = 'http://localhost:50178/api/values';

export const actionCreators = {
    getValuesList: () => async (dispatch, getState) => {

        dispatch({ type: requestValuesType, hasError: false });

        fetch(url).then(async (response) => {

            dispatch({
                type: receiveValuesType,
                valuesListResponse: await response.json(),
                hasError: false,
                errorState: {}
            });
        }).catch((error) => {
            dispatch({
                type: receiveValuesErrorType,
                valuesListResponse: [],
                hasError: true,
                errorState: { "Error": ["An unexpected error occurred while processing your request, please try again later!"] }
            })
        });

    },

    // NOTE: The create value can create a record, but also can modify an existing record
    createValue: (request) => async (dispatch, getState) => {
        console.log(request);
        dispatch({ type: requestCreateValueType, hasError: false });

        var model = {
            tenantId: {
                value: request.tenantId
            },
            code: {
                value: request.code
            },
            name: {
                value: request.name
            },
            value: {
                value: request.value
            },
            id: request.id ? request.id : '',
            version: {
                value: request.version
            }
        };

        var newUrl = request.id ? url + `/${request.id}` : url;
        var method = request.id ? 'PUT' : 'POST';

        fetch(newUrl, {
            method: method,
            headers: {
                "Content-Type": "application/json; charset=utf-8",
            },
            body: JSON.stringify(model)

        }).then(async (createValueResponse) => {
            var json = await createValueResponse.json();
            if (createValueResponse.ok === true) {
                dispatch({
                    type: receiveCreateValueType,
                    createValueResponse: json,
                    hasError: false,
                    errorState: {}
                })
            }
            else if (createValueResponse.ok === false && createValueResponse.status === 400) {
                dispatch({
                    type: receiveCreateValueErrorType,
                    createValueResponse: {},
                    hasError: true,
                    errorState: json
                })
            }
        }).catch((errorResponse) => {
            dispatch({
                type: receiveCreateValueErrorType,
                createValueResponse: {},
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
            newState = state || initialState;
            break;
    }
    return newState;
};
