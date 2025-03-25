# 📘 Address Book API - ASP.NET Core  

An API for managing user contacts using ASP.NET Core, integrated with authentication, email notifications, caching, and event-driven messaging.  

---

## ✅ Use Cases  

### **UC 1: Configure Database and Application Settings**  
- Define database connection in `appsettings.json`  
- Set up Entity Framework Core Migrations  
- Implement DbContext for database interaction  
- Configure Dependency Injection (DI) for database services  

---

### **UC 2: Implement Address Book API Controller**  
- Define RESTful Endpoints:  
  - `GET /api/addressbook` → Fetch all contacts  
  - `GET /api/addressbook/{id}` → Get contact by ID  
  - `POST /api/addressbook` → Add a new contact  
  - `PUT /api/addressbook/{id}` → Update contact  
  - `DELETE /api/addressbook/{id}` → Delete contact  
- Use `ActionResult` to return JSON responses  
- Test APIs using Postman or CURL  

---

### **UC 3: Implement Address Book Service Layer and API Documentation**  
- Create `IAddressBookService` interface  
- Implement `AddressBookService`:  
  - Move logic from controller to service layer  
  - Handle CRUD operations in business logic  
- Inject Service Layer using Dependency Injection  
- Document API using Swagger:  
  - Enable Swagger UI for API testing  
  - Define request/response models in Swagger  
  - Auto-generate API documentation  

---

### **UC 4: Implement User Registration & Login**  
- Create User Model & DTO  
- Implement Password Hashing using BCrypt  
- Generate JWT Token upon successful login  
- Store user data securely in MS SQL Database  
- Endpoints:  
  - `POST /api/auth/register` → Register new user  
  - `POST /api/auth/login` → Authenticate user and return JWT  

---

### **UC 5: Implement Forgot & Reset Password**  
- Generate password reset token using JWT  
- Send reset password email via SMTP  
- Verify the token and reset the password  
- Endpoints:  
  - `POST /api/auth/forgot-password` → Send password reset email  
  - `POST /api/auth/reset-password` → Reset password using token  

---

### **UC 6: Integrate Redis for Caching**  
- Store session data using Redis  
- Cache Address Book data for faster access  
- Improve performance and reduce database calls  

---

### **UC 7: Integrate RabbitMQ for Event-Driven Messaging**  
- Publish events using RabbitMQ when:  
  - A new user registers (Send email)  
  - A new contact is added to the Address Book  
- Consume messages asynchronously to handle background tasks  

---

### **UC 8: Test APIs Using NUnit**  
- Perform unit testing using NUnit  
- Test user authentication and authorization  
- Validate CRUD operations for Address Book  
- Ensure email sending, JWT generation, and Redis caching work correctly  

---

### **UC 9: Additional Features and Enhancements**  
- Implement Role-Based Access Control (RBAC)  
- Provide Admin access for managing user contacts  
- Secure API using JWT authentication  
- Enable logging for system monitoring and debugging  

---

## 🛠️ **Technologies Used**  
- **ASP.NET Core** for API Development  
- **Entity Framework Core** for Database Management  
- **MS SQL Server** as the Database  
- **JWT** for Secure Authentication  
- **RabbitMQ** for Event-Driven Messaging  
- **Redis** for Caching  
- **SMTP** for Email Services  
- **Swagger** for API Documentation  
- **NUnit** for Testing  

