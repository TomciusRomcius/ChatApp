interface CircleIconProps {
    svgClassName?: string;
    circleStroke: string;
    size: number;
    color: string;
}

export default function CircleIcon(props: CircleIconProps) {
    return (
        <svg
            width={props.size}
            height={props.size}
            viewBox="0 0 24 24"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
            preserveAspectRatio="xMidYMid meet"
            className={`fill-current ${props.svgClassName ?? ""}`}
        >
            <circle cx="12" cy="12" r="10" fill={props.color} strokeWidth="2" />
        </svg>
    );
}
