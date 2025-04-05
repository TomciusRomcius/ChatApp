data "aws_iam_policy_document" "ecs-instance-policy" {
  statement {
    sid     = ""
    actions = ["sts:AssumeRole"]
    effect  = "Allow"
    principals {
      type        = "Service"
      identifiers = ["ecs-tasks.amazonaws.com"]
    }
  }
}

resource "aws_iam_role" "ecs-task-exec-role" {
  name               = "ecs-task-exec-role"
  assume_role_policy = data.aws_iam_policy_document.ecs-instance-policy.json
}

resource "aws_iam_role_policy_attachment" "ecs_task_execution_policy" {
  role       = aws_iam_role.ecs-task-exec-role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}
