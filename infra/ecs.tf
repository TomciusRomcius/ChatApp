resource "aws_ecs_cluster" "chatapp-cluster" {
  name = "chatapp-cluster"
}

# secrets setup
resource "aws_secretsmanager_secret" "chatapp-secrets" {
  name = "chatapp-secrets"
}

resource "aws_iam_policy" "aws_secrets_policy" {
  name = "ecs-secrets-access"
  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "secretsmanager:GetSecretValue"
        ],
        Resource = [
          aws_secretsmanager_secret.chatapp-secrets.arn,
        ]
      }
    ]
  })
}

resource "aws_ecs_task_definition" "chatapp-frontend-task-definition" {
  network_mode             = "awsvpc"
  family                   = "chatapp-frontend"
  cpu                      = "1024"
  memory                   = "2048"
  requires_compatibilities = ["FARGATE"]
  execution_role_arn       = aws_iam_role.ecs-task-exec-role.arn

  container_definitions = jsonencode([
    {
      name      = "chatapp-frontend"
      image     = aws_ecr_repository.chatapp-frontend.repository_url
      essential = true
      environment = [
        {
          name  = "NEXT_PUBLIC_BACKEND_URL",
          value = "no url"
        }
      ]
      portMappings = [
        {
          containerPort = 3000
          hostPort      = 3000
          protocol      = "tcp"
        }
      ]

      logConfiguration = {
        logDriver = "awslogs"
        options = {
          awslogs-group         = "${aws_cloudwatch_log_group.chatapp.name}"
          awslogs-region        = "eu-west-1"
          awslogs-stream-prefix = "streaming"
        }
      }
    }
  ])
}


resource "aws_iam_role_policy_attachment" "secrets-attatchement" {
  role       = aws_iam_role.ecs-task-exec-role.name
  policy_arn = aws_iam_policy.aws_secrets_policy.arn
}

# task definitions
resource "aws_ecs_task_definition" "chatapp-backend-task-definition" {
  network_mode             = "awsvpc"
  family                   = "chatapp-backend"
  cpu                      = "1024"
  memory                   = "2048"
  requires_compatibilities = ["FARGATE"]
  execution_role_arn       = aws_iam_role.ecs-task-exec-role.arn

  container_definitions = jsonencode([
    {
      name      = "chatapp-backend"
      image     = aws_ecr_repository.chatapp-backend.repository_url
      essential = true
      environment = [
        {
          name  = "CA_OIDC_GOOGLE_CLIENT_ID",
          value = "178336905267-g69rdobhf6ivt0p5nq0lbnmec86r0tgu.apps.googleusercontent.com"
        },
        {
          name  = "CA_OIDC_GOOGLE_AUTHORITY",
          value = "https://accounts.google.com"
        },
      ]
      portMappings = [
        {
          containerPort = 3000
          hostPort      = 3000
          protocol      = "tcp"
        }
      ]

      logConfiguration = {
        logDriver = "awslogs"
        options = {
          awslogs-group         = "${aws_cloudwatch_log_group.chatapp.name}"
          awslogs-region        = "eu-west-1"
          awslogs-stream-prefix = "streaming"
        }
      }
    }
  ])
}

# services
resource "aws_ecs_service" "frontend" {
  cluster         = aws_ecs_cluster.chatapp-cluster.id
  name            = "frontend"
  task_definition = aws_ecs_task_definition.chatapp-frontend-task-definition.arn
  desired_count   = 0

  network_configuration {
    subnets          = [aws_subnet.chatapp-public.id]
    security_groups  = [aws_security_group.allow-all.id]
    assign_public_ip = true
  }
  launch_type = "FARGATE"
}

resource "aws_ecs_service" "backend" {
  cluster         = aws_ecs_cluster.chatapp-cluster.id
  name            = "backend"
  task_definition = aws_ecs_task_definition.chatapp-backend-task-definition.arn
  desired_count   = 0

  network_configuration {
    subnets          = [aws_subnet.chatapp-public.id]
    security_groups  = [aws_security_group.allow-all.id]
    assign_public_ip = true
  }

  launch_type = "FARGATE"
}
