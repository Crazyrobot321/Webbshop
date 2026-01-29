# Webshop Console Application

A console-based webshop built with **C#**, **Entity Framework Core**, and **Azure SQL Database**.

The project focuses on database design and usage, including relationships, constraints, LINQ queries, and raw SQL via Dapper.

## Features

- Product browsing with categories and suppliers
- Shopping cart and checkout flow
- Delivery and payment selection
- Orders and order items stored in the database
- 
## Tech Stack

- C# / .NET
- Entity Framework Core
- LINQ
- Dapper (raw SQL)
- SQL Server (Azure)

## Configuration

Create an `appsettings.json` and place it in \bin\Debug\net10.0 locally to run the application.

Template for appsettings.json:
'''
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING_HERE"
  }
}
'''

