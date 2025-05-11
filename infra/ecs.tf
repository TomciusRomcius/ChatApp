variable "chatapp-domain" {
  type    = string
  default = "tomwpagency.com"
}

resource "aws_ecs_cluster" "chatapp-cluster" {
  name = "chatapp-cluster"
}

# secrets setup
data "aws_secretsmanager_secret" "chatapp-secrets" {
  name = "chatapp-secrets"
}

# used for integration testing to make sure that dotnet app can retrieve secrets
data "aws_secretsmanager_secret" "test-secrets" {
  name = "test-secrets"
}

resource "aws_secretsmanager_secret_version" "test-secrets-value" {
  secret_id = data.aws_secretsmanager_secret.test-secrets.id

  secret_string = jsonencode({
    KEY1 = "Key 1"
    KEY2 = "Key 2"
  })
}

data "aws_secretsmanager_secret_version" "chatapp-secrets" {
  secret_id = data.aws_secretsmanager_secret.chatapp-secrets.id
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
          data.aws_secretsmanager_secret.chatapp-secrets.arn,
        ]
      }
    ]
  })
}

# task definitions
resource "aws_ecs_task_definition" "chatapp-backend-task-definition" {
  network_mode             = "awsvpc"
  family                   = "chatapp-backend"
  cpu                      = "1024"
  memory                   = "2048"
  requires_compatibilities = ["FARGATE"]
  execution_role_arn       = aws_iam_role.ecs-task-exec-role.arn
  task_role_arn            = aws_iam_role.ecs-task-exec-role.arn

  container_definitions = jsonencode([
    {
      name      = "chatapp-backend"
      image     = data.aws_ecr_repository.chatapp-backend.repository_url
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
        {
          name  = "CA_FRONTEND_URL"
          value = "https://${var.chatapp-domain}"
        },
        {
          name  = "CA_MSSQL_HOST"
          value = "mssql,1433"
        },
        {
          name  = "Logging__EventLog__LogLevel__Default"
          value = "Information"
        }
      ]
      portMappings = [
        {
          containerPort = 8080
          hostPort      = 8080
          protocol      = "tcp"
          name          = "backend"
          appProtocol   = "http"
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

resource "aws_ecs_task_definition" "chatapp_mssql_task_definition" {
  network_mode             = "awsvpc"
  family                   = "chatapp-msqql"
  cpu                      = "1024"
  memory                   = "2048"
  requires_compatibilities = ["FARGATE"]
  execution_role_arn       = aws_iam_role.ecs-task-exec-role.arn

  container_definitions = jsonencode([
    {
      name      = "chatapp-mssql"
      image     = "mcr.microsoft.com/mssql/server:2022-preview-ubuntu-22.04"
      essential = true
      environment = [
        {
          name  = "ACCEPT_EULA"
          value = "Y"
        },
        {
          name  = "MSSQL_PID"
          value = "Developer"
        },
        {
          # TODO: not ideal, pass secret in a safer way
          name  = "MSSQL_SA_PASSWORD"
          value = jsondecode(data.aws_secretsmanager_secret_version.chatapp-secrets.secret_string)["CA_MSSQL_PASSWORD"]
        }
      ]
      portMappings = [
        {
          containerPort = 1433
          hostPort      = 1433
          protocol      = "tcp"
          name          = "mssql"
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
      image     = data.aws_ecr_repository.chatapp-frontend.repository_url
      essential = true
      environment = [
        {
          name  = "NEXT_PUBLIC_BACKEND_URL",
          value = "https://${var.chatapp-domain}/api"
        },
        {
          name  = "HOSTNAME"
          value = "0.0.0.0"
        }
      ]
      portMappings = [
        {
          containerPort = 3000
          hostPort      = 3000
          protocol      = "tcp"
          name          = "frontend"
          appProtocol   = "http"
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

resource "local_file" "backend_task_definition" {
  content = jsonencode({
    containerDefinitions    = jsondecode(aws_ecs_task_definition.chatapp-backend-task-definition.container_definitions)
    family                  = aws_ecs_task_definition.chatapp-backend-task-definition.family
    cpu                     = aws_ecs_task_definition.chatapp-backend-task-definition.cpu
    memory                  = aws_ecs_task_definition.chatapp-backend-task-definition.memory
    networkMode             = aws_ecs_task_definition.chatapp-backend-task-definition.network_mode
    volumes                 = aws_ecs_task_definition.chatapp-backend-task-definition.volume
    requiresCompatibilities = aws_ecs_task_definition.chatapp-backend-task-definition.requires_compatibilities
    taskRoleArn             = aws_ecs_task_definition.chatapp-backend-task-definition.task_role_arn
    executionRoleArn        = aws_ecs_task_definition.chatapp-backend-task-definition.execution_role_arn
  })
  filename = "${path.module}/.aws/backend_task_definition.json"
}

resource "local_file" "frontend_task_definition" {
  content = jsonencode({
    containerDefinitions    = jsondecode(aws_ecs_task_definition.chatapp-frontend-task-definition.container_definitions)
    family                  = aws_ecs_task_definition.chatapp-frontend-task-definition.family
    cpu                     = aws_ecs_task_definition.chatapp-frontend-task-definition.cpu
    memory                  = aws_ecs_task_definition.chatapp-frontend-task-definition.memory
    networkMode             = aws_ecs_task_definition.chatapp-frontend-task-definition.network_mode
    volumes                 = aws_ecs_task_definition.chatapp-frontend-task-definition.volume
    requiresCompatibilities = aws_ecs_task_definition.chatapp-frontend-task-definition.requires_compatibilities
    taskRoleArn             = aws_ecs_task_definition.chatapp-backend-task-definition.task_role_arn
    executionRoleArn        = aws_ecs_task_definition.chatapp-backend-task-definition.execution_role_arn
  })
  filename = "${path.module}/.aws/frontend_task_definition.json"
}

resource "aws_iam_role_policy_attachment" "secrets-attatchement" {
  role       = aws_iam_role.ecs-task-exec-role.name
  policy_arn = aws_iam_policy.aws_secrets_policy.arn
}

# services
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

  load_balancer {
    target_group_arn = aws_alb_target_group.chatapp-frontend.arn
    container_name   = "chatapp-frontend"
    container_port   = 3000
  }

  launch_type = "FARGATE"
}

resource "aws_ecs_service" "mssql" {
  cluster         = aws_ecs_cluster.chatapp-cluster.id
  name            = "mssql"
  task_definition = aws_ecs_task_definition.chatapp_mssql_task_definition.arn
  desired_count   = 1

  network_configuration {
    subnets          = [aws_subnet.chatapp-public.id]
    security_groups  = [aws_security_group.allow-all.id]
    assign_public_ip = true
  }

  service_connect_configuration {
    enabled   = true
    namespace = aws_service_discovery_http_namespace.chatapp-private.arn
    service {
      discovery_name = "mssql"
      port_name      = "mssql"
      client_alias {
        dns_name = "mssql"
        port     = 1433
      }
    }
  }

  launch_type = "FARGATE"
}


resource "aws_ecs_service" "backend" {
  cluster         = aws_ecs_cluster.chatapp-cluster.id
  name            = "backend"
  task_definition = aws_ecs_task_definition.chatapp-backend-task-definition.arn
  desired_count   = 1

  network_configuration {
    subnets          = [aws_subnet.chatapp-public.id]
    security_groups  = [aws_security_group.allow-all.id]
    assign_public_ip = true
  }

  load_balancer {
    target_group_arn = aws_alb_target_group.chatapp-backend.arn
    container_name   = "chatapp-backend"
    container_port   = 8080
  }

  service_connect_configuration {
    enabled   = true
    namespace = aws_service_discovery_http_namespace.chatapp-private.arn
    service {
      discovery_name = "backend"
      port_name      = "backend"
      client_alias {
        dns_name = "backend"
        port     = 8080
      }
    }
  }

  enable_execute_command = "true"

  launch_type = "FARGATE"
}
