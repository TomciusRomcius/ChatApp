resource "aws_ecs_cluster" "chatapp-cluster" {
  name = "chatapp-cluster"
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

resource "aws_ecs_service" "frontend" {
  cluster         = aws_ecs_cluster.chatapp-cluster.id
  name            = "frontend"
  task_definition = aws_ecs_task_definition.chatapp-frontend-task-definition.arn
  desired_count   = 1

  network_configuration {
    subnets          = [aws_subnet.chatapp-public.id]
    security_groups  = [aws_security_group.allow-all.id]
    assign_public_ip = true
  }

  launch_type = "FARGATE"
}
