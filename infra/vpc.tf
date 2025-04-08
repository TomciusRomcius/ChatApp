resource "aws_vpc" "chatapp-vpc" {
  cidr_block = "10.0.0.0/16"
}

resource "aws_subnet" "chatapp-public" {
  vpc_id            = aws_vpc.chatapp-vpc.id
  availability_zone = "eu-west-1a"
  cidr_block        = "10.0.0.0/24"
}

resource "aws_subnet" "chatapp-private" {
  vpc_id            = aws_vpc.chatapp-vpc.id
  availability_zone = "eu-west-1a"
  cidr_block        = "10.0.1.0/24"
}

resource "aws_internet_gateway" "chatapp-internet-gateway" {
  vpc_id = aws_vpc.chatapp-vpc.id
}

resource "aws_eip" "chatapp-nat-gateway-eip" {
  vpc = true
}

resource "aws_nat_gateway" "chatapp-nat-gateway" {
  subnet_id     = aws_subnet.chatapp-public.id
  allocation_id = aws_eip.chatapp-nat-gateway-eip.id
}

resource "aws_route_table" "public-route-table" {
  vpc_id = aws_vpc.chatapp-vpc.id
  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.chatapp-internet-gateway.id
  }
}

resource "aws_route_table" "private-route-table" {
  vpc_id = aws_vpc.chatapp-vpc.id
  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_nat_gateway.chatapp-nat-gateway.id
  }
}

resource "aws_route_table_association" "public" {
  route_table_id = aws_route_table.public-route-table.id
  subnet_id      = aws_subnet.chatapp-public.id
}

resource "aws_route_table_association" "private" {
  route_table_id = aws_route_table.private-route-table.id
  subnet_id      = aws_subnet.chatapp-private.id
}


resource "aws_service_discovery_private_dns_namespace" "chatapp_private" {
  vpc  = aws_vpc.chatapp-vpc.id
  name = "chatapp_private"
}
