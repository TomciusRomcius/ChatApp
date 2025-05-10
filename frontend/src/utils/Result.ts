export interface Result<TData, TError> {
    data: TData | null;
    error: TError | null;
}
