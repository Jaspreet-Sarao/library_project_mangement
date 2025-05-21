## Library Management System

**What It Is**

A **database-driven web application** for managing library operations, including book inventory, member registrations, and borrowing records.

**What It's For**

**Librarians**: Track books, manage members, and automate overdue fees.

**Patrons**: Check book availability and borrowing history.

**Key Features**

**Automated Late Fees** – Triggers calculate fees when books are overdue.

**Real-Time Book Availability** – Views filter available books instantly.

**Duplicate Member Prevention** – Stored procedures validate new entries.
**Checkout & Return Tracking** – Records borrow/return dates with status updates.
**Reporting** – Lists overdue books and active checkouts.


**Database Schema (ERD)
Tables**
1. members
   member_id(PK)
   first_name(NN)
   last_name(NN)
   email(NN, Unique)
   phone (NN,Unique)
2. books
   book_id(PK)
   title(NN)
   author(NN)
   genre(NN)
   available(NN,Boolean)
3. borrowing_records
   record_id(PK)
   book_id(FK-> books)
   member_id(FK-> members)
   borrow_date(NN)
   due_date(NN)
   returned (NN, Boolean)
   late_fee(Decimal)

**Relationships**
**One-to-Many:**

A **member** can have multiple **borrowing_records.**
A **book** can appear in multiple **borrowing_records.**
   
