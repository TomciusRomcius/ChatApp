resource "aws_ecr_repository" "chatapp-frontend" {
  name                 = "chatapp-frontend"
  image_tag_mutability = "MUTABLE"
}

resource "aws_ecr_repository" "chatapp-backend" {
  name                 = "chatapp-backend"
  image_tag_mutability = "MUTABLE"
}
