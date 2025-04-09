data "aws_ecr_repository" "chatapp-frontend" {
  name = "chatapp-frontend"
}

data "aws_ecr_repository" "chatapp-backend" {
  name = "chatapp-backend"
}
