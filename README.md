# Task Management System API

A robust task management system built with .NET Core that provides CRUD operations for tasks and includes automatic status updates based on due dates.

## Features

- **Task Management**
  - Create tasks with title, description, due date (with time), and status
  - Update existing tasks
  - Delete tasks
  - Retrieve single task or list all tasks
  - Automatic status updates based on due dates

- **Task Statuses**
  - Pending
  - In Progress
  - Completed
  - Overdue

- **Automatic Status Updates**
  - Background service runs every 5 minutes
  - Automatically updates overdue tasks:
    - "Pending" → "Overdue" when due date passes
    - "In Progress" → "Completed" when due date passes

## Technical Stack

- **.NET Core**
- **Entity Framework Core**
- **SQLite Database**
- **xUnit** for testing
- **Swagger** for API documentation

## API Endpoints

### Tasks