# SimpleCSV
![SimpleCSV](https://i.imgur.com/pteCC44.png)

# About
SimpleCSV is a Windows Form app written in VB.Net to bulk import .csv and .xlsx files into a SQL Server database. 

# Features
* Supports .csv and .xlsx files
* Can bulk import 400,000+ rows
* Dynamically add what tables to import to via the TableNamesList table
* Can store filenames of the files being imported
* Recognizes key constraints and will fire triggers
* Developer mode, which allows bulk imports to a test enviornment

# Setup
You will need a table in your database named TableNamesList. View the script.sql in the ExampleDB folder to get a better understanding of how it works. This table will fill the list for which tables can be imported to.

You will also need to setup your project settings to include your database connection strings and Dev password.
