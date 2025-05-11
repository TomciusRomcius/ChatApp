# A Real-time Chat App

### Features
* Register and log in using email and password or OpenID(currently only Google provider).
* Add and remove friends.
* Create and delete chat rooms; invite friends to join rooms.
* Send real-time messages to individual friends or within chat rooms.

### Start in development mode
  #### Database
  From the root directory: <br>
  ```cd backend && docker compose up -d```
  #### Backend
  Trust and generate a TLS certificate if you don't have one already: <br>
  ```dotnet dev-certs https --trust``` <br>
  Run: <br>
  ```cd backend && dotnet watch --project ChatApp.Presentation --launch-profile https``` <br>
  #### Frontend
  From the root directory: <br>
  ```cd frontend && npm i && npm run dev:https```

### Run tests
* To run the tests, run from backend directory:
```
dotnet watch --project ChatApp.Presentation
```
**Ensure Docker is running. Integration tests requiring AWS Secrets Manager will fail unless a secret is properly configured (set up guide at the bottom).**

### Tech-stack
* Frontend: Next.js (leveraged for SSR and optimized JS bundle sizes).
* Backend: ASP.NET Core (.NET 9).
* Database: Microsoft SQL Server.

### Design
* Developed using clean architecture with a minor exception: .NET IdentityUser is referenced in the Domain layer.
* Used a code-first approach to database design using EF Core.
* Implemented authentication and authorization using Identity API.
* Integrated OIDC via direct HTTP communication with provider servers.
* Implemented real-time notifications using WebSockets and developed a background task queue to process sending WebSocket messages to clients without blocking the main thread. 
* Set up testing using xUnit, Moq, and TestContainers(for testing database operations).

### Cloud infrastructure
* Hosted the application on AWS and deployed using the IaC tool Terraform.
* Configured a remote Terraform backend with AWS S3 for state management.
* Designed a secure network architecture with ECS containers in a private subnet, exposing services via an Application Load Balancer with enforced HTTPS.
* Integrated Cloudflare as a CDN for improved performance and security.
* Implemented end-to-end HTTPS encryption.
* Built a CI/CD pipeline to automate deployments to Amazon ECR and ECS.

### AWS secrets manager setup for integration tests
* Run ```aws configure``` and login to your AWS account
#### Linux
```aws secretsmanager create-secret --name test-secrets --secret-string "{\"KEY1\":\"Key1\",\"KEY2\":\"Key2\"}"```
#### Windows
```aws secretsmanager create-secret --name test-secrets --secret-string '{\"KEY1\":\"Key1\",\"KEY2\":\"Key2\"}'```

