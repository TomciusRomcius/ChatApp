

interface Result<TData, TError>
{
    errors: TError[];
    data: TData | null;
}