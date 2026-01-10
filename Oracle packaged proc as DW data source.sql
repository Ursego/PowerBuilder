-- This undocumented PowerBuilder feature may be useful only for developers working with Oracle databases.

-- When you select "Stored Procedure" as the data source for your DataWindow, a dropdown lists all available stored procedures in the database.
-- Unfortunately, this list includes only standalone procedures — it doesn't show those defined inside packages.
-- To use a packaged stored procedure, you'll have to trick PowerBuilder a little.

-- Let's say you want to use my_package.my_proc. Here's what you need to do.

-- Step 1: Create a temporary standalone procedure with the same name and arguments as the real packaged procedure, but with no logic — just use NULL; as the body.

-- Step 2: In the DataWindow Painter, select this temporary standalone procedure from the dropdown as the data source (you may need to reconnect to the database if it doesn't appear).

-- Step 3: Save and close the DataWindow.

-- Step 4: Right-click the DataWindow and select "Edit Source".

-- Step 5: Find the occurrence of my_proc and add my_package. in front of it (with the dot), so it becomes my_package.my_proc.

-- Step 6: Save the DataWindow. You've just done the impossible!

-- Step 7: Delete the temporary standalone procedure.

-- You can use the same trick for the procedures used for insert, update, and delete operations (DataWindow Painter > menu "Rows" > "Stored Procedure Update").

-- Here is a sample package showing procs for all the CRUD operations:

CREATE OR REPLACE PACKAGE pkg_employee IS
    c_date_mask CONSTANT VARCHAR2(20) := 'YYYY MM DD';

    PROCEDURE upsert_employee(
        i_emp_id     IN NUMBER,
        i_first_name IN VARCHAR2,
        i_last_name  IN VARCHAR2,
        i_hire_date  IN VARCHAR2,  -- YYYY MM DD
        i_salary     IN NUMBER
    );

    PROCEDURE select_employee(
        i_emp_id IN NUMBER
    );

    PROCEDURE delete_employee(
        i_emp_id IN NUMBER
    );
END pkg_employee;
/

CREATE OR REPLACE PACKAGE BODY pkg_employee IS
    PROCEDURE upsert_employee(
        i_emp_id     IN NUMBER,
        i_first_name IN VARCHAR2,
        i_last_name  IN VARCHAR2,
        i_hire_date  IN VARCHAR2,
        i_salary     IN NUMBER
    ) IS
    BEGIN
        UPDATE employee
           SET first_name = i_first_name,
               last_name  = i_last_name,
               hire_date  = TO_DATE(i_hire_date, c_date_mask),
               salary     = i_salary
         WHERE emp_id = i_emp_id;

        IF SQL%ROWCOUNT = 0 THEN
            INSERT INTO employee (
                emp_id,
                first_name,
                last_name,
                hire_date,
                salary
            ) VALUES (
                i_emp_id,
                i_first_name,
                i_last_name,
                TO_DATE(i_hire_date, c_date_mask),
                i_salary
            );
        END IF;
    END upsert_employee;

    PROCEDURE select_employee(
        i_emp_id IN NUMBER
    ) IS
        rc SYS_REFCURSOR;
    BEGIN
        OPEN rc FOR
            SELECT emp_id,
                   first_name,
                   last_name,
                   TO_CHAR(hire_date, c_date_mask) AS hire_date,
                   salary
              FROM employee
             WHERE emp_id = i_emp_id;

        DBMS_SQL.return_result(rc);
    END select_employee;

    PROCEDURE delete_employee(
        i_emp_id IN NUMBER
    ) IS
    BEGIN
        DELETE FROM employee
        WHERE emp_id = i_emp_id;
    END delete_employee;
END pkg_employee;
/

