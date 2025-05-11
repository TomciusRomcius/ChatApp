export default function ErrorMessage(props: { message: string }) {
    if (props.message) {
        return (
            <small className="text-base text-red-500">{props.message}</small>
        );
    }
}
