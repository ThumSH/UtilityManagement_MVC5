UTILITY MANAGEMENT SYSTEM (UMS) - SETUP GUIDE
===================================================

STEP 1: DATABASE SETUP (CRITICAL)
---------------------------------
1. Open SQL Server Management Studio (SSMS).
2. Connect to your local server (e.g., .\SQLEXPRESS or (localdb)\MSSQLLocalDB).
3. Create a new empty database named: UMS_DB
4. Open the file "UMS_Full_Database_Script.sql" included in this folder.
5. Execute the script to create all Tables, Views, and Procedures.

STEP 2: PROJECT SETUP
---------------------
1. Open "UtilityManagementSystem.sln" in Visual Studio 2022.
2. Open the "Web.config" file.
3. Look for the "connectionStrings" section (around line 15).
4. Check the "Data Source".
   - If you use SQL Express, keep it as: Data Source=.\SQLEXPRESS
   - If you use LocalDB, change it to: Data Source=(localdb)\MSSQLLocalDB
5. Go to Build > Rebuild Solution.
6. Press the Green Play Button to run.

LOGIN CREDENTIALS (Sample Data)
-------------------------------
- Admin:        admin / admin123
- Field Officer: reader1 / pass123
- Cashier:      cashier1 / pass123
- Manager:      manager1 / pass123