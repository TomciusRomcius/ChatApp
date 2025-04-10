terraform {
  backend "s3" {
    bucket = "tomas-chatapp-terraform-state"
    key    = "state/terraform.tfstate"
    region = "eu-north-1"
  }
  required_providers {
    aws = {
      source  = "hashicorp/aws",
      version = "~> 4.16"
    }
  }
}

provider "aws" {
  region = "eu-west-1"
}
