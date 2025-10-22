select * from Employee

INSERT INTO Employee (EmpName, Gender, Username, Password, PAN, PhoneNum, Address, DeptID, EmpType, ManagerID)
VALUES ('Bawandeep Singh', 'M', 'bawandeep', HASHBYTES('SHA2_256', 'Bawan@123'), 'PQRS5678', '9876543210', 'Bangalore, India', 'DEPT01', 'M', NULL);
GO

Select * from Customer
Select* from Department
Select * from Employee
Update Employee
Set DeptID = 'MNGR'
where EmpId = 'EMP0002'



-- name, gender, PAN, Phone number, Address, in Employee Table
-- Department - Update accordingly in department Table
-- Display Emmployee ID, username, Password


--  validations
--Name - only character, Phone Number - Unique and 10 digits, DOB ( Before Age 18)
--pan card - 8 Chars ( ABCD1234), Check if Pan already exists in customer and employee table