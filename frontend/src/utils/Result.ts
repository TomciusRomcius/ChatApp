export interface Result<TData, TError> {
    data: TData;
    error: TError;
    didSucceed: boolean;
}
