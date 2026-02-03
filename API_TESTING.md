# API Testing Guide

## Testing with Swagger UI

1. Run the application:
```bash
dotnet run
```

2. Navigate to Swagger UI at `https://localhost:<port>/` or `http://localhost:<port>/`

3. The Swagger UI will display all available endpoints grouped by controller.

## Authentication Flow

### 1. Register a Customer
**Endpoint**: `POST /api/Auth/register`

**Request Body**:
```json
{
  "email": "customer@example.com",
  "password": "Password123!",
  "fullName": "John Doe",
  "phoneNumber": "+919876543210",
  "role": 1,
  "address": "123 Main St, Kochi",
  "cityId": 2
}
```

### 2. Register a Worker
**Endpoint**: `POST /api/Auth/register`

**Request Body**:
```json
{
  "email": "worker@example.com",
  "password": "Password123!",
  "fullName": "Jane Smith",
  "phoneNumber": "+919876543211",
  "role": 2,
  "address": "456 Worker Lane, Kochi",
  "cityId": 2,
  "latitude": 9.9312,
  "longitude": 76.2673,
  "hourlyRate": 500,
  "experienceYears": 5,
  "bio": "Experienced electrician",
  "serviceCategoryIds": [1, 4]
}
```

### 3. Login
**Endpoint**: `POST /api/Auth/login`

**Request Body**:
```json
{
  "email": "customer@example.com",
  "password": "Password123!"
}
```

**Response**: Returns JWT token and user details.

### 4. Authorize in Swagger
1. Copy the token from the login response
2. Click the "Authorize" button in Swagger UI
3. Enter: `Bearer <your-token>`
4. Click "Authorize"

## Customer Workflow

### 1. Create a Task
**Endpoint**: `POST /api/Customer/tasks`

**Request Body**:
```json
{
  "serviceCategoryId": 1,
  "title": "Fix electrical wiring",
  "description": "Need to fix faulty wiring in the living room",
  "address": "123 Main St, Kochi",
  "latitude": 9.9312,
  "longitude": 76.2673,
  "budget": 2000,
  "scheduledDate": "2026-02-10T10:00:00Z"
}
```

### 2. View My Tasks
**Endpoint**: `GET /api/Customer/tasks`

### 3. View Bids for a Task
**Endpoint**: `GET /api/Customer/tasks/{taskId}/bids`

### 4. Assign Worker to Task
**Endpoint**: `POST /api/Customer/tasks/{taskId}/assign`

**Request Body**:
```json
{
  "bidId": 1
}
```

### 5. Make Payment
**Endpoint**: `POST /api/Customer/tasks/{taskId}/payment`

**Request Body**:
```json
{
  "paymentMethod": "UPI",
  "transactionId": "TXN123456789"
}
```

### 6. Submit Review
**Endpoint**: `POST /api/Customer/tasks/{taskId}/review`

**Request Body**:
```json
{
  "rating": 5,
  "comment": "Excellent work!"
}
```

## Worker Workflow

### 1. View Nearby Tasks
**Endpoint**: `GET /api/Worker/tasks/nearby`

Returns tasks within 15km radius that match worker's skills.

### 2. Create a Bid
**Endpoint**: `POST /api/Worker/bids`

**Request Body**:
```json
{
  "taskId": 1,
  "proposedAmount": 1800,
  "estimatedHours": 3,
  "message": "I can complete this job efficiently with 5 years of experience."
}
```

### 3. View My Bids
**Endpoint**: `GET /api/Worker/bids`

### 4. View Active Task
**Endpoint**: `GET /api/Worker/tasks/active`

### 5. Start Task
**Endpoint**: `POST /api/Worker/tasks/{taskId}/start`

### 6. Complete Task
**Endpoint**: `POST /api/Worker/tasks/{taskId}/complete?isComplete=true`

## Admin Workflow

### 1. View All Users
**Endpoint**: `GET /api/Admin/users`

### 2. View All Tasks
**Endpoint**: `GET /api/Admin/tasks`

### 3. View Dashboard Stats
**Endpoint**: `GET /api/Admin/dashboard/stats`

### 4. Suspend User
**Endpoint**: `POST /api/Admin/users/{userId}/suspend`

**Request Body**:
```json
{
  "suspend": true
}
```

## Master Data

### Get Cities
**Endpoint**: `GET /api/MasterData/cities`

### Get Service Categories
**Endpoint**: `GET /api/MasterData/service-categories`

## User Roles

- **Customer**: `1`
- **Worker**: `2`
- **Admin**: `3`

## Task Status Flow

1. **Open** (1) - Task created, accepting bids
2. **Assigned** (2) - Worker assigned, not started yet
3. **InProgress** (3) - Work in progress
4. **Completed** (4) - Work completed successfully
5. **Incomplete** (5) - Work completed but requires revision

## Bid Status

1. **Pending** (1) - Bid submitted, awaiting response
2. **Accepted** (2) - Bid accepted, worker assigned
3. **Rejected** (3) - Bid rejected

## Payment Status

1. **Pending** (1) - Payment initiated but not completed
2. **Completed** (2) - Payment successful
3. **Failed** (3) - Payment failed
4. **Refunded** (4) - Payment refunded

## Important Notes

1. Workers can only have ONE active task at a time
2. Tasks can only be updated/deleted when in Open status
3. Bids can only be placed on Open tasks
4. Payment requires task to be in Completed status
5. Reviews require payment to be completed first
6. Revision requests are only for Incomplete tasks
7. Workers see only tasks within 15km radius with matching skills
8. All distances are calculated using Haversine formula
