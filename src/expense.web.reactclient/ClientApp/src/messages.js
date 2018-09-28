export class Messages {
    static FIELD_LENGTH_MIN_ERROR = (fieldname, minlength) => `The ${fieldname} cannot be less than ${minlength} characters`
    static FIELD_LENGTH_MAX_ERROR = (fieldname, maxlength) => `The ${fieldname} cannot be more than ${maxlength} characters`
}

export class Constants {
    static TENANT_ID_FIELD_LABEL = "Tenant ID"
    static TENANT_ID_FIELD_KEY = "tenantId"

    static CODE_FIELD_LABEL = "Code"
    static CODE_FIELD_KEY = "code"

    static NAME_FIELD_LABEL = "Name"
    static NAME_FIELD_KEY = "name"

    static VALUE_FIELD_LABEL = "Value"
    static VALUE_FIELD_KEY = "value"

    static VALIDATION_STATE_KEY = "validationState"

    static SUCCESS = "success"

    static ERROR = "error"
}
