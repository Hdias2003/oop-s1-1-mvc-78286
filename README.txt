============================================================
oop-s1-1-mvc-78286 - LIBRARY MANAGEMENT SYSTEM
============================================================

PROJECT OVERVIEW
----------------
This is a  Library Management System built with 
ASP.NET Core 8.0 MVC for the frist assessment of OOP. The application handles book catalogs, 
member registrations, and a synchronized loaning system with 
automated status tracking.

CORE FEATURES
-------------

A) Admin-only Role Management page (required)
Create a page /Admin/Roles that allows:
• List roles
• Create role (textbox + button)
• Delete role

B) CRUD + simple workflows
• Books: list + create + edit + delete
• Members: list + create + edit + delete
• Loans:
o Create a loan (choose member + choose available book)
o Mark returned (set ReturnedDate)
o Prevent lending a book that is already on an active loan (ReturnedDate
null)

C) Fake/seed data
On first run (or via migration/seed):
• 20 books
• 10 members
• 15 loans (some returned, some active, some overdue)

D) Searching + filtering (mandatory, on one page)
On Books index page implement:
• Search by Title or Author (contains)
• Filter by Category (dropdown)
• Filter by Availability (All / Available / On loan)
• Sorting optional (Title A–Z)

E) CI workflow

F)Minimum tests (xUnit)

---------------
GETTING STARTED
---------------
1. Clone the repository.

2. run terminal lines 

tools > nuget packet manager

Add-Migration InitialLibrarySetup
Update-Database


3. Run application
4. Navigate to '/seed-library' to populate the database and 
   create the Admin account.
5. admin login details:
   - Email: admin@library.com
   - Password: Admin123!
