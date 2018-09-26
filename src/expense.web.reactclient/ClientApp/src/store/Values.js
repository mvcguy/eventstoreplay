const requestValuesType = 'REQUEST_VALUES';
const receiveValuesType = 'RECEIVE_VALUES';
const initialState = { lifeValues: [], isLoading: false };

export const actionCreators = {
    requestLifeValues: () => async (dispatch, getState) => {

        dispatch({ type: requestValuesType });

        const url = 'http://localhost:5000/api/values';
        const response = await fetch(url);
        
        const lifeValues = await response.json();
        console.log(lifeValues);
        dispatch({ type: receiveValuesType, lifeValues });
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    if (action.type === requestValuesType) {
        return {
            ...state,
            isLoading: true
        };
    }

    if (action.type === receiveValuesType) {
        return {
            ...state,
            lifeValues: action.lifeValues,
            isLoading: false
        };
    }

    return state;
};
