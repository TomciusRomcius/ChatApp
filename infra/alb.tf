resource "aws_acm_certificate" "chatapp-certificate" {
  domain_name       = "tomwpagency.com"
  validation_method = "DNS"
}

resource "aws_alb" "chatapp-alb" {
  name               = "chatapp-alb"
  internal           = false
  load_balancer_type = "application"
  # TODO: setup a more restrictive security group
  security_groups = [aws_security_group.allow-all.id]
  subnets         = [aws_subnet.chatapp-public.id, aws_subnet.chatapp-public-b.id]
}

resource "aws_alb_target_group" "chatapp-frontend" {
  name        = "chatapp-frontend"
  port        = 3000
  protocol    = "HTTP"
  vpc_id      = aws_vpc.chatapp-vpc.id
  target_type = "ip"
}

resource "aws_alb_target_group" "chatapp-backend" {
  name        = "chatapp-backend"
  port        = 8080
  protocol    = "HTTP"
  vpc_id      = aws_vpc.chatapp-vpc.id
  target_type = "ip"
}

resource "aws_alb_listener" "chatapp-alb-http-listener" {
  load_balancer_arn = aws_alb.chatapp-alb.arn
  port              = 443
  protocol          = "HTTPS"

  ssl_policy      = "ELBSecurityPolicy-2016-08"
  certificate_arn = aws_acm_certificate.chatapp-certificate.arn

  default_action {
    type             = "forward"
    target_group_arn = aws_alb_target_group.chatapp-frontend.arn
  }
}

resource "aws_alb_listener_rule" "chatapp-api-path" {
  listener_arn = aws_alb_listener.chatapp-alb-http-listener.arn
  priority     = 10

  action {
    type             = "forward"
    target_group_arn = aws_alb_target_group.chatapp-backend.arn
  }
  condition {
    path_pattern {
      values = ["/api/*"]
    }
  }
}
